using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Catharsis.Commons;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Data repository which is based on the NHibernate ORM framework.</para>
  /// </summary>
  /// <typeparam name="ENTITY">Type of mapped business entities.</typeparam>
  /// <remarks>This repository implementation is not thread-safe.</remarks>
  public class NHibernateRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    private readonly ISession session;
    private readonly bool ownsSession;

    /// <summary>
    ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    ///   <para>You can force several instances of repository for different entity types share the same <see cref="ISession"/> object, making caching more efficient.</para>
    /// </summary>
    /// <param name="session">NHibernate session, used for operations.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="session"/> is a <c>null</c> reference.</exception>
    public NHibernateRepository(ISession session)
    {
      Assertion.NotNull(session);

      this.session = session;
      this.session.FlushMode = FlushMode.Never;
    }

    /// <summary>
    ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    ///   <para>Each repository instance manages a single NHibernate <see cref="ISession"/> object internally.</para>
    /// </summary>
    /// <param name="sessionFactory">NHibernate session factory, used for creation of <see cref="ISession"/>s.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="sessionFactory"/> is a <c>null</c> reference.</exception>
    public NHibernateRepository(ISessionFactory sessionFactory)
    {
      Assertion.NotNull(sessionFactory);

      this.session = sessionFactory.OpenSession();
      this.session.FlushMode = FlushMode.Never;
      this.ownsSession = true;
    }

    /// <summary>
    ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    ///   <para>Each repository instance manages a single NHibernate <see cref="ISession"/> object internally.</para>
    /// </summary>
    /// <param name="configuration">NHibernate configuration object, used for creation of <see cref="ISessionFactory"/>.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="configuration"/> is a <c>null</c> reference.</exception>
    public NHibernateRepository(Configuration configuration) : this(configuration != null ? configuration.BuildSessionFactory() : null)
    {
    }

    /// <summary>
    ///   <para>Returns enumerator to iterate through entities of <typeparamref name="ENTITY"/> type in the underlying data storage.</para>
    /// </summary>
    /// <returns>Enumerator for iteration through repository's data.</returns>
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return this.Session.Query<ENTITY>().GetEnumerator();
    }

    /// <summary>
    ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="ISession.Flush()"/>
    public override IRepository<ENTITY> Commit()
    {
      this.Session.Flush();
      return this;
    }

    /// <summary>
    ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Business entity to be deleted.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="DeleteAll()"/>
    /// <seealso cref="ISession.Delete(object)"/>
    public override IRepository<ENTITY> Delete(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.Session.Delete(entity);
      return this;
    }

    /// <summary>
    ///   <para>Marks all entities of <typeparamref name="ENTITY"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="Delete(ENTITY)"/>
    /// <seealso cref="ISession.CreateQuery(string)"/>
    /// <seealso cref="ISession.Delete(string)"/>
    public override IRepository<ENTITY> DeleteAll()
    {
      this.Session.CreateQuery("DELETE FROM {0}".FormatSelf(typeof(ENTITY).FullName)).ExecuteUpdate();
      return this;
    }

    /// <summary>
    ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Entity to be added/updated.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="ISession.Save(object)"/>
    public override IRepository<ENTITY> Persist(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.Session.Save(entity);
      return this;
    }

    /// <summary>
    ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
    /// </summary>
    /// <param name="entity">Entity, whose state is to be restored.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="ISession.Refresh(object)"/>
    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.Session.Refresh(entity);
      return this;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    /// </summary>
    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new NHibernateTransaction(this.Session, isolation);
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Expression"/> property.</para>
    /// </summary>
    public override Expression Expression
    {
      get { return this.Session.Query<ENTITY>().Expression; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    public override Type ElementType
    {
      get { return this.Session.Query<ENTITY>().ElementType; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Provider"/> property.</para>
    /// </summary>
    public override IQueryProvider Provider
    {
      get { return this.Session.Query<ENTITY>().Provider; }
    }

    /// <summary>
    ///   <para>Allows direct access to NHibernate <see cref="ISession"/> instance.</para>
    /// </summary>
    public ISession Session
    {
      get { return this.session; }
    }

    protected override void OnDisposing()
    {
      if (this.ownsSession)
      {
        this.Session.Dispose();
      }
    }
  }
}
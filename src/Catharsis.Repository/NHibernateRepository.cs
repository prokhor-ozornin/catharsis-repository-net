using System.Data;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Cfg;
using static NHibernate.FlushMode;

namespace Catharsis.Repository;

/// <summary>
///   <para>Data repository which is based on the NHibernate ORM framework.</para>
/// </summary>
/// <typeparam name="TEntity">Type of mapped business entities.</typeparam>
/// <remarks>This repository implementation is not thread-safe.</remarks>
public class NHibernateRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
{
  private bool OwnsSession { get; }

  /// <summary>
  ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="TEntity"/> type.</para>
  ///   <para>You can force several instances of repository for different entity types share the same <see cref="ISession"/> object, making caching more efficient.</para>
  /// </summary>
  /// <param name="session">NHibernate session, used for operations.</param>
  public NHibernateRepository(ISession session)
  {
    Session = session;
    Session.FlushMode = Manual;
  }

  /// <summary>
  ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="TEntity"/> type.</para>
  ///   <para>Each repository instance manages a single NHibernate <see cref="ISession"/> object internally.</para>
  /// </summary>
  /// <param name="sessionFactory">NHibernate session factory, used for creation of <see cref="ISession"/>s.</param>
  public NHibernateRepository(ISessionFactory sessionFactory)
  {
    Session = sessionFactory.OpenSession();
    Session.FlushMode = Manual;
    OwnsSession = true;
  }

  /// <summary>
  ///   <para>Creates new instance of NHibernate ORM repository that works with mapped entities of <typeparamref name="TEntity"/> type.</para>
  ///   <para>Each repository instance manages a single NHibernate <see cref="ISession"/> object internally.</para>
  /// </summary>
  /// <param name="configuration">NHibernate configuration object, used for creation of <see cref="ISessionFactory"/>.</param>
  public NHibernateRepository(Configuration? configuration) : this(configuration?.BuildSessionFactory())
  {
  }

  /// <summary>
  ///   <para>Returns enumerator to iterate through entities of <typeparamref name="TEntity"/> type in the underlying data storage.</para>
  /// </summary>
  /// <returns>Enumerator for iteration through repository's data.</returns>
  public override IEnumerator<TEntity> GetEnumerator() => Session.Query<TEntity>().GetEnumerator();

  /// <summary>
  ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="ISession.Flush()"/>
  public override IRepository<TEntity> Commit()
  {
    Session.Flush();

    return this;
  }

  /// <summary>
  ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <param name="entity">Business entity to be deleted.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="DeleteAll()"/>
  /// <seealso cref="ISession.Delete(object)"/>
  public override IRepository<TEntity> Delete(TEntity entity)
  {
    Session.Delete(entity);

    return this;
  }

  /// <summary>
  ///   <para>Marks all entities of <typeparamref name="TEntity"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="Delete(TEntity)"/>
  /// <seealso cref="ISession.CreateQuery(string)"/>
  /// <seealso cref="ISession.Delete(string)"/>
  public override IRepository<TEntity> DeleteAll()
  {
    Session.CreateQuery($"DELETE FROM {typeof(TEntity).FullName}").ExecuteUpdate();

    return this;
  }

  /// <summary>
  ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <param name="entity">Entity to be added/updated.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="ISession.Save(object)"/>
  public override IRepository<TEntity> Persist(TEntity entity)
  {
    Session.Save(entity);

    return this;
  }

  /// <summary>
  ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
  /// </summary>
  /// <param name="entity">Entity, whose state is to be restored.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="ISession.Refresh(object)"/>
  public override IRepository<TEntity> Refresh(TEntity entity)
  {
    Session.Refresh(entity);

    return this;
  }

  /// <summary>
  ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
  /// </summary>
  public override ITransaction Transaction(IsolationLevel? isolation = null) => new NHibernateTransaction(Session, isolation);

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.Expression"/> property.</para>
  /// </summary>
  public override Expression Expression => Session.Query<TEntity>().Expression;

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.ElementType"/> property.</para>
  /// </summary>
  public override Type ElementType => Session.Query<TEntity>().ElementType;

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.Provider"/> property.</para>
  /// </summary>
  public override IQueryProvider Provider => Session.Query<TEntity>().Provider;

  /// <summary>
  ///   <para>Allows direct access to NHibernate <see cref="ISession"/> instance.</para>
  /// </summary>
  public ISession Session { get; }

  protected override void OnDisposing()
  {
    if (OwnsSession)
    {
      Session.Dispose();
    }
  }
}
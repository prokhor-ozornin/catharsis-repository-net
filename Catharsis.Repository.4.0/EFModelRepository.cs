using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Data repository which is based on Microsoft Entity Framework ORM (Database-First approach).</para>
  /// </summary>
  /// <typeparam name="ENTITY">Type of mapped business entities.</typeparam>
  /// <remarks>This repository implementation is not thread-safe.</remarks>
  public class EFModelRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    private readonly ObjectContext objectContext;
    private readonly IObjectSet<ENTITY> objectSet;
    private readonly bool ownsContext;

    /// <summary>
    ///   <para>Creates new instance of Entity Framework ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    /// </summary>
    /// <param name="context">Shared <see cref="ObjectContext"/> instance, used for operations. Its lifecycle must be controlled by external code.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="context"/> is a <c>null</c> reference.</exception>
    public EFModelRepository(ObjectContext context)
    {
      Assertion.NotNull(context);

      this.objectContext = context;
      this.objectSet = this.objectContext.CreateObjectSet<ENTITY>();
    }

    /// <summary>
    ///   <para>Creates new instance of Entity Framework ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    /// </summary>
    /// <param name="connection">Connection string, used for internal instantiation of database <see cref="IDbConnection"/> object. Proper closing and life cycle management of created <see cref="IDbConnection"/> object will be performed automatically.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="connection"/> is a <c>null</c> reference.</exception>
    /// <exception cref="ArgumentException">If <paramref name="connection"/> is <see cref="string.Empty"/> string.</exception>
    public EFModelRepository(string connection)
    {
      Assertion.NotEmpty(connection);

      this.objectContext = new ObjectContext(connection);
      this.objectSet = this.objectContext.CreateObjectSet<ENTITY>();
      this.ownsContext = true;
    }

    /// <summary>
    ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="System.Data.Entity.Core.Objects.ObjectContext.SaveChanges()"/>
    public override IRepository<ENTITY> Commit()
    {
      this.ObjectContext.SaveChanges();
      return this;
    }

    /// <summary>
    ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Business entity to be deleted.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="DeleteAll()"/>
    /// <seealso cref="IObjectSet{ENTITY}.DeleteObject(ENTITY)"/>
    public override IRepository<ENTITY> Delete(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.objectSet.DeleteObject(entity);
      return this;
    }

    /// <summary>
    ///   <para>Marks all entities of <typeparamref name="ENTITY"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="Delete(ENTITY)"/>
    /// <remarks>Current implementation does not support this operation, always throwing <see cref="NotSupportedException"/>.</remarks>
    public override IRepository<ENTITY> DeleteAll()
    {
      foreach (var entity in this.objectSet)
      {
        this.objectSet.DeleteObject(entity);
      }

      return this;
    }

    /// <summary>
    ///   <para>Returns enumerator to iterate through entities of <typeparamref name="ENTITY"/> type in the underlying data storage.</para>
    /// </summary>
    /// <returns>Enumerator for iteration through repository's data.</returns>
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return this.objectSet.GetEnumerator();
    }

    /// <summary>
    ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Entity to be added/updated.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IObjectSet{ENTITY}.AddObject(ENTITY)"/>
    public override IRepository<ENTITY> Persist(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.objectSet.AddObject(entity);
      return this;
    }

    /// <summary>
    ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
    /// </summary>
    /// <param name="entity">Entity, whose state is to be restored.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="System.Data.Entity.Core.Objects.ObjectContext.Refresh(RefreshMode, object)"/>
    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.ObjectContext.Refresh(RefreshMode.StoreWins, entity);
      return this;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    /// </summary>
    /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
    /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new AdoNetTransaction(this.ObjectContext.Connection, isolation);
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Expression"/> property.</para>
    /// </summary>
    public override Expression Expression
    {
      get { return this.objectSet.Expression; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    public override Type ElementType
    {
      get { return this.objectSet.ElementType; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Provider"/> property.</para>
    /// </summary>
    public override IQueryProvider Provider
    {
      get { return this.objectSet.Provider; }
    }

    /// <summary>
    ///   <para>Allows direct access to Entity Framework <see cref="ObjectContext"/> instance.</para>
    /// </summary>
    public ObjectContext ObjectContext
    {
      get { return this.objectContext; }
    }

    protected override void OnDisposing()
    {
      if (this.ownsContext)
      {
        this.ObjectContext.Dispose();
      }
    }
  }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Data repository which is based on Microsoft Entity Framework ORM (Code-First approach).</para>
  /// </summary>
  /// <typeparam name="ENTITY">Type of mapped business entities.</typeparam>
  /// <remarks>This repository implementation is not thread-safe.</remarks>
  public class EFCodeFirstRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    private readonly DbContext dbContext;

    /// <summary>
    ///   <para></para>
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException">If <paramref name="context"/> is a <c>null</c> reference.</exception>
    public EFCodeFirstRepository(DbContext context)
    {
      Assertion.NotNull(context);

      this.dbContext = context;
    }

    /// <summary>
    ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="ObjectContext.SaveChanges()"/>
    public override IRepository<ENTITY> Commit()
    {
      this.DbContext.SaveChanges();
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

      this.DbContext.Set<ENTITY>().Remove(entity);
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
      this.DbContext.Set<ENTITY>().RemoveRange(this.dbContext.Set<ENTITY>());
      return this;
    }

    /// <summary>
    ///   <para>Returns enumerator to iterate through entities of <typeparamref name="ENTITY"/> type in the underlying data storage.</para>
    /// </summary>
    /// <returns>Enumerator for iteration through repository's data.</returns>
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return this.DbContext.Set<ENTITY>().AsQueryable().GetEnumerator();
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

      this.DbContext.Set<ENTITY>().Add(entity);
      return this;
    }

    /// <summary>
    ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
    /// </summary>
    /// <param name="entity">Entity, whose state is to be restored.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="ObjectContext.Refresh(RefreshMode, object)"/>
    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.DbContext.Entry(entity).Reload();
      return this;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    /// </summary>
    /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
    /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new AdoNetTransaction(this.DbContext.Database.Connection, isolation);
    }

    /// <summary>
    ///   <para>Allows direct access to Entity Framework <see cref="DbContext"/> instance.</para>
    /// </summary>
    public DbContext DbContext
    {
      get { return this.dbContext; }
    }

    protected override void OnDisposing()
    {
      this.DbContext.Dispose();
    }
  }
}
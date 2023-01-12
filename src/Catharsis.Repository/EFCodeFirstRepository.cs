using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Catharsis.Repository;

/// <summary>
///   <para>Data repository which is based on Microsoft Entity Framework ORM (Code-First approach).</para>
/// </summary>
/// <typeparam name="TEntity">Type of mapped business entities.</typeparam>
/// <remarks>This repository implementation is not thread-safe.</remarks>
public class EFCodeFirstRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
{
  /// <summary>
  ///   <para>Allows direct access to Entity Framework <see cref="DbContext"/> instance.</para>
  /// </summary>
  public DbContext DbContext { get; }

  /// <summary>
  ///   <para></para>
  /// </summary>
  /// <param name="context"></param>
  public EFCodeFirstRepository(DbContext context) => DbContext = context;

  /// <summary>
  ///   <para>Returns enumerator to iterate through entities of <typeparamref name="TEntity"/> type in the underlying data storage.</para>
  /// </summary>
  /// <returns>Enumerator for iteration through repository's data.</returns>
  public override IEnumerator<TEntity> GetEnumerator() => DbContext.Set<TEntity>().AsQueryable().GetEnumerator();

  /// <summary>
  ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="ObjectContext.SaveChanges()"/>
  public override IRepository<TEntity> Commit()
  {
    DbContext.SaveChanges();

    return this;
  }

  /// <summary>
  ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <param name="entity">Business entity to be deleted.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="DeleteAll()"/>
  /// <seealso cref="IObjectSet{TEntity}.DeleteObject(TEntity)"/>
  public override IRepository<TEntity> Delete(TEntity entity)
  {
    DbContext.Set<TEntity>().Remove(entity);

    return this;
  }

  /// <summary>
  ///   <para>Marks all entities of <typeparamref name="TEntity"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="Delete(TEntity)"/>
  /// <remarks>Current implementation does not support this operation, always throwing <see cref="NotSupportedException"/>.</remarks>
  public override IRepository<TEntity> DeleteAll()
  {
    DbContext.Set<TEntity>().RemoveRange(DbContext.Set<TEntity>());

    return this;
  }

  /// <summary>
  ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
  /// </summary>
  /// <param name="entity">Entity to be added/updated.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="IObjectSet{TEntity}.AddObject(TEntity)"/>
  public override IRepository<TEntity> Persist(TEntity entity)
  {
    DbContext.Set<TEntity>().Add(entity);

    return this;
  }

  /// <summary>
  ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
  /// </summary>
  /// <param name="entity">Entity, whose state is to be restored.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="ObjectContext.Refresh(RefreshMode, object)"/>
  public override IRepository<TEntity> Refresh(TEntity entity)
  {
    DbContext.Entry(entity).Reload();

    return this;
  }

  /// <summary>
  ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
  /// </summary>
  /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
  /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
  public override ITransaction Transaction(IsolationLevel? isolation = null) => new AdoNetTransaction(DbContext.Database.Connection, isolation);

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.Expression"/> property.</para>
  /// </summary>
  public override Expression Expression => DbContext.Set<TEntity>().AsQueryable().Expression;

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.ElementType"/> property.</para>
  /// </summary>
  public override Type ElementType => DbContext.Set<TEntity>().AsQueryable().ElementType;

  /// <summary>
  ///   <para>Implementation of <see cref="IQueryable{TEntity}.Provider"/> property.</para>
  /// </summary>
  public override IQueryProvider Provider => DbContext.Set<TEntity>().AsQueryable().Provider;
}
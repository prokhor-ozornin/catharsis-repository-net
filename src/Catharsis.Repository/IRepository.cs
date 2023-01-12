using System.Data;

namespace Catharsis.Repository;

/// <summary>
///   <para>Representation of a generic data storage facility for working with persistent objects (business entities).</para>
///   <para>Each repository instance manages persistent stage of particular type of business entities.</para>
///   <para>Particular persistence logic depends upon the implementation.</para>
/// </summary>
/// <typeparam name="TEntity">Type of business entities.</typeparam>
public interface IRepository<TEntity> : IDisposable, IQueryable<TEntity> where TEntity : class
{
  /// <summary>
  ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  IRepository<TEntity> Commit();

  /// <summary>
  ///   <para>Marks specified business entity as deleted. Actual deletion is performed when <see cref="Commit()"/> method is called or parent transaction is successfully committed.</para>
  /// </summary>
  /// <param name="entity">Business entity to be deleted.</param>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="DeleteAll()"/>
  IRepository<TEntity> Delete(TEntity entity);

  /// <summary>
  ///   <para>Marks all entities of <typeparamref name="TEntity"/> type as deleted. Actual deletion is performed when <see cref="Commit()"/> method is called or parent transaction is successfully committed.</para>
  /// </summary>
  /// <returns>Back reference to the current repository.</returns>
  /// <seealso cref="Delete(TEntity)"/>
  IRepository<TEntity> DeleteAll();

  /// <summary>
  ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when <see cref="Commit()"/> method is called or parent transaction is successfully committed.</para>
  /// </summary>
  /// <param name="entity">Entity to be added/updated.</param>
  /// <returns>Back reference to the current repository.</returns>
  IRepository<TEntity> Persist(TEntity entity);

  /// <summary>
  ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
  /// </summary>
  /// <param name="entity">Entity, whose state is to be restored.</param>
  /// <returns>Back reference to the current repository.</returns>
  IRepository<TEntity> Refresh(TEntity entity);

  /// <summary>
  ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
  /// </summary>
  /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
  /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
  ITransaction Transaction(IsolationLevel? isolation = null);
}
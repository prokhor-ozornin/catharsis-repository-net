using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Simple data repository which is purely memory-based.</para>
  ///   <para>This implementation does not support transactional behaviour.</para>
  /// </summary>
  /// <typeparam name="ENTITY">Type of mapped business entities.</typeparam>
  /// <remarks>This repository implementation is not thread-safe.</remarks>
  public class MemoryRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    private readonly ICollection<ENTITY> entities = new HashSet<ENTITY>();

    /// <summary>
    ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <remarks>This implementation simply returns reference to the current repository.</remarks>
    public override IRepository<ENTITY> Commit()
    {
      return this;
    }

    /// <summary>
    ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Business entity to be deleted.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <remarks>This implementation performs deletion operations instantly.</remarks>
    /// <seealso cref="DeleteAll()"/>
    public override IRepository<ENTITY> Delete(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.entities.Remove(entity);
      return this;
    }

    /// <summary>
    ///   <para>Marks all entities of <typeparamref name="ENTITY"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <remarks>This implementation performs deletion operations instantly.</remarks>
    /// <seealso cref="Delete(ENTITY)"/>
    public override IRepository<ENTITY> DeleteAll()
    {
      this.entities.Clear();
      return this;
    }

    /// <summary>
    ///   <para>Returns enumerator to iterate through entities of <typeparamref name="ENTITY"/> type in the underlying data storage.</para>
    /// </summary>
    /// <returns>Enumerator for iteration through repository's data.</returns>
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return this.entities.GetEnumerator();
    }

    /// <summary>
    ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Entity to be added/updated.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <remarks>In this implementation the state of persisted entities in data storage is always the same as current state of these objects.</remarks>
    public override IRepository<ENTITY> Persist(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.entities.Add(entity);
      return this;
    }

    /// <summary>
    ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
    /// </summary>
    /// <param name="entity">Entity, whose state is to be restored.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <remarks>This implementation simply returns reference to the current repository.</remarks>
    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      Assertion.NotNull(entity);

      return this;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    /// </summary>
    /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
    /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new NoOpTransaction();
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Expression"/> property.</para>
    /// </summary>
    public override Expression Expression
    {
      get { return this.entities.AsQueryable().Expression; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    public override Type ElementType
    {
      get { return this.entities.AsQueryable().ElementType; }
    }

    /// <summary>
    ///   <para>Implementation of <see cref="IQueryable{ENTITY}.Provider"/> property.</para>
    /// </summary>
    public override IQueryProvider Provider
    {
      get { return this.entities.AsQueryable().Provider; }
    }
  }
}
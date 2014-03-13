using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Data repository which is based on LinqToSQL (<see cref="System.Data.Linq"/>) persistence provider.</para>
  /// </summary>
  /// <typeparam name="ENTITY">Type of mapped business entities.</typeparam>
  /// <remarks>This repository implementation is not thread-safe.</remarks>
  public class LinqToSqlRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    private readonly DataContext dataContext;

    /// <summary>
    ///   <para>Creates new instance of LinqToSQL ORM repository that works with mapped entities of <typeparamref name="ENTITY"/> type.</para>
    /// </summary>
    /// <param name="connection">Database connection object, internaly used for performing of SQL queries. Connection must be in open state. Proper closing and life cycle management of this connection is up to external calling code.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="connection"/> is a <c>null</c> reference.</exception>
    public LinqToSqlRepository(IDbConnection connection)
    {
      Assertion.NotNull(connection);

      this.dataContext = new DataContext(connection);
    }

    /// <summary>
    ///   <para>Saves all non-persisted changes to the underlying data storage facility by persisting modified entities and deleting those which have been marked as deleted.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="System.Data.Linq.DataContext.SubmitChanges()"/>
    public override IRepository<ENTITY> Commit()
    {
      this.DataContext.SubmitChanges();
      return this;
    }

    /// <summary>
    ///   <para>Marks specified business entity as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Business entity to be deleted.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="DeleteAll()"/>
    /// <seealso cref="Table{ENTITY}.DeleteOnSubmit(ENTITY)"/>
    public override IRepository<ENTITY> Delete(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.DataContext.GetTable<ENTITY>().DeleteOnSubmit(entity);
      return this;
    }

    /// <summary>
    ///   <para>Marks all entities of <typeparamref name="ENTITY"/> type as deleted. Actual deletion is performed when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <returns>Back reference to the current repository.</returns>
    /// <seealso cref="Delete(ENTITY)"/>
    /// <seealso cref="Table{ENTITY}.DeleteAllOnSubmit{ENTITY}(IEnumerable{ENTITY})"/>
    public override IRepository<ENTITY> DeleteAll()
    {
      this.DataContext.GetTable<ENTITY>().DeleteAllOnSubmit(this.DataContext.GetTable<ENTITY>());
      return this;
    }

    /// <summary>
    ///   <para>Returns enumerator to iterate through entities of <typeparamref name="ENTITY"/> type in the underlying data storage.</para>
    /// </summary>
    /// <returns>Enumerator for iteration through repository's data.</returns>
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return this.DataContext.GetTable<ENTITY>().GetEnumerator();
    }

    /// <summary>
    ///   <para>Persists state of specified entity in the underlying data storage. Either a new entity will be created, or a state of the already existing one will be updated when either <see cref="Commit()"/> method is called or this call is made inside a transaction.</para>
    /// </summary>
    /// <param name="entity">Entity to be added/updated.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="Table{ENTITY}.InsertOnSubmit(ENTITY)"/>
    public override IRepository<ENTITY> Persist(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.DataContext.GetTable<ENTITY>().InsertOnSubmit(entity);
      return this;
    }

    /// <summary>
    ///   <para>Restores original state of modified entity from values in the underlying data storage. Local changes, which were made to the non-persisted <paramref name="entity"/>, will be lost.</para>
    /// </summary>
    /// <param name="entity">Entity, whose state is to be restored.</param>
    /// <returns>Back reference to the current repository.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="System.Data.Linq.DataContext.Refresh(RefreshMode, object)"/>
    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      Assertion.NotNull(entity);

      this.DataContext.Refresh(RefreshMode.OverwriteCurrentValues, entity);
      return this;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    /// </summary>
    /// <param name="isolation">Transaction isolation level. If not specified, default isolation level for underlying data storage will be used.</param>
    /// <returns>Initialized and started transaction. When <see cref="ITransaction.Dispose()"/> method is called, transaction commit is performed.</returns>
    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new AdoNetTransaction(this.DataContext.Connection, isolation);
    }

    /// <summary>
    ///   <para>Allows direct access to LINQ to SQL <see cref="DataContext"/> instance.</para>
    /// </summary>
    public DataContext DataContext
    {
      get { return this.dataContext; }
    }

    protected override void OnDisposing()
    {
      this.DataContext.Dispose();
    }
  }
}
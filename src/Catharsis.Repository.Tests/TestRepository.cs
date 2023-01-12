using System.Data;
using System.Linq.Expressions;

namespace Catharsis.Repository.Tests;

internal sealed class TestRepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
{
  public override IEnumerator<TEntity> GetEnumerator() => Enumerable.Empty<TEntity>().GetEnumerator();

  public override IRepository<TEntity> Commit() => this;

  public override IRepository<TEntity> Delete(TEntity entity) => this;

  public override IRepository<TEntity> DeleteAll() => this;

  public override IRepository<TEntity> Persist(TEntity entity) => this;

  public override IRepository<TEntity> Refresh(TEntity entity) => this;

  public override ITransaction Transaction(IsolationLevel? isolation = null) => new TestTransaction(isolation);

  public override Expression Expression => Enumerable.Empty<TEntity>().AsQueryable().Expression;

  public override Type ElementType => Enumerable.Empty<TEntity>().AsQueryable().ElementType;

  public override IQueryProvider Provider => Enumerable.Empty<TEntity>().AsQueryable().Provider;

  private sealed class TestTransaction : ITransaction
  {
    private bool Disposed
    {
      get; set;
    }

    public IsolationLevel IsolationLevel
    {
      get;
    }

    public TestTransaction(IsolationLevel? isolation = null) => IsolationLevel = isolation ?? IsolationLevel.Unspecified;

    public void Dispose()
    {
      if (Disposed)
      {
        throw new ObjectDisposedException(ToString());
      }

      Disposed = true;
    }

    public ITransaction Commit() => this;

    public ITransaction Rollback() => this;
  }
}
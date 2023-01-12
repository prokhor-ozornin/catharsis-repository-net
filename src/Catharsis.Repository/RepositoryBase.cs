using System.Collections;
using System.Data;
using System.Linq.Expressions;

namespace Catharsis.Repository;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class
{
  private bool disposed;

  public virtual void Dispose()
  {
    if (disposed)
    {
      return;
    }

    OnDisposing();
    disposed = true;
  }

  public abstract IEnumerator<TEntity> GetEnumerator();

  public abstract IRepository<TEntity> Commit();

  public abstract IRepository<TEntity> Delete(TEntity entity);

  public abstract IRepository<TEntity> DeleteAll();

  public abstract IRepository<TEntity> Persist(TEntity entity);

  public abstract IRepository<TEntity> Refresh(TEntity entity);

  public abstract ITransaction Transaction(IsolationLevel? isolation = null);

  public abstract Expression Expression { get; }

  public abstract Type ElementType { get; }

  public abstract IQueryProvider Provider { get; }

  protected virtual void OnDisposing()
  {
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
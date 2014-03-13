using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Catharsis.Repository
{
  public abstract class RepositoryBase<ENTITY> : IRepository<ENTITY> where ENTITY : class
  {
    private bool disposed;

    public virtual void Dispose()
    {
      if (disposed)
      {
        return;
      }

      this.OnDisposing();
      this.disposed = true;
    }

    public abstract IEnumerator<ENTITY> GetEnumerator();

    public abstract IRepository<ENTITY> Commit();

    public abstract IRepository<ENTITY> Delete(ENTITY entity);

    public abstract IRepository<ENTITY> DeleteAll();

    public abstract IRepository<ENTITY> Persist(ENTITY entity);

    public abstract IRepository<ENTITY> Refresh(ENTITY entity);

    public abstract ITransaction Transaction(IsolationLevel? isolation = null);

    protected virtual void OnDisposing()
    {
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
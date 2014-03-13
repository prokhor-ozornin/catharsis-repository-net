using System;
using System.Data;

namespace Catharsis.Repository
{
  internal sealed class MockTransaction : ITransaction
  {
    private bool disposed;
    private IsolationLevel isolation;
    private bool wasCommitted;
    private bool wasRolledBack;

    public MockTransaction(IsolationLevel? isolation = null)
    {
      this.isolation = isolation ?? IsolationLevel.Unspecified;
    }

    public void Dispose()
    {
      if (this.disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }

      this.disposed = true;
    }

    public ITransaction Commit()
    {
      this.wasCommitted = true;
      return this;
    }

    public ITransaction Rollback()
    {
      this.wasRolledBack = true;
      return this;
    }

    public IsolationLevel IsolationLevel
    {
      get { return this.isolation; }
    }

    public bool WasCommitted
    {
      get { return this.wasCommitted; }
    }

    public bool WasRolledBack
    {
      get { return this.wasRolledBack; }
    }
  }
}
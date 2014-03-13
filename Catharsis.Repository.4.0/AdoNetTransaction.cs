using System;
using System.Data;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  internal sealed class AdoNetTransaction : ITransaction
  {
    private readonly IDbTransaction transaction;
    private bool disposed;
    private bool wasCommitted;
    private bool wasRolledBack;

    public AdoNetTransaction(IDbConnection connection, IsolationLevel? isolation = null)
    {
      Assertion.NotNull(connection);

      if (connection.State == ConnectionState.Broken || connection.State == ConnectionState.Closed)
      {
        connection.Open();
      }

      this.transaction = isolation != null ? connection.BeginTransaction(isolation.Value) : connection.BeginTransaction();
    }

    public void Dispose()
    {
      if (this.disposed)
      {
        throw new ObjectDisposedException(this.ToString());
      }

      if (this.wasCommitted && !this.wasRolledBack)
      {
        this.transaction.Commit();
      }
      else
      {
        this.transaction.Rollback();
      }

      this.transaction.Dispose();
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
      get { return this.transaction.IsolationLevel; }
    }
  }
}
using System.Data;

namespace Catharsis.Repository;

internal sealed class AdoNetTransaction : ITransaction
{
  private IDbTransaction Transaction { get; }
  private bool Disposed { get; set; }
  private bool WasCommitted { get; set; }
  private bool WasRolledBack { get; set; }

  public AdoNetTransaction(IDbConnection connection, IsolationLevel? isolation = null)
  {
    if (connection.State is ConnectionState.Broken or ConnectionState.Closed)
    {
      connection.Open();
    }

    Transaction = isolation != null ? connection.BeginTransaction(isolation.Value) : connection.BeginTransaction();
  }

  public void Dispose()
  {
    if (Disposed)
    {
      throw new ObjectDisposedException(ToString());
    }

    if (WasCommitted && !WasRolledBack)
    {
      Transaction.Commit();
    }
    else
    {
      Transaction.Rollback();
    }

    Transaction.Dispose();
    Disposed = true;
  }

  public ITransaction Commit()
  {
    WasCommitted = true;

    return this;
  }

  public ITransaction Rollback()
  {
    WasRolledBack = true;

    return this;
  }

  public IsolationLevel IsolationLevel => Transaction.IsolationLevel;
}
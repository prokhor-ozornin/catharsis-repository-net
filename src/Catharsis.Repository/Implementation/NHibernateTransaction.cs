using System.Data;
using NHibernate;

namespace Catharsis.Repository;

internal sealed class NHibernateTransaction : ITransaction
{
  private bool Disposed { get; set; }
  private bool WasCommitted { get; set; }
  private bool WasRolledBack { get; set; }
  private ISession Session { get; }
  private NHibernate.ITransaction Transaction { get; }
  private IsolationLevel Isolation { get; }

  public IsolationLevel IsolationLevel => Isolation;

  public NHibernateTransaction(ISession session, IsolationLevel? isolation = null)
  {
    Session = session;
    Isolation = isolation ?? IsolationLevel.Unspecified;
    Transaction = isolation is not null ? session.BeginTransaction(isolation.Value) : session.BeginTransaction();
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
      Session.Flush();
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
}
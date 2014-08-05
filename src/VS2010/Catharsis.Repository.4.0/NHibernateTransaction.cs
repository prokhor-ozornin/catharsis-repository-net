using System;
using System.Data;
using Catharsis.Commons;
using NHibernate;

namespace Catharsis.Repository
{
  internal sealed class NHibernateTransaction : ITransaction
  {
    private readonly ISession session;
    private readonly NHibernate.ITransaction transaction;
    private readonly IsolationLevel isolation;
    private bool disposed;
    private bool wasCommitted;
    private bool wasRolledBack;

    public NHibernateTransaction(ISession session, IsolationLevel? isolation = null)
    {
      Assertion.NotNull(session);

      this.session = session;
      this.transaction = isolation != null ? session.BeginTransaction(isolation.Value) : session.BeginTransaction();
      this.isolation = isolation ?? IsolationLevel.Unspecified;
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
        this.session.Flush();
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
      get { return this.isolation; }
    }
  }
}
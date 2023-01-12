using System.Data;

namespace Catharsis.Repository;

internal sealed class NoOpTransaction : ITransaction
{
  private readonly IsolationLevel isolation;

  public NoOpTransaction(IsolationLevel? isolation = null)
  {
    this.isolation = isolation ?? IsolationLevel.Unspecified;

    if (isolation != null)
    {
      this.isolation = isolation.Value;
    }
  }

  public void Dispose()
  {
  }

  public ITransaction Commit() => this;

  public ITransaction Rollback() => this;

  public IsolationLevel IsolationLevel => isolation;
}
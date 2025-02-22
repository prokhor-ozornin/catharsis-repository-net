using System.Data;

namespace Catharsis.Repository;

internal sealed class NoOpTransaction : ITransaction
{
  private IsolationLevel Isolation { get; }

  public NoOpTransaction(IsolationLevel? isolation = null)
  {
    Isolation = isolation ?? IsolationLevel.Unspecified;

    if (isolation is not null)
    {
      Isolation = isolation.Value;
    }
  }

  public void Dispose()
  {
  }

  public ITransaction Commit() => this;

  public ITransaction Rollback() => this;

  public IsolationLevel IsolationLevel => Isolation;
}
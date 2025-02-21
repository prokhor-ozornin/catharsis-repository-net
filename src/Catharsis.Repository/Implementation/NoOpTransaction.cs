using System.Data;

namespace Catharsis.Repository;

internal sealed class NoOpTransaction : ITransaction
{
  private readonly IsolationLevel _isolation;

  public NoOpTransaction(IsolationLevel? isolation = null)
  {
    _isolation = isolation ?? IsolationLevel.Unspecified;

    if (isolation is not null)
    {
      _isolation = isolation.Value;
    }
  }

  public void Dispose()
  {
  }

  public ITransaction Commit() => this;

  public ITransaction Rollback() => this;

  public IsolationLevel IsolationLevel => _isolation;
}
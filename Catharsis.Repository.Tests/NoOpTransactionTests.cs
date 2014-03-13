using System.Data;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="NoOpTransaction"/>.</para>
  /// </summary>
  public sealed class NoOpTransactionTests
  {
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="NoOpTransaction(IsolationLevel?)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      using (var transaction = new NoOpTransaction())
      {
        Assert.Equal(IsolationLevel.Unspecified, transaction.IsolationLevel);
      }

      using (var transaction = new NoOpTransaction(IsolationLevel.ReadCommitted))
      {
        Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NoOpTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var transaction = new NoOpTransaction();
      transaction.Dispose();
      transaction.Dispose();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NoOpTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var transaction = new NoOpTransaction();
      Assert.True(ReferenceEquals(transaction.Commit(), transaction));
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NoOpTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
      var transaction = new NoOpTransaction();
      Assert.True(ReferenceEquals(transaction.Rollback(), transaction));
    }
  }
}
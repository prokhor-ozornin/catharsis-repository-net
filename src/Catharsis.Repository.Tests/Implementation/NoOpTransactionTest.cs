using System.Data;
using FluentAssertions;
using Xunit;

namespace Catharsis.Repository.Tests.Implementation;

/// <summary>
///   <para>Tests set for class <see cref="NoOpTransaction"/>.</para>
/// </summary>
public sealed class NoOpTransactionTest
{
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    /// </summary>
    /// <seealso cref="NoOpTransaction(IsolationLevel?)"/>
    [Fact]
    public void Constructors()
    {
        using (var transaction = new NoOpTransaction())
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.Unspecified);
        }

        using (var transaction = new NoOpTransaction(IsolationLevel.ReadCommitted))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
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
        transaction.Commit().Should().BeSameAs(transaction);
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NoOpTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
        var transaction = new NoOpTransaction();
        transaction.Rollback().Should().BeSameAs(transaction);
    }
}
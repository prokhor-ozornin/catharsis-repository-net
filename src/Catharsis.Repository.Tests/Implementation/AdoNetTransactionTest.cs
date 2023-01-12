using System.Configuration;
using System.Data;
using System.Data.Common;
using Catharsis.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests.Implementation;

/// <summary>
///   <para>Tests set for class <see cref="AdoNetTransaction"/>.</para>
/// </summary>
public sealed class AdoNetTransactionTest
{
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    /// </summary>
    /// <seealso cref="AdoNetTransaction(IDbConnection, IsolationLevel?)"/>
    [Fact]
    public void Constructors()
    {
        using (new AssertionScope())
        {
            AssertionExtensions.Should(() => new AdoNetTransaction(null)).ThrowExactly<ArgumentNullException>();
        }

        using var connection = Connection();

        using (var transaction = new AdoNetTransaction(connection))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
            transaction.Field("disposed").To<bool>().Should().BeFalse();
            transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
            transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        }
        using (var transaction = new AdoNetTransaction(connection, IsolationLevel.ReadCommitted))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
            transaction.Field("disposed").To<bool>().Should().BeFalse();
            transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
            transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
        using var connection = Connection();

        var transaction = new AdoNetTransaction(connection);
        transaction.Dispose();
        transaction.Field("disposed").To<bool>().Should().BeTrue();
        transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
        transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        AssertionExtensions.Should(() => transaction.Dispose()).ThrowExactly<ObjectDisposedException>();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
        using var connection = Connection();

        var transaction = new AdoNetTransaction(connection);
        transaction.Commit().Should().BeSameAs(transaction);
        transaction.Field("disposed").To<bool>().Should().BeFalse();
        transaction.Field("wasCommitted").To<bool>().Should().BeTrue();
        transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
        using var connection = Connection();

        var transaction = new AdoNetTransaction(connection);
        transaction.Rollback().Should().BeSameAs(transaction);
        transaction.Field("disposed").To<bool>().Should().BeFalse();
        transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
        transaction.Field("wasRolledBack").To<bool>().Should().BeTrue();
    }

    private IDbConnection Connection()
    {
        var connection = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection();
        connection.ConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
        connection.Open();

        return connection;
    }
}
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Catharsis.Commons;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="AdoNetTransaction"/>.</para>
  /// </summary>
  public sealed class AdoNetTransactionTests
  {
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    /// </summary>
    /// <seealso cref="AdoNetTransaction(IDbConnection, IsolationLevel?)"/>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new AdoNetTransaction(null));

      using (var connection = this.Connection())
      {
        using (var transaction = new AdoNetTransaction(connection))
        {
          Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
          Assert.False(transaction.Field("disposed").To<bool>());
          Assert.False(transaction.Field("wasCommitted").To<bool>());
          Assert.False(transaction.Field("wasRolledBack").To<bool>());
        }
        using (var transaction = new AdoNetTransaction(connection, IsolationLevel.ReadCommitted))
        {
          Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
          Assert.False(transaction.Field("disposed").To<bool>());
          Assert.False(transaction.Field("wasCommitted").To<bool>());
          Assert.False(transaction.Field("wasRolledBack").To<bool>());
        }
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      using (var connection = this.Connection())
      {
        var transaction = new AdoNetTransaction(connection);
        transaction.Dispose();
        Assert.True(transaction.Field("disposed").To<bool>());
        Assert.False(transaction.Field("wasCommitted").To<bool>());
        Assert.False(transaction.Field("wasRolledBack").To<bool>());
        Assert.Throws<ObjectDisposedException>(() => transaction.Dispose());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      using (var connection = this.Connection())
      {
        var transaction = new AdoNetTransaction(connection);
        Assert.True(ReferenceEquals(transaction.Commit(), transaction));
        Assert.False(transaction.Field("disposed").To<bool>());
        Assert.True(transaction.Field("wasCommitted").To<bool>());
        Assert.False(transaction.Field("wasRolledBack").To<bool>());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="AdoNetTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
      using (var connection = this.Connection())
      {
        var transaction = new AdoNetTransaction(connection);
        Assert.True(ReferenceEquals(transaction.Rollback(), transaction));
        Assert.False(transaction.Field("disposed").To<bool>());
        Assert.False(transaction.Field("wasCommitted").To<bool>());
        Assert.True(transaction.Field("wasRolledBack").To<bool>());
      }
    }

    private IDbConnection Connection()
    {
      var connection = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection();
      connection.ConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
      connection.Open();

      return connection;
    }
  }
}
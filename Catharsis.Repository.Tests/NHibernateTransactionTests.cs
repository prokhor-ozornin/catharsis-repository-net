using System;
using System.Data;
using Catharsis.Commons;
using NHibernate;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="NHibernateTransaction"/>.</para>
  /// </summary>
  public sealed class NHibernateTransactionTests
  {
    private readonly ISessionFactory sessionFactory = Bootstrapper.NHibernate().BuildSessionFactory();

    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="NHibernateTransaction(ISession, IsolationLevel?)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new NHibernateTransaction(null));
      
      using (var connection = this.sessionFactory.OpenSession())
      {
        using (var transaction = new NHibernateTransaction(connection))
        {
          Assert.Equal(IsolationLevel.Unspecified, transaction.IsolationLevel);
          Assert.False(transaction.Field("disposed").To<bool>());
          Assert.False(transaction.Field("wasCommitted").To<bool>());
          Assert.False(transaction.Field("wasRolledBack").To<bool>());
        }
        using (var transaction = new NHibernateTransaction(connection, IsolationLevel.ReadCommitted))
        {
          Assert.Equal(IsolationLevel.ReadCommitted, transaction.IsolationLevel);
          Assert.False(transaction.Field("disposed").To<bool>());
          Assert.False(transaction.Field("wasCommitted").To<bool>());
          Assert.False(transaction.Field("wasRolledBack").To<bool>());
        }
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      using (var connection = this.sessionFactory.OpenSession())
      {
        var transaction = new NHibernateTransaction(connection);
        transaction.Dispose();
        Assert.True(transaction.Field("disposed").To<bool>());
        Assert.False(transaction.Field("wasCommitted").To<bool>());
        Assert.False(transaction.Field("wasRolledBack").To<bool>());
        Assert.Throws<ObjectDisposedException>(() => transaction.Dispose());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      using (var connection = this.sessionFactory.OpenSession())
      {
        var transaction = new NHibernateTransaction(connection);
        Assert.True(ReferenceEquals(transaction.Commit(), transaction));
        Assert.False(transaction.Field("disposed").To<bool>());
        Assert.True(transaction.Field("wasCommitted").To<bool>());
        Assert.False(transaction.Field("wasRolledBack").To<bool>());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
      using (var connection = this.sessionFactory.OpenSession())
      {
        var transaction = new NHibernateTransaction(connection);
        Assert.True(ReferenceEquals(transaction.Rollback(), transaction));
        Assert.False(transaction.Field("disposed").To<bool>());
        Assert.False(transaction.Field("wasCommitted").To<bool>());
        Assert.True(transaction.Field("wasRolledBack").To<bool>());
      }
    }
  }
}
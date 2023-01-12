using System.Data;
using Catharsis.Extensions;
using NHibernate;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests.Implementation;

/// <summary>
///   <para>Tests set for class <see cref="NHibernateTransaction"/>.</para>
/// </summary>
public sealed class NHibernateTransactionTest
{
    private readonly ISessionFactory sessionFactory = Bootstrapper.NHibernate().BuildSessionFactory();

    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    /// </summary>
    /// <seealso cref="NHibernateTransaction(ISession, IsolationLevel?)"/>
    [Fact]
    public void Constructors()
    {
        using (new AssertionScope())
        {
            AssertionExtensions.Should(() => new NHibernateTransaction(null)).ThrowExactly<ArgumentNullException>();
        }

        using var connection = sessionFactory.OpenSession();

        using (var transaction = new NHibernateTransaction(connection))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.Unspecified);
            transaction.Field("disposed").To<bool>().Should().BeFalse();
            transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
            transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        }

        using (var transaction = new NHibernateTransaction(connection, IsolationLevel.ReadCommitted))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
            transaction.Field("disposed").To<bool>().Should().BeFalse();
            transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
            transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
        using var connection = sessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Dispose();
        transaction.Field("disposed").To<bool>().Should().BeTrue();
        transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
        transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
        AssertionExtensions.Should(() => transaction.Dispose()).ThrowExactly<ObjectDisposedException>();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
        using var connection = sessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Commit().Should().BeSameAs(transaction);
        transaction.Field("disposed").To<bool>().Should().BeFalse();
        transaction.Field("wasCommitted").To<bool>().Should().BeTrue();
        transaction.Field("wasRolledBack").To<bool>().Should().BeFalse();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
        using var connection = sessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Rollback().Should().BeSameAs(transaction);
        transaction.Field("disposed").To<bool>().Should().BeFalse();
        transaction.Field("wasCommitted").To<bool>().Should().BeFalse();
        transaction.Field("wasRolledBack").To<bool>().Should().BeTrue();
    }
}
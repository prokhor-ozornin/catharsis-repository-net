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
    private ISessionFactory SessionFactory { get; } = Bootstrapper.NHibernate().BuildSessionFactory();

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

        using var connection = SessionFactory.OpenSession();

        using (var transaction = new NHibernateTransaction(connection))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.Unspecified);
            transaction.GetFieldValue<bool>("_disposed").Should().BeFalse();
            transaction.GetFieldValue<bool>("_wasCommitted").Should().BeFalse();
            transaction.GetFieldValue<bool>("_wasRolledBack").Should().BeFalse();
        }

        using (var transaction = new NHibernateTransaction(connection, IsolationLevel.ReadCommitted))
        {
            transaction.IsolationLevel.Should().Be(IsolationLevel.ReadCommitted);
            transaction.GetFieldValue<bool>("_disposed").Should().BeFalse();
            transaction.GetFieldValue<bool>("_wasCommitted").Should().BeFalse();
            transaction.GetFieldValue<bool>("_wasRolledBack").Should().BeFalse();
        }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
        using var connection = SessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Dispose();
        transaction.GetFieldValue<bool>("_disposed").Should().BeTrue();
        transaction.GetFieldValue<bool>("_wasCommitted").Should().BeFalse();
        transaction.GetFieldValue<bool>("_wasRolledBack").Should().BeFalse();
        AssertionExtensions.Should(() => transaction.Dispose()).ThrowExactly<ObjectDisposedException>();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
        using var connection = SessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Commit().Should().BeSameAs(transaction);
        transaction.GetFieldValue<bool>("_disposed").Should().BeFalse();
        transaction.GetFieldValue<bool>("_wasCommitted").Should().BeTrue();
        transaction.GetFieldValue<bool>("_wasRolledBack").Should().BeFalse();
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateTransaction.Rollback()"/> method.</para>
    /// </summary>
    [Fact]
    public void Rollback_Method()
    {
        using var connection = SessionFactory.OpenSession();

        var transaction = new NHibernateTransaction(connection);
        transaction.Rollback().Should().BeSameAs(transaction);
        transaction.GetFieldValue<bool>("_disposed").Should().BeFalse();
        transaction.GetFieldValue<bool>("_wasCommitted").Should().BeFalse();
        transaction.GetFieldValue<bool>("_wasRolledBack").Should().BeTrue();
    }
}
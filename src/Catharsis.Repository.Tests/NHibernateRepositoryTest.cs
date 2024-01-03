using System.Data;
using NHibernate;
using NHibernate.Cfg;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;
using Catharsis.Commons;
using Catharsis.Extensions;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="NHibernateRepository{TEntity}"/>.</para>
/// </summary>
public sealed class NHibernateRepositoryTest : UnitTest
{
  private readonly Configuration configuration = Bootstrapper.NHibernate();

  /// <summary>
  ///   <para>Performs testing of class constructor(s).</para>
  /// </summary>
  /// <seealso cref="NHibernateRepository{TEntity}(ISession)"/>
  /// <seealso cref="NHibernateRepository{TEntity}(ISessionFactory)"/>
  /// <seealso cref="NHibernateRepository{TEntity}(Configuration)"/>
  [Fact]
  public void Constructors()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new NHibernateRepository<TestEntity>((ISessionFactory) null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => new NHibernateRepository<TestEntity>((Configuration) null)).ThrowExactly<ArgumentNullException>();
    }

    using var sessionFactory = configuration.BuildSessionFactory();

    var session = sessionFactory.OpenSession();
    using (var repository = new NHibernateRepository<TestEntity>(session))
    {
      session.SessionFactory.Should().BeSameAs(sessionFactory);
      session.FlushMode.Should().Be(FlushMode.Never);
      repository.GetFieldValue<bool>("ownsSession").Should().BeFalse();
    }
    session.Dispose();

    using (var repository = new NHibernateRepository<TestEntity>(sessionFactory))
    {
      session = repository.Session;
      session.SessionFactory.Should().BeSameAs(sessionFactory);
      session.FlushMode.Should().Be(FlushMode.Never);
      repository.GetFieldValue<bool>("ownsSession").Should().BeTrue();
    }

    using (var repository = new NHibernateRepository<TestEntity>(configuration))
    {
      session = repository.Session;
      session.SessionFactory.Should().NotBeSameAs(sessionFactory);
      session.FlushMode.Should().Be(FlushMode.Never);
      repository.GetFieldValue<bool>("ownsSession").Should().BeTrue();
    }
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Commit()"/> method.</para>
  /// </summary>
  [Fact]
  public void Commit_Method()
  {
    var entity = new TestEntity();

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Should().BeEmpty();

    repository.Persist(entity).Commit().Should().BeSameAs(repository);
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Delete(entity);
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Commit();
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Delete(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Delete_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new NHibernateRepository<TestEntity>(configuration).Delete(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity();

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Transaction(() => repository.Delete(entity).Should().BeSameAs(repository));
      
    repository.Transaction(() => repository.Persist(entity));
    repository.Transaction(() => repository.Delete(new TestEntity()));
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);
      
    repository.Transaction(() => repository.Delete(entity));
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.DeleteAll()"/> method.</para>
  /// </summary>
  [Fact]
  public void DeleteAll_Method()
  {
    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Transaction(() => repository.DeleteAll().Should().BeSameAs(repository));
    repository.Should().BeEmpty();
      
    repository.Transaction(() => repository.Persist(new TestEntity()).Persist(new TestEntity()));
    repository.Should().HaveCount(2);

    repository.Transaction(() => repository.DeleteAll());
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Dispose()"/> method.</para>
  /// </summary>
  [Fact]
  public void Dispose_Method()
  {
    var entity = new TestEntity();

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Persist(entity).Dispose();
    AssertionExtensions.Should(() => repository.Single()).ThrowExactly<ObjectDisposedException>();

    repository.Dispose();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.GetEnumerator()"/> method.</para>
  /// </summary>
  [Fact]
  public void GetEnumerator_Method()
  {
    var entity = new TestEntity();

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.GetEnumerator().MoveNext().Should().BeFalse();

    repository.Transaction(() => repository.Persist(entity));
    using var enumerator = repository.GetEnumerator();
    enumerator.MoveNext().Should().BeTrue();
    enumerator.MoveNext().Should().BeFalse();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Persist(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Persist_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new NHibernateRepository<TestEntity>(configuration).Persist(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Should().BeEmpty();

    repository.Transaction(() => repository.Persist(entity).Should().BeSameAs(repository));
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("first");

    entity.Name = "second";
    repository.Transaction(() => repository.Persist(entity));
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("second");
    repository.Should().ContainSingle(entity => entity.Name == "second").Which.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Refresh(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Refresh_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new NHibernateRepository<TestEntity>(configuration).Refresh(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Refresh(entity).Should().BeSameAs(repository);
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("first");

    repository.Transaction(() => repository.Persist(entity));
    var originalId = entity.Id;
    var originalName = entity.Name;

    entity.Name = "second";
    repository.Refresh(entity);
    entity.Id.Should().Be(originalId);
    entity.Name.Should().Be(originalName);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Transaction(IsolationLevel?)"/> method.</para>
  /// </summary>
  [Fact]
  public async void Transaction_Method()
  {
    var entity = new TestEntity();

    using var repository = new NHibernateRepository<TestEntity>(configuration);

    using (repository.Transaction())
    {
      repository.Persist(new TestEntity());
    }
    repository.Should().BeEmpty();

    using (var transaction = repository.Transaction())
    {
      repository.Persist(new TestEntity());
      transaction.Rollback();
    }
    repository.Should().BeEmpty();

    await AssertionExtensions.Should(() =>
    {
      using (repository.Transaction())
      {
        repository.Persist(new TestEntity());

        throw new Exception();
      }
    }).ThrowExactlyAsync<Exception>();

    repository.Should().BeEmpty();

    using (var transaction = repository.Transaction())
    {
      repository.Persist(entity);
      transaction.Commit();
    }
    repository.Should().ContainSingle();


    using (repository.Transaction())
    {
      repository.Delete(entity);
    }
    repository.Should().ContainSingle();

    using (var transaction = repository.Transaction())
    {
      repository.Delete(entity);
      transaction.Rollback();
    }
    repository.Should().ContainSingle();

    await AssertionExtensions.Should(() =>
    {
      using (repository.Transaction())
      {
        repository.Delete(entity);
        throw new Exception();
      }
    }).ThrowExactlyAsync<Exception>();
    repository.Should().ContainSingle();

    using (var transaction = repository.Transaction())
    {
      repository.Delete(entity);
      transaction.Commit();
    }
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Expression"/> property.</para>
  /// </summary>
  [Fact]
  public void Expression_Property()
  {
    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Session.Query<TestEntity>().Expression.ToString().Should().Be(repository.Expression.ToString());
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.ElementType"/> property.</para>
  /// </summary>
  [Fact]
  public void ElementType_Property()
  {
    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Session.Query<TestEntity>().ElementType.Should().Be(repository.ElementType);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="NHibernateRepository{TEntity}.Provider"/> property.</para>
  /// </summary>
  [Fact]
  public void Provider_Property()
  {
    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.Session.Query<TestEntity>().Provider.ToString().Should().Be(repository.Provider.ToString());
  }

  public void Dispose()
  {
    using var repository = new NHibernateRepository<TestEntity>(configuration);

    repository.DeleteAll().Commit();
  }
}
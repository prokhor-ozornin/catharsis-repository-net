using System.Data;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="EFCodeFirstRepository{TEntity}"/>.</para>
/// </summary>
public sealed class EFCodeFirstRepositoryTest : IDisposable
{
  /// <summary>
  ///   <para>Performs testing of class constructor(s).</para>
  /// </summary>
  /// <seealso cref="EFCodeFirstRepository{TEntity}(DbContext)"/>
  [Fact]
  public void Constructors()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFCodeFirstRepository<TestEntity>(null)).ThrowExactly<ArgumentNullException>();
    }

    var dbContext = new TestContext();
    using var repository = new EFCodeFirstRepository<TestEntity>(dbContext);

    repository.DbContext.Should().BeSameAs(dbContext);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Commit()"/> method.</para>
  /// </summary>
  [Fact]
  public void Commit_Method()
  {
    var entity = new TestEntity();

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.Should().BeEmpty();

    repository.Persist(entity).Commit().Should().BeSameAs(repository);
    repository.Single().Should().BeSameAs(entity);

    repository.Delete(entity);
    repository.Single().Should().BeSameAs(entity);

    repository.Commit();
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Delete(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Delete_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Delete(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity();

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    AssertionExtensions.Should(() => repository.Delete(entity)).ThrowExactly<InvalidOperationException>();
    repository.Persist(entity).Delete(entity).Commit().Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.DeleteAll()"/> method.</para>
  /// </summary>
  [Fact]
  public void DeleteAll_Method()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.DeleteAll().Should().BeSameAs(repository);
    repository.Commit().Should().BeEmpty();

    repository.Persist(new TestEntity()).Persist(new TestEntity());
    repository.Commit().Should().HaveCount(2);

    repository.DeleteAll();
    repository.Commit().Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Dispose()"/> method.</para>
  /// </summary>
  [Fact]
  public void Dispose_Method()
  {
    var entity = new TestEntity();

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.Persist(entity).Dispose();
    AssertionExtensions.Should(() => repository.Single()).ThrowExactly<InvalidOperationException>();

    repository.Dispose();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.GetEnumerator()"/> method.</para>
  /// </summary>
  [Fact]
  public void GetEnumerator_Method()
  {
    var entity = new TestEntity();

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.GetEnumerator().MoveNext().Should().BeFalse();

    repository.Persist(entity).Commit();
    using var enumerator = repository.GetEnumerator();
    enumerator.MoveNext().Should().BeTrue();
    enumerator.MoveNext().Should().BeFalse();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Persist(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Persist_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Persist(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());
    
    repository.Should().BeEmpty();

    repository.Persist(entity).Should().BeSameAs(repository);
    repository.Commit();
    
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("first");

    entity.Name = "second";
    repository.Commit();
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Refresh(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Refresh_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Refresh(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    AssertionExtensions.Should(() => repository.Refresh(entity)).ThrowExactly<InvalidOperationException>();

    repository.Persist(entity).Commit();
    var originalId = entity.Id;
    var originalName = entity.Name;

    entity.Name = "second";
    repository.Refresh(entity).Should().BeSameAs(repository);
    entity.Id.Should().Be(originalId);
    entity.Name.Should().Be(originalName);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Transaction(IsolationLevel?)"/> method.</para>
  /// </summary>
  [Fact]
  public void Transaction_Method()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.Transaction().Should().NotBeNull();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Expression"/> property.</para>
  /// </summary>
  [Fact]
  public void Expression_Property()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.DbContext.Set<TestEntity>().AsQueryable().Expression.ToString().Should().Be(repository.Expression.ToString());
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.ElementType"/> property.</para>
  /// </summary>
  [Fact]
  public void ElementType_Property()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.DbContext.Set<TestEntity>().AsQueryable().ElementType.Should().BeSameAs(repository.ElementType);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFCodeFirstRepository{TEntity}.Provider"/> property.</para>
  /// </summary>
  [Fact]
  public void Provider_Property()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.DbContext.Set<TestEntity>().AsQueryable().Provider.ToString().Should().Be(repository.Provider.ToString());
  }

  public void Dispose()
  {
    using var repository = new EFCodeFirstRepository<TestEntity>(new TestContext());

    repository.DeleteAll().Commit();
  }
}
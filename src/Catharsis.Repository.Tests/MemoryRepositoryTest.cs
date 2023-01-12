using System.Data;
using Catharsis.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="MemoryRepository{TEntity}"/>.</para>
/// </summary>
public sealed class MemoryRepositoryTest : IDisposable
{
  /// <summary>
  ///   <para>Performs testing of class constructor(s).</para>
  /// </summary>
  /// <seealso cref="MemoryRepository{TEntity}()"/>
  [Fact]
  public void Constructors()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Commit()"/> method.</para>
  /// </summary>
  [Fact]
  public void Commit_Method()
  {
    var entity = new TestEntity();

    using var repository = new MemoryRepository<TestEntity>();

    repository.Should().BeEmpty();

    repository.Persist(entity).Commit().Should().BeSameAs(repository);
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Delete(entity);
    repository.Should().BeEmpty();

    repository.Commit();
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Delete(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Delete_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new MemoryRepository<TestEntity>().Delete(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity();

    using var repository = new MemoryRepository<TestEntity>();

    repository.Transaction(() => repository.Delete(entity).Should().BeSameAs(repository));
      
    repository.Transaction(() => repository.Persist(entity));
    repository.Transaction(() => repository.Delete(new TestEntity()));
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);
      
    repository.Transaction(() => repository.Delete(entity));
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.DeleteAll()"/> method.</para>
  /// </summary>
  [Fact]
  public void DeleteAll_Method()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.Transaction(() => repository.DeleteAll().Should().BeSameAs(repository));
    repository.Should().BeEmpty();
      
    repository.Transaction(() => repository.Persist(new TestEntity()).Persist(new TestEntity()));
    repository.Should().HaveCount(2);

    repository.Transaction(() => repository.DeleteAll());
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Dispose()"/> method.</para>
  /// </summary>
  [Fact]
  public void Dispose_Method()
  {
    var entity = new TestEntity();

    using var repository = new MemoryRepository<TestEntity>();

    repository.Persist(entity).Dispose();
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Delete(entity).Dispose();
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.GetEnumerator()"/> method.</para>
  /// </summary>
  [Fact]
  public void GetEnumerator_Method()
  {
    var entity = new TestEntity();

    using var repository = new MemoryRepository<TestEntity>();

    repository.GetEnumerator().MoveNext().Should().BeFalse();

    repository.Transaction(() => repository.Persist(entity));
    using var enumerator = repository.GetEnumerator();
    enumerator.MoveNext().Should().BeTrue();
    enumerator.MoveNext().Should().BeFalse();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Persist(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Persist_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new MemoryRepository<TestEntity>().Persist(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new MemoryRepository<TestEntity>();

    repository.Should().BeEmpty();
      
    repository.Transaction(() => repository.Persist(entity).Should().BeSameAs(repository));
    entity = repository.Single();
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("first");

    entity.Name = "second";
    repository.Transaction(() => repository.Persist(entity));
    entity = repository.Single();
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("second");
    repository.Should().ContainSingle(entity => entity.Name == "second");
    repository.Should().ContainSingle(entity => entity.Name == "second").Which.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Refresh(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Refresh_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new MemoryRepository<TestEntity>().Refresh(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new TestEntity { Name = "first" };

    using var repository = new MemoryRepository<TestEntity>();

    repository.Refresh(entity).Should().BeSameAs(repository);
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("first");

    repository.Transaction(() => repository.Persist(entity));

    entity.Name = "second";
    repository.Refresh(entity);
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Transaction(IsolationLevel?)"/> method.</para>
  /// </summary>
  [Fact]
  public void Transaction_Method()
  {
    var entity = new TestEntity();

    using var repository = new MemoryRepository<TestEntity>();

    using (repository.Transaction())
    {
      repository.Persist(entity);
    }
    repository.Should().ContainSingle();

    using (var transaction = repository.Transaction())
    {
      repository.Persist(entity);
      transaction.Rollback();
    }
    repository.Should().ContainSingle();

    AssertionExtensions.Should(() =>
    {
      using (repository.Transaction())
      {
        repository.Persist(entity);
        throw new Exception();
      }
    });
    repository.Should().ContainSingle();
      
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
    repository.Should().BeEmpty();

    using (var transaction = repository.Transaction())
    {
      repository.Delete(entity);
      transaction.Rollback();
    }
    repository.Should().BeEmpty();

    AssertionExtensions.Should(() =>
    {
      using (repository.Transaction())
      {
        repository.Delete(entity);
        throw new Exception();
      }
    });
    repository.Should().BeEmpty();

    using (var transaction = repository.Transaction())
    {
      repository.Delete(entity);
      transaction.Commit();
    }
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Expression"/> property.</para>
  /// </summary>
  [Fact]
  public void Expression_Property()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.Field("entities").To<IEnumerable<TestEntity>>().AsQueryable().Expression.ToString().Should().Be(repository.Expression.ToString());
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.ElementType"/> property.</para>
  /// </summary>
  [Fact]
  public void ElementType_Property()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.Field("entities").To<IEnumerable<TestEntity>>().AsQueryable().ElementType.Should().Be(repository.ElementType);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="MemoryRepository{TEntity}.Provider"/> property.</para>
  /// </summary>
  [Fact]
  public void Provider_Property()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.Field("entities").To<IEnumerable<TestEntity>>().AsQueryable().Provider.ToString().Should().Be(repository.Provider.ToString());
  }

  public void Dispose()
  {
    using var repository = new MemoryRepository<TestEntity>();

    repository.DeleteAll().Commit();
  }
}
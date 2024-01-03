using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using Catharsis.Commons;
using Catharsis.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="EFModelRepository{TEntity}"/>.</para>
/// </summary>
public sealed class EFModelRepositoryTest : UnitTest
{
  private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLServer.EF"].ConnectionString;

  /// <summary>
  ///   <para>Performs testing of class constructor(s).</para>
  /// </summary>
  /// <seealso cref="EFModelRepository{TEntity}(ObjectContext)"/>
  /// <seealso cref="EFModelRepository{TEntity}(string)"/>
  [Fact]
  public void Constructors()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>((ObjectContext) null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>((string) null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>(string.Empty)).ThrowExactly<ArgumentException>();
    }

    var objectContext = new ObjectContext(connectionString);
    using (var repository = new EFModelRepository<EFModelEntity>(objectContext))
    {
      repository.ObjectContext.Should().BeSameAs(objectContext);
      repository.GetFieldValue< ObjectSet<EFModelEntity>>("objectSet").Context.Should().BeSameAs(repository.GetFieldValue<ObjectContext>("objectContext"));
      repository.GetFieldValue<bool>("ownsContext").Should().BeFalse();
    }

    using (var repository = new EFModelRepository<EFModelEntity>(connectionString))
    {
      repository.ObjectContext.Should().NotBeSameAs(objectContext);
      repository.ObjectContext.Connection.ConnectionString.Should().Be(connectionString);
      repository.GetFieldValue<ObjectSet<EFModelEntity>>("objectSet").Context.Should().BeSameAs(repository.GetFieldValue<ObjectContext>("objectContext"));
      repository.GetFieldValue<bool>("ownsContext").Should().BeTrue();
    }
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Commit()"/> method.</para>
  /// </summary>
  [Fact]
  public void Commit_Method()
  {
    var entity = new EFModelEntity();

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.Should().BeEmpty();

    repository.Persist(entity).Commit().Should().BeSameAs(repository);
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Delete(entity);
    repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

    repository.Commit();
    repository.Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Delete(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Delete_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>(connectionString).Delete(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new EFModelEntity();

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    AssertionExtensions.Should(() => repository.Delete(entity)).ThrowExactly<InvalidOperationException>();
    repository.Persist(entity).Delete(entity).Commit().Should().NotBeNullOrEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.DeleteAll()"/> method.</para>
  /// </summary>
  [Fact]
  public void DeleteAll_Method()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.DeleteAll().Should().BeSameAs(repository);
    repository.Commit().Should().BeEmpty();

    repository.Persist(new EFModelEntity()).Persist(new EFModelEntity());
    repository.Commit().Should().HaveCount(2);

    repository.DeleteAll();
    repository.Commit().Should().BeEmpty();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Dispose()"/> method.</para>
  /// </summary>
  [Fact]
  public void Dispose_Method()
  {
    var entity = new EFModelEntity();

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.Persist(entity).Dispose();
    AssertionExtensions.Should(() => repository.Single()).ThrowExactly<ObjectDisposedException>();

    repository.Dispose();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.GetEnumerator()"/> method.</para>
  /// </summary>
  [Fact]
  public void GetEnumerator_Method()
  {
    var entity = new EFModelEntity();

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.GetEnumerator().MoveNext().Should().BeFalse();

    repository.Persist(entity).Commit();
    using var enumerator = repository.GetEnumerator();
    enumerator.MoveNext().Should().BeTrue();
    enumerator.MoveNext().Should().BeFalse();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Persist(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Persist_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>(connectionString).Persist(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new EFModelEntity { Name = "first" };

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

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
    repository.Should().ContainSingle(entity => entity.Name == "second").Which.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Refresh(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Refresh_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => new EFModelRepository<EFModelEntity>(connectionString).Refresh(null)).ThrowExactly<ArgumentNullException>();
    }

    var entity = new EFModelEntity { Name = "first" };

    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

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
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Transaction(IsolationLevel?)"/> method.</para>
  /// </summary>
  [Fact]
  public void Transaction_Method()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.Transaction().Should().NotBeNull();
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Expression"/> property.</para>
  /// </summary>
  [Fact]
  public void Expression_Property()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.ObjectContext.CreateObjectSet<EFModelEntity>().AsQueryable().Expression.ToString().Should().Be(repository.Expression.ToString());
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.ElementType"/> property.</para>
  /// </summary>
  [Fact]
  public void ElementType_Property()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.ObjectContext.CreateObjectSet<EFModelEntity>().AsQueryable().ElementType.Should().Be(repository.ElementType);
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="EFModelRepository{TEntity}.Provider"/> property.</para>
  /// </summary>
  [Fact]
  public void Provider_Property()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.ObjectContext.CreateObjectSet<EFModelEntity>().AsQueryable().Provider.ToString().Should().Be(repository.Provider.ToString());
  }

  public void Dispose()
  {
    using var repository = new EFModelRepository<EFModelEntity>(connectionString);

    repository.DeleteAll().Commit();
  }
}
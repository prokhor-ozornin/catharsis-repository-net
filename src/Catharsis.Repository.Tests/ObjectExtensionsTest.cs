using CommonServiceLocator;
using FluentAssertions;
using FluentAssertions.Execution;
using Unity.ServiceLocation;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="ObjectExtensions"/>.</para>
/// </summary>
public sealed class ObjectExtensionsTest : IDisposable
{
  private readonly UnityServiceLocator serviceLocator = new UnityServiceLocator(Bootstrapper.Unity());

  /// <summary>
  ///   <para>Performs testing of <see cref="ObjectExtensions.Delete{TEntity}(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Delete_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => ObjectExtensions.Delete<TestEntity>(null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => ServiceLocator.Current.GetInstance<IRepository<TestEntity>>()).ThrowExactly<InvalidOperationException>();
    }

    ServiceLocator.SetLocatorProvider(() => serviceLocator);

    var entity = new TestEntity();
    using (var repository = ServiceLocator.Current.GetInstance<IRepository<TestEntity>>())
    {
      repository.Transaction(() => entity.Delete().Should().BeSameAs(entity));
      repository.Transaction(() => entity.Persist());
      repository.Transaction(() => new TestEntity().Delete());
      repository.Should().ContainSingle().Which.Should().BeSameAs(entity);

      repository.Transaction(() => entity.Delete());
      repository.Should().BeEmpty();
    }
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="ObjectExtensions.Persist{TEntity}(TEntity)"/> method.</para>
  /// </summary>
  [Fact]
  public void Persist_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => ObjectExtensions.Persist<TestEntity>(null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => ServiceLocator.Current.GetInstance<IRepository<TestEntity>>()).ThrowExactly<InvalidOperationException>();
    }

    ServiceLocator.SetLocatorProvider(() => serviceLocator);

    var entity = new TestEntity { Name = "first" };
    using var repository = ServiceLocator.Current.GetInstance<IRepository<TestEntity>>();

    repository.Should().BeEmpty();

    repository.Transaction(() => entity.Persist().Should().BeSameAs(entity));
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("first");

    entity.Name = "second";
    repository.Transaction(() => entity.Persist());
    entity = repository.Single();
    entity.Id.Should().NotBe(0);
    entity.Name.Should().Be("second");
  }

  /// <summary>
  ///   <para>Performs testing of <see cref="ObjectExtensions.Refresh{TEntity}(TEntity, bool)"/> method.</para>
  /// </summary>
  [Fact]
  public void Refresh_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => ObjectExtensions.Refresh<TestEntity>(null)).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => ServiceLocator.Current.GetInstance<IRepository<TestEntity>>()).ThrowExactly<InvalidOperationException>();
    }

    ServiceLocator.SetLocatorProvider(() => serviceLocator);

    var entity = new TestEntity { Name = "first" };
    using var repository = ServiceLocator.Current.GetInstance<IRepository<TestEntity>>();

    entity.Refresh().Should().BeSameAs(entity);
    entity.Id.Should().Be(0);
    entity.Name.Should().Be("first");

    repository.Transaction(() => entity.Persist());
    var originalId = entity.Id;
    var originalName = entity.Name;

    entity.Name = "second";
    entity.Refresh();
    entity.Id.Should().Be(originalId);
    entity.Name.Should().Be(originalName);
  }

  public void Dispose()
  {
    ServiceLocator.SetLocatorProvider(null);
  }
}
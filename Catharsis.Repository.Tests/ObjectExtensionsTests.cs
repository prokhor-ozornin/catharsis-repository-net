using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="ObjectExtensions"/>.</para>
  /// </summary>
  public sealed class ObjectExtensionsTests : IDisposable
  {
    private readonly UnityServiceLocator serviceLocator = new UnityServiceLocator(Bootstrapper.Unity());

    /// <summary>
    ///   <para>Performs testing of <see cref="ObjectExtensions.Delete{ENTITY}(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => ObjectExtensions.Delete<MockEntity>(null));
      Assert.Throws<InvalidOperationException>(() => ServiceLocator.Current.GetInstance<IRepository<MockEntity>>());

      ServiceLocator.SetLocatorProvider(() => this.serviceLocator);

      var entity = new MockEntity();
      using (var repository = ServiceLocator.Current.GetInstance<IRepository<MockEntity>>())
      {
        repository.Transaction(() => Assert.True(ReferenceEquals(entity.Delete(), entity)));
        repository.Transaction(() => entity.Persist());
        repository.Transaction(() => new MockEntity().Delete());
        Assert.True(ReferenceEquals(repository.Single(), entity));

        repository.Transaction(() => entity.Delete());
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="ObjectExtensions.Persist{ENTITY}(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => ObjectExtensions.Persist<MockEntity>(null));
      Assert.Throws<InvalidOperationException>(() => ServiceLocator.Current.GetInstance<IRepository<MockEntity>>());

      ServiceLocator.SetLocatorProvider(() => this.serviceLocator);

      var entity = new MockEntity { Name = "first" };
      using (var repository = ServiceLocator.Current.GetInstance<IRepository<MockEntity>>())
      {
        Assert.False(repository.Any());

        repository.Transaction(() => Assert.True(ReferenceEquals(entity.Persist(), entity)));
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("first", entity.Name);

        entity.Name = "second";
        repository.Transaction(() => entity.Persist());
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("second", entity.Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="ObjectExtensions.Refresh{ENTITY}(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => ObjectExtensions.Refresh<MockEntity>(null));
      Assert.Throws<InvalidOperationException>(() => ServiceLocator.Current.GetInstance<IRepository<MockEntity>>());

      ServiceLocator.SetLocatorProvider(() => this.serviceLocator);

      var entity = new MockEntity { Name = "first" };
      using (var repository = ServiceLocator.Current.GetInstance<IRepository<MockEntity>>())
      {
        Assert.True(ReferenceEquals(entity.Refresh(), entity));
        Assert.Equal(0, entity.Id);
        Assert.Equal("first", entity.Name);

        repository.Transaction(() => entity.Persist());
        var originalId = entity.Id;
        var originalName = entity.Name;

        entity.Name = "second";
        entity.Refresh();
        Assert.Equal(originalId, entity.Id);
        Assert.Equal(originalName, entity.Name);
      }
    }

    public void Dispose()
    {
      ServiceLocator.SetLocatorProvider(null);
    }
  }
}
using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="Repository"/>.</para>
  /// </summary>
  public sealed class RepositoryTests : IDisposable
  {
    private readonly UnityServiceLocator serviceLocator = new UnityServiceLocator(Bootstrapper.Unity());

    /// <summary>
    ///   <para>Performs testing of <see cref="Repository.For{ENTITY}()"/> method.</para>
    /// </summary>
    [Fact]
    public void For_Method()
    {
      Assert.Throws<InvalidOperationException>(() => Repository.For<TestEntity>());

      ServiceLocator.SetLocatorProvider(() => this.serviceLocator);

      Assert.NotNull(Repository.For<TestEntity>());
      Assert.True(ReferenceEquals(Repository.For<TestEntity>(), Repository.For<TestEntity>()));
    }

    public void Dispose()
    {
      ServiceLocator.SetLocatorProvider(null);
    }
  }
}
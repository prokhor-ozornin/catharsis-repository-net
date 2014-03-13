using System;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity.ServiceLocatorAdapter;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="Repository"/>.</para>
  /// </summary>
  public sealed class RepositoryTests
  {
    private readonly UnityServiceLocator serviceLocator = new UnityServiceLocator(Bootstrapper.Unity());

    /// <summary>
    ///   <para>Performs testing of <see cref="Repository.For{ENTITY}()"/> method.</para>
    /// </summary>
    [Fact]
    public void For_Method()
    {
      ServiceLocator.SetLocatorProvider(null);

      Assert.Throws<InvalidOperationException>(() => Repository.For<MockEntity>());

      ServiceLocator.SetLocatorProvider(() => this.serviceLocator);

      Assert.NotNull(Repository.For<MockEntity>());
      Assert.True(ReferenceEquals(Repository.For<MockEntity>(), Repository.For<MockEntity>()));
    }
  }
}
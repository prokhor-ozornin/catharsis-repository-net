using Catharsis.Commons;
using CommonServiceLocator;
using FluentAssertions;
using FluentAssertions.Execution;
using Unity.ServiceLocation;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="Repository"/>.</para>
/// </summary>
public sealed class RepositoryTest : UnitTest
{
  private readonly UnityServiceLocator serviceLocator = new UnityServiceLocator(Bootstrapper.Unity());

  /// <summary>
  ///   <para>Performs testing of <see cref="Repository.For{TEntity}()"/> method.</para>
  /// </summary>
  [Fact]
  public void For_Method()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => Repository.For<TestEntity>()).ThrowExactly<InvalidOperationException>();
    }

    ServiceLocator.SetLocatorProvider(() => serviceLocator);

    Repository.For<TestEntity>().Should().NotBeNull();
    Repository.For<TestEntity>().Should().BeSameAs(Repository.For<TestEntity>());
  }

  public void Dispose()
  {
    ServiceLocator.SetLocatorProvider(null);
  }
}
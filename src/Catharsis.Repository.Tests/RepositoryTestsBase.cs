using Catharsis.Commons;
using Unity;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para></para>
/// </summary>
public abstract class RepositoryTestsBase : UnitTest
{
  /// <summary>
  ///   <para></para>
  /// </summary>
  protected RepositoryTestsBase()
  {
    //ServiceLocator.SetLocatorProvider(() => locator);
  }

  /// <summary>
  ///   <para></para>
  /// </summary>
  protected IUnityContainer Container => /*this.container;*/null;

  /// <summary>
  ///   <para></para>
  /// </summary>
  public override void Dispose()
  {
    base.Dispose();

    //this.ioc.Dispose();
  }
}
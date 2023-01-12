using Unity;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para></para>
/// </summary>
public abstract class RepositoryTestsBase
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
  public void Dispose()
  {
    //this.ioc.Dispose();
  }
}
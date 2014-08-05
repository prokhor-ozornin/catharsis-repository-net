using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Catharsis.Repository
{
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
    protected IUnityContainer Container
    {
      get { return /*this.container;*/null; }
    }

    /// <summary>
    ///   <para></para>
    /// </summary>
    public void Dispose()
    {
      //this.ioc.Dispose();
    }
  }
}
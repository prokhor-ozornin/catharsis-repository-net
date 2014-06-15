using Microsoft.Practices.ServiceLocation;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Registered repositories access manager.</para>
  /// </summary>
  public static class Repository
  {
    /// <summary>
    ///   <para>Retrieves instance of registered implementation of repository from underlying IOC container, using <see cref="IServiceLocator"/> approach.</para>
    ///   <para>Implementation of <see cref="IRepository{ENTITY}"/> interface must be registered prior to calling this method.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <returns>Persistence repository instance that works with business entity of <typeparamref name="ENTITY"/> type.</returns>
    public static IRepository<ENTITY> For<ENTITY>() where ENTITY : class
    {
      return ServiceLocator.Current.GetInstance<IRepository<ENTITY>>();
    }
  }
}
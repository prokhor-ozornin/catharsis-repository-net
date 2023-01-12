using CommonServiceLocator;

namespace Catharsis.Repository;

/// <summary>
///   <para>Registered repositories access manager.</para>
/// </summary>
public static class Repository
{
  /// <summary>
  ///   <para>Retrieves instance of registered implementation of repository from underlying IOC container, using <see cref="IServiceLocator"/> approach.</para>
  ///   <para>Implementation of <see cref="IRepository{TEntity}"/> interface must be registered prior to calling this method.</para>
  /// </summary>
  /// <typeparam name="TEntity">Type of business entity.</typeparam>
  /// <returns>Persistence repository instance that works with business entity of <typeparamref name="TEntity"/> type.</returns>
  public static IRepository<TEntity> For<TEntity>() where TEntity : class => ServiceLocator.Current.GetInstance<IRepository<TEntity>>();
}
using CommonServiceLocator;

namespace Catharsis.Repository;

/// <summary>
///   <para></para>
/// </summary>
public static class ObjectExtensions
{
  /// <param name="entity">Entity to be deleted.</param>
  /// <typeparam name="TEntity">Type of business entity.</typeparam>
  extension<TEntity>(TEntity entity) where TEntity : class
  {
    /// <summary>
    ///   <para>Deletes specified business entity from registered repository.</para>
    ///   <para><see cref="IRepository{TEntity}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <param name="commit"><c>true</c> to perform immediate commit (<see cref="IRepository{TEntity}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <seealso cref="IRepository{TEntity}.Delete(TEntity)"/>
    public TEntity Delete(bool commit = true)
    {
      var repository = Repository.For<TEntity>();

      repository.Delete(entity);

      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }

    /// <summary>
    ///   <para>Persists state of specified business entity in registered repository.</para>
    ///   <para><see cref="IRepository{TEntity}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <param name="commit"><c>true</c> to perform immediate commit (<see cref="IRepository{TEntity}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <seealso cref="IRepository{TEntity}.Persist(TEntity)"/>
    public TEntity Persist(bool commit = true)
    {
      var repository = Repository.For<TEntity>();

      repository.Persist(entity);

      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }

    /// <summary>
    ///   <para>Restores original state of business entity from registered repository.</para>
    ///   <para><see cref="IRepository{TEntity}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <param name="commit"><c>true</c> to perform immediate commit (<see cref="IRepository{TEntity}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <seealso cref="IRepository{TEntity}.Refresh(TEntity)"/>
    public TEntity Refresh(bool commit = true)
    {
      var repository = Repository.For<TEntity>();

      repository.Refresh(entity);

      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }
  }
}
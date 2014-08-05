using System;
using Catharsis.Commons;
using Microsoft.Practices.ServiceLocation;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para></para>
  /// </summary>
  public static class ObjectExtensions
  {
    /// <summary>
    ///   <para>Deletes specified business entity from registered repository.</para>
    ///   <para><see cref="IRepository{ENTITY}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <param name="entity">Entity to be deleted.</param>
    /// <param name="commit"><c>true</c> to perform immedial commit (<see cref="IRepository{ENTITY}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IRepository{ENTITY}.Delete(ENTITY)"/>
    public static ENTITY Delete<ENTITY>(this ENTITY entity, bool commit = true) where ENTITY : class
    {
      Assertion.NotNull(entity);

      var repository = Repository.For<ENTITY>();
      repository.Delete(entity);
      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }

    /// <summary>
    ///   <para>Persists state of specified business entity in registered repository.</para>
    ///   <para><see cref="IRepository{ENTITY}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <param name="entity">Entity to be persisted (added/updated).</param>
    /// <param name="commit"><c>true</c> to perform immedial commit (<see cref="IRepository{ENTITY}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IRepository{ENTITY}.Persist(ENTITY)"/>
    public static ENTITY Persist<ENTITY>(this ENTITY entity, bool commit = true) where ENTITY : class
    {
      Assertion.NotNull(entity);

      var repository = Repository.For<ENTITY>();
      repository.Persist(entity);
      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }

    /// <summary>
    ///   <para>Restores original state of business entity from registered repository.</para>
    ///   <para><see cref="IRepository{ENTITY}"/> implementation must be registered in IOC container and be available for <see cref="IServiceLocator"/> prior to this method's call.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <param name="entity">Entity whose state is to be restored.</param>
    /// <param name="commit"><c>true</c> to perform immedial commit (<see cref="IRepository{ENTITY}.Commit()"/>) and persist changes, <c>false</c> to leave this task to external code.</param>
    /// <returns>Back reference to the current entity.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="entity"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IRepository{ENTITY}.Refresh(ENTITY)"/>
    public static ENTITY Refresh<ENTITY>(this ENTITY entity, bool commit = true) where ENTITY : class
    {
      Assertion.NotNull(entity);

      var repository = Repository.For<ENTITY>();
      repository.Refresh(entity);
      if (commit)
      {
        repository.Commit();
      }

      return entity;
    }
  }
}
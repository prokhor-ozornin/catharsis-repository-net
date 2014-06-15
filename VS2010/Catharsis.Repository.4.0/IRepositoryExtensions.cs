using System;
using Catharsis.Commons;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Set of extension methods for interface <see cref="IRepository{T}"/>.</para>
  /// </summary>
  /// <seealso cref="IRepository{T}"/>
  public static class IRepositoryExtensions
  {
    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    ///   <para>If no exceptions occurs during execution of <paramref name="action"/>, transaction is succesfully committed, and all changes made inside it's body are persisted to the underlying data storage; otherwise, transaction is rolled back and all changes are discarded.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <param name="repository">Current repository instance that will spawn a transaction.</param>
    /// <param name="action">Delegate that represents a block of code that is the body of transaction.</param>
    /// <returns>Back reference to the current persistent repository.</returns>
    /// <exception cref="ArgumentNullException">If either <paramref name="repository"/> or <paramref name="action"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IRepository{ENTITY}.Transaction"/>
    /// <seealso cref="Transaction{ENTITY}(IRepository{ENTITY}, Action{IRepository{ENTITY}})"/>
    public static IRepository<ENTITY> Transaction<ENTITY>(this IRepository<ENTITY> repository, Action action) where ENTITY : class
    {
      Assertion.NotNull(repository);
      Assertion.NotNull(action);

      using (var transaction = repository.Transaction())
      {
        action();
        transaction.Commit();
      }

      return repository;
    }

    /// <summary>
    ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
    ///   <para>If no exceptions occurs during execution of <paramref name="action"/>, transaction is succesfully committed, and all changes made inside it's body are persisted to the underlying data storage; otherwise, transaction is rolled back and all changes are discarded.</para>
    /// </summary>
    /// <typeparam name="ENTITY">Type of business entity.</typeparam>
    /// <param name="repository">Current repository instance that will spawn a transaction.</param>
    /// <param name="action">Delegate that represents a block of code that is the body of transaction.</param>
    /// <returns>Back reference to the current persistent repository.</returns>
    /// <exception cref="ArgumentNullException">If either <paramref name="repository"/> or <paramref name="action"/> is a <c>null</c> reference.</exception>
    /// <seealso cref="IRepository{ENTITY}.Transaction"/>
    /// <seealso cref="Transaction{ENTITY}(IRepository{ENTITY}, Action)"/>
    public static IRepository<ENTITY> Transaction<ENTITY>(this IRepository<ENTITY> repository, Action<IRepository<ENTITY>> action) where ENTITY : class
    {
      Assertion.NotNull(repository);
      Assertion.NotNull(action);

      using (var transaction = repository.Transaction())
      {
        action(repository);
        transaction.Commit();
      }

      return repository;
    }
  }
}
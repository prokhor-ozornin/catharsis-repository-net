namespace Catharsis.Repository;

/// <summary>
///   <para>Set of extension methods for interface <see cref="IRepository{T}"/>.</para>
/// </summary>
/// <seealso cref="IRepository{T}"/>
public static class IRepositoryExtensions
{
  /// <summary>
  ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
  ///   <para>If no exceptions occurs during execution of <paramref name="action"/>, transaction is successfully committed, and all changes made inside it's body are persisted to the underlying data storage; otherwise, transaction is rolled back and all changes are discarded.</para>
  /// </summary>
  /// <typeparam name="TEntity">Type of business entity.</typeparam>
  /// <param name="repository">Current repository instance that will spawn a transaction.</param>
  /// <param name="action">Delegate that represents a block of code that is the body of transaction.</param>
  /// <returns>Back reference to the current persistent repository.</returns>
  /// <seealso cref="IRepository{TEntity}.Transaction"/>
  /// <seealso cref="Transaction{TEntity}(IRepository{TEntity}, Action{IRepository{TEntity}})"/>
  public static IRepository<TEntity> Transaction<TEntity>(this IRepository<TEntity> repository, Action action) where TEntity : class
  {
    using var transaction = repository.Transaction();

    action();
    transaction.Commit();

    return repository;
  }

  /// <summary>
  ///   <para>Wraps a set of operations over repository inside an atomic transaction, making it a single unit-of-work block.</para>
  ///   <para>If no exceptions occurs during execution of <paramref name="action"/>, transaction is successfully committed, and all changes made inside it's body are persisted to the underlying data storage; otherwise, transaction is rolled back and all changes are discarded.</para>
  /// </summary>
  /// <typeparam name="TEntity">Type of business entity.</typeparam>
  /// <param name="repository">Current repository instance that will spawn a transaction.</param>
  /// <param name="action">Delegate that represents a block of code that is the body of transaction.</param>
  /// <returns>Back reference to the current persistent repository.</returns>
  /// <seealso cref="IRepository{TEntity}.Transaction"/>
  /// <seealso cref="Transaction{TEntity}(IRepository{TEntity}, Action)"/>
  public static IRepository<TEntity> Transaction<TEntity>(this IRepository<TEntity> repository, Action<IRepository<TEntity>> action) where TEntity : class
  {
    using var transaction = repository.Transaction();

    action(repository);
    transaction.Commit();

    return repository;
  }
}
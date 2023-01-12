using System.Data;

namespace Catharsis.Repository;

/// <summary>
///   <para>Represents atomic unit of work with <see cref="IRepository{TEntity}"/> instance.</para>
/// </summary>
/// <seealso cref="IDbTransaction"/>
public interface ITransaction : IDisposable
{
  /// <summary>
  ///   <para>Accepts all changes, made during the course of transaction.</para>
  /// </summary>
  /// <returns>Back reference to the current transaction.</returns>
  ITransaction Commit();

  /// <summary>
  ///   <para>Discards all changes, made during the course of transaction.</para>
  /// </summary>
  /// <returns>Back reference to the current transaction.</returns>
  ITransaction Rollback();

  /// <summary>
  ///   <para>Currently used transactional isolation level.</para>
  /// </summary>
  IsolationLevel IsolationLevel { get; }
}
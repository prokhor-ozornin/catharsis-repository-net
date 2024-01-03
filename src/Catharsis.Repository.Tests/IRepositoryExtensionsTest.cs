using Catharsis.Commons;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Catharsis.Repository.Tests;

/// <summary>
///   <para>Tests set for class <see cref="IRepositoryExtensions"/>.</para>
/// </summary>
public sealed class IRepositoryExtensionsTest : UnitTest
{
  /// <summary>
  ///   <para>Performs testing of following methods :</para>
  ///   <list type="bullet">
  ///     <item><description><see cref="IRepositoryExtensions.Transaction{TEntity}(IRepository{TEntity}, Action)"/></description></item>
  ///     <item><description><see cref="IRepositoryExtensions.Transaction{TEntity}(IRepository{TEntity}, Action{IRepository{TEntity}})"/></description></item>
  ///   </list>
  /// </summary>
  [Fact]
  public void Transaction_Methods()
  {
    using (new AssertionScope())
    {
      AssertionExtensions.Should(() => IRepositoryExtensions.Transaction<object>(null, () => {})).ThrowExactly<ArgumentNullException>();
      AssertionExtensions.Should(() => new MemoryRepository<TestEntity>().Transaction((Action) null)).ThrowExactly<ArgumentNullException>();
    }

    var counter = 0;
    using (var repository = new TestRepository<TestEntity>())
    {
      repository.Transaction(() => counter++).Should().BeSameAs(repository);
    }
    counter.Should().Be(1);

    counter = 0;
    using (var repository = new TestRepository<TestEntity>())
    {
      repository.Transaction(_ => counter++).Should().BeSameAs(repository);
    }
    counter.Should().Be(1);
  }
}
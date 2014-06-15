using System;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="IRepositoryExtensions"/>.</para>
  /// </summary>
  public sealed class IRepositoryExtensionsTests
  {
    /// <summary>
    ///   <para>Performs testing of following methods :</para>
    ///   <list type="bullet">
    ///     <item><description><see cref="IRepositoryExtensions.Transaction{ENTITY}(IRepository{ENTITY}, Action)"/></description></item>
    ///     <item><description><see cref="IRepositoryExtensions.Transaction{ENTITY}(IRepository{ENTITY}, Action{IRepository{ENTITY}})"/></description></item>
    ///   </list>
    /// </summary>
    [Fact]
    public void Transaction_Methods()
    {
      Assert.Throws<ArgumentNullException>(() => IRepositoryExtensions.Transaction<object>(null, () => { }));
      Assert.Throws<ArgumentNullException>(() => new MemoryRepository<TestEntity>().Transaction((Action) null));

      var counter = 0;
      using (var repository = new TestRepository<TestEntity>())
      {
        Assert.True(ReferenceEquals(repository.Transaction(() => counter++), repository));
      }
      Assert.Equal(1, counter);

      counter = 0;
      using (var repository = new TestRepository<TestEntity>())
      {
        Assert.True(ReferenceEquals(repository.Transaction(x => counter++), repository));
      }
      Assert.Equal(1, counter);
    }
  }
}
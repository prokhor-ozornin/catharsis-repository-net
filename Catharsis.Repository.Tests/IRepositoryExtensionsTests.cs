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
    ///   <para>Performs testing of <see cref="IRepositoryExtensions.Transaction{ENTITY}(IRepository{ENTITY}, Action)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      Assert.Throws<ArgumentNullException>(() => IRepositoryExtensions.Transaction<object>(null, () => { }));
      Assert.Throws<ArgumentNullException>(() => new MemoryRepository<MockEntity>().Transaction((Action) null));

      var counter = 0;
      using (var repository = new MockRepository<MockEntity>())
      {
        Assert.True(ReferenceEquals(repository.Transaction(() => counter++), repository));
      }
      Assert.Equal(1, counter);
    }
  }
}
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="EFCodeFirstRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class EFCodeFirstRepositoryTests : IDisposable
  {
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    /// </summary>
    /// <seealso cref="EFCodeFirstRepository{ENTITY}(DbContext)"/>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new EFCodeFirstRepository<TestEntity>(null));

      var dbContext = new TestContext();
      using (var repository = new EFCodeFirstRepository<TestEntity>(dbContext))
      {
        Assert.True(ReferenceEquals(repository.DbContext, dbContext));
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new TestEntity();

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.False(repository.Any());

        Assert.True(ReferenceEquals(repository.Persist(entity).Commit(), repository));
        Assert.True(ReferenceEquals(repository.Single(), entity));

        repository.Delete(entity);
        Assert.True(ReferenceEquals(repository.Single(), entity));

        repository.Commit();
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Delete(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Delete(null));

      var entity = new TestEntity();

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.Throws<InvalidOperationException>(() => repository.Delete(entity));
        Assert.False(repository.Persist(entity).Delete(entity).Commit().Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.DeleteAll()"/> method.</para>
    /// </summary>
    [Fact]
    public void DeleteAll_Method()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.True(ReferenceEquals(repository.DeleteAll(), repository));
        Assert.False(repository.Commit().Any());

        repository.Persist(new TestEntity()).Persist(new TestEntity());
        Assert.Equal(2, repository.Commit().Count());

        repository.DeleteAll();
        Assert.False(repository.Commit().Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var entity = new TestEntity();

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        repository.Persist(entity).Dispose();
        Assert.Throws<InvalidOperationException>(() => repository.Single());

        repository.Dispose();
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.GetEnumerator()"/> method.</para>
    /// </summary>
    [Fact]
    public void GetEnumerator_Method()
    {
      var entity = new TestEntity();

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.False(repository.GetEnumerator().MoveNext());

        repository.Persist(entity).Commit();
        var enumerator = repository.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Persist(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Persist(null));

      var entity = new TestEntity { Name = "first" };

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.False(repository.Any());

        Assert.True(ReferenceEquals(repository.Persist(entity), repository));
        repository.Commit();
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("first", entity.Name);

        entity.Name = "second";
        repository.Commit();
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("second", entity.Name);
        Assert.Equal(1, repository.Count(x => x.Name == "second"));
        Assert.Equal("second", repository.Single(x => x.Name == "second").Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFCodeFirstRepository<TestEntity>(new TestContext()).Refresh(null));

      var entity = new TestEntity { Name = "first" };

      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.Throws<InvalidOperationException>(() => repository.Refresh(entity));

        repository.Persist(entity).Commit();
        var originalId = entity.Id;
        var originalName = entity.Name;

        entity.Name = "second";
        Assert.True(ReferenceEquals(repository.Refresh(entity), repository));
        Assert.Equal(originalId, entity.Id);
        Assert.Equal(originalName, entity.Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Transaction(IsolationLevel?)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.NotNull(repository.Transaction());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Expression"/> property.</para>
    /// </summary>
    [Fact]
    public void Expression_Property()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.Equal(repository.DbContext.Set<TestEntity>().AsQueryable().Expression.ToString(), repository.Expression.ToString());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    [Fact]
    public void ElementType_Property()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.True(ReferenceEquals(repository.DbContext.Set<TestEntity>().AsQueryable().ElementType, repository.ElementType));
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFCodeFirstRepository{ENTITY}.Provider"/> property.</para>
    /// </summary>
    [Fact]
    public void Provider_Property()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        Assert.Equal(repository.DbContext.Set<TestEntity>().AsQueryable().Provider.ToString(), repository.Provider.ToString());
      }
    }

    public void Dispose()
    {
      using (var repository = new EFCodeFirstRepository<TestEntity>(new TestContext()))
      {
        repository.DeleteAll().Commit();
      }
    }
  }
}
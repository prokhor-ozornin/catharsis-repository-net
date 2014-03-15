using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Catharsis.Commons;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="MemoryRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class MemoryRepositoryTests : IDisposable
  {
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="MemoryRepository{ENTITY}()"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new MockEntity();

      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.False(repository.Any());

        Assert.True(ReferenceEquals(repository.Persist(entity).Commit(), repository));
        Assert.True(ReferenceEquals(repository.Single(), entity));

        repository.Delete(entity);
        Assert.False(repository.Any());

        repository.Commit();
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Delete(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new MemoryRepository<MockEntity>().Delete(null));

      var entity = new MockEntity();

      using (var repository = new MemoryRepository<MockEntity>())
      {
        repository.Transaction(() =>  Assert.True(ReferenceEquals(repository.Delete(entity), repository)));
        
        repository.Transaction(() => repository.Persist(entity));
        repository.Transaction(() => repository.Delete(new MockEntity()));
        Assert.True(ReferenceEquals(repository.Single(), entity));
        
        repository.Transaction(() => repository.Delete(entity));
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.DeleteAll()"/> method.</para>
    /// </summary>
    [Fact]
    public void DeleteAll_Method()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        repository.Transaction(() => Assert.True(ReferenceEquals(repository.DeleteAll(), repository)));
        Assert.False(repository.Any());
        
        repository.Transaction(() => repository.Persist(new MockEntity()).Persist(new MockEntity()));
        Assert.Equal(2, repository.Count());

        repository.Transaction(() => repository.DeleteAll());
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var entity = new MockEntity();

      using (var repository = new MemoryRepository<MockEntity>())
      {
        repository.Persist(entity).Dispose();
        Assert.True(ReferenceEquals(repository.Single(), entity));

        repository.Delete(entity).Dispose();
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.GetEnumerator()"/> method.</para>
    /// </summary>
    [Fact]
    public void GetEnumerator_Method()
    {
      var entity = new MockEntity();

      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.False(repository.GetEnumerator().MoveNext());

        repository.Transaction(() => repository.Persist(entity));
        var enumerator = repository.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Persist(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new MemoryRepository<MockEntity>().Persist(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.False(repository.Any());
        
        repository.Transaction(() => Assert.True(ReferenceEquals(repository.Persist(entity), repository)));
        entity = repository.Single();
        Assert.Equal(0, entity.Id);
        Assert.Equal("first", entity.Name);

        entity.Name = "second";
        repository.Transaction(() => repository.Persist(entity));
        entity = repository.Single();
        Assert.Equal(0, entity.Id);
        Assert.Equal("second", entity.Name);
        Assert.Equal(1, repository.Count(x => x.Name == "second"));
        Assert.Equal("second", repository.Single(x => x.Name == "second").Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new MemoryRepository<MockEntity>().Refresh(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.True(ReferenceEquals(repository.Refresh(entity), repository));
        Assert.Equal(0, entity.Id);
        Assert.Equal("first", entity.Name);

        repository.Transaction(() => repository.Persist(entity));

        entity.Name = "second";
        repository.Refresh(entity);
        Assert.Equal(0, entity.Id);
        Assert.Equal("second", entity.Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Transaction(IsolationLevel?)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      var entity = new MockEntity();

      using (var repository = new MemoryRepository<MockEntity>())
      {
        using (repository.Transaction())
        {
          repository.Persist(entity);
        }
        Assert.Equal(1, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Persist(entity);
          transaction.Rollback();
        }
        Assert.Equal(1, repository.Count());

        try
        {
          using (repository.Transaction())
          {
            repository.Persist(entity);
            throw new Exception();
          }
        }
        catch
        {
          Assert.True(true);
        }
        Assert.Equal(1, repository.Count());
        
        using (var transaction = repository.Transaction())
        {
          repository.Persist(entity);
          transaction.Commit();
        }
        Assert.Equal(1, repository.Count());


        using (repository.Transaction())
        {
          repository.Delete(entity);
        }
        Assert.Equal(0, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Delete(entity);
          transaction.Rollback();
        }
        Assert.Equal(0, repository.Count());

        try
        {
          using (repository.Transaction())
          {
            repository.Delete(entity);
            throw new Exception();
          }
        }
        catch
        {
          Assert.True(true);
        }
        Assert.Equal(0, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Delete(entity);
          transaction.Commit();
        }
        Assert.Equal(0, repository.Count());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Expression"/> property.</para>
    /// </summary>
    [Fact]
    public void Expression_Property()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.Equal(repository.Field("entities").To<IEnumerable<MockEntity>>().AsQueryable().Expression.ToString(), repository.Expression.ToString());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    [Fact]
    public void ElementType_Property()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.True(ReferenceEquals(repository.Field("entities").To<IEnumerable<MockEntity>>().AsQueryable().ElementType, repository.ElementType));
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="MemoryRepository{ENTITY}.Provider"/> property.</para>
    /// </summary>
    [Fact]
    public void Provider_Property()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        Assert.Equal(repository.Field("entities").To<IEnumerable<MockEntity>>().AsQueryable().Provider.ToString(), repository.Provider.ToString());
      }
    }

    public void Dispose()
    {
      using (var repository = new MemoryRepository<MockEntity>())
      {
        repository.DeleteAll().Commit();
      }
    }
  }
}
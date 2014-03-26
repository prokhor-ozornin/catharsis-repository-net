using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using Catharsis.Commons;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="LinqToSqlRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class LinqToSqlRepositoryTests : IDisposable
  {
    private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;

    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="LinqToSqlRepository{ENTITY}(string)"/>
    ///   <seealso cref="LinqToSqlRepository{ENTITY}(IDbConnection)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<TestEntity>((string)null));
      Assert.Throws<ArgumentException>(() => new LinqToSqlRepository<TestEntity>(string.Empty));
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<TestEntity>((IDbConnection)null));

      using (var repository = new LinqToSqlRepository<TestEntity>(this.connectionString))
      {
        Assert.Equal(ConnectionState.Closed, repository.DataContext.Connection.State);
        Assert.True(repository.Field("ownsConnection").To<bool>());
      }

      using (var connection = this.Connection())
      {
        using (var repository = new LinqToSqlRepository<TestEntity>(connection))
        {
          Assert.True(ReferenceEquals(repository.DataContext.Connection, connection));
          Assert.False(repository.Field("ownsConnection").To<bool>());
        }
        Assert.Equal(ConnectionState.Open, connection.State);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new TestEntity();

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
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
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Delete(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<TestEntity>(this.Connection()).Delete(null));

      var entity = new TestEntity();

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.Throws<InvalidOperationException>(() => repository.Delete(entity));
        Assert.False(repository.Persist(entity).Delete(entity).Commit().Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.DeleteAll()"/> method.</para>
    /// </summary>
    [Fact]
    public void DeleteAll_Method()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
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
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var entity = new TestEntity();

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        repository.Persist(entity).Dispose();
        Assert.Throws<ObjectDisposedException>(() => repository.Single());

        repository.Dispose();
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.GetEnumerator()"/> method.</para>
    /// </summary>
    [Fact]
    public void GetEnumerator_Method()
    {
      var entity = new TestEntity();

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.False(repository.GetEnumerator().MoveNext());

        repository.Persist(entity).Commit();
        var enumerator = repository.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Persist(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<TestEntity>(this.Connection()).Persist(null));

      var entity = new TestEntity { Name = "first" };

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
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
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<TestEntity>(this.Connection()).Refresh(null));

      var entity = new TestEntity { Name = "first" };

      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.Throws<ArgumentException>(() => repository.Refresh(entity));

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
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Transaction(IsolationLevel?)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.NotNull(repository.Transaction());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Expression"/> property.</para>
    /// </summary>
    [Fact]
    public void Expression_Property()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.Equal(repository.DataContext.GetTable<TestEntity>().AsQueryable().Expression.ToString(), repository.Expression.ToString());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.ElementType"/> property.</para>
    /// </summary>
    [Fact]
    public void ElementType_Property()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.True(ReferenceEquals(repository.DataContext.GetTable<TestEntity>().AsQueryable().ElementType, repository.ElementType));
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Provider"/> property.</para>
    /// </summary>
    [Fact]
    public void Provider_Property()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        Assert.Equal(repository.DataContext.GetTable<TestEntity>().AsQueryable().Provider.ToString(), repository.Provider.ToString());
      }
    }

    public void Dispose()
    {
      using (var repository = new LinqToSqlRepository<TestEntity>(this.Connection()))
      {
        repository.DeleteAll().Commit();
      }
    }

    private IDbConnection Connection()
    {
      var connection = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection();
      connection.ConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
      connection.Open();

      return connection;
    }
  }
}
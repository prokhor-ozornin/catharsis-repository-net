using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="LinqToSqlRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class LinqToSqlRepositoryTests : IDisposable
  {
    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="LinqToSqlRepository{ENTITY}(IDbConnection)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<MockEntity>(null));

      IDbConnection connection;
      using (connection = this.Connection())
      {
        using (var repository = new LinqToSqlRepository<MockEntity>(connection))
        {
          Assert.True(ReferenceEquals(repository.DataContext.Connection, connection));
        }
      }
      Assert.Equal(ConnectionState.Closed, connection.State);
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new MockEntity();

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<MockEntity>(this.Connection()).Delete(null));

      var entity = new MockEntity();

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
      {
        Assert.True(ReferenceEquals(repository.DeleteAll(), repository));
        Assert.False(repository.Commit().Any());

        repository.Persist(new MockEntity()).Persist(new MockEntity());
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
      var entity = new MockEntity();

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      var entity = new MockEntity();

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<MockEntity>(this.Connection()).Persist(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="LinqToSqlRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new LinqToSqlRepository<MockEntity>(this.Connection()).Refresh(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
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
      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
      {
        Assert.NotNull(repository.Transaction());
      }
    }

    private IDbConnection Connection()
    {
      var connection = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection();
      connection.ConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
      connection.Open();
      
      return connection;
    }

    public void Dispose()
    {
      using (var repository = new LinqToSqlRepository<MockEntity>(this.Connection()))
      {
        repository.DeleteAll().Commit();
      }
    }
  }
}
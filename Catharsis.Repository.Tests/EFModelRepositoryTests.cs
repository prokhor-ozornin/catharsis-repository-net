using System;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Catharsis.Commons;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="EFModelRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class EFModelRepositoryTests : IDisposable
  {
    private readonly string connectionString = ConfigurationManager.ConnectionStrings["SQLServer.EF"].ConnectionString;

    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="EFModelRepository{ENTITY}(string)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new EFModelRepository<MockEntity>(null));
      Assert.Throws<ArgumentException>(() => new EFModelRepository<MockEntity>(string.Empty));

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        Assert.Equal(this.connectionString, repository.ObjectContext.Connection.ConnectionString);
        Assert.True(ReferenceEquals(repository.Field("objectSet").To<ObjectSet<MockEntity>>().Context, repository.Field("objectContext").To<ObjectContext>()));
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new MockEntity();

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
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
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Delete(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFModelRepository<MockEntity>(this.connectionString).Delete(null));

      var entity = new MockEntity();

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        Assert.Throws<InvalidOperationException>(() => repository.Delete(entity));
        Assert.False(repository.Persist(entity).Delete(entity).Commit().Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.DeleteAll()"/> method.</para>
    /// </summary>
    [Fact]
    public void DeleteAll_Method()
    {
      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
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
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var entity = new MockEntity();

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        repository.Persist(entity).Dispose();
        Assert.Throws<ObjectDisposedException>(() => repository.Single());

        repository.Dispose();
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.GetEnumerator()"/> method.</para>
    /// </summary>
    [Fact]
    public void GetEnumerator_Method()
    {
      var entity = new MockEntity();

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        Assert.False(repository.GetEnumerator().MoveNext());

        repository.Persist(entity).Commit();
        var enumerator = repository.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Persist(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFModelRepository<MockEntity>(this.connectionString).Persist(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
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
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new EFModelRepository<MockEntity>(this.connectionString).Refresh(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
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
    ///   <para>Performs testing of <see cref="EFModelRepository{ENTITY}.Transaction(IsolationLevel?)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        Assert.NotNull(repository.Transaction());
      }
    }

    public void Dispose()
    {
      using (var repository = new EFModelRepository<MockEntity>(this.connectionString))
      {
        repository.DeleteAll().Commit();
      }
    }
  }
}
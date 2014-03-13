using System;
using System.Data;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using Xunit;

namespace Catharsis.Repository
{
  /// <summary>
  ///   <para>Tests set for class <see cref="NHibernateRepository{ENTITY}"/>.</para>
  /// </summary>
  public sealed class NHibernateRepositoryTests : IDisposable
  {
    private readonly Configuration configuration = Bootstrapper.NHibernate();

    /// <summary>
    ///   <para>Performs testing of class constructor(s).</para>
    ///   <seealso cref="NHibernateRepository{ENTITY}(ISessionFactory)"/>
    ///   <seealso cref="NHibernateRepository{ENTITY}(Configuration)"/>
    /// </summary>
    [Fact]
    public void Constructors()
    {
      Assert.Throws<ArgumentNullException>(() => new NHibernateRepository<MockEntity>((ISessionFactory) null));
      Assert.Throws<ArgumentNullException>(() => new NHibernateRepository<MockEntity>((Configuration)null));

      using (var sessionFactory = this.configuration.BuildSessionFactory())
      {
        using (var repository = new NHibernateRepository<MockEntity>(sessionFactory))
        {
          var session = repository.Session;
          Assert.True(ReferenceEquals(sessionFactory, session.SessionFactory));
          Assert.Equal(FlushMode.Never, session.FlushMode);
        }

        using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
        {
          var session = repository.Session;
          Assert.False(ReferenceEquals(sessionFactory, session.SessionFactory));
          Assert.Equal(FlushMode.Never, session.FlushMode);
        }
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Commit()"/> method.</para>
    /// </summary>
    [Fact]
    public void Commit_Method()
    {
      var entity = new MockEntity();

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
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
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Delete(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Delete_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new NHibernateRepository<MockEntity>(this.configuration).Delete(null));

      var entity = new MockEntity();

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        repository.Transaction(() => Assert.True(ReferenceEquals(repository.Delete(entity), repository)));
        
        repository.Transaction(() => repository.Persist(entity));
        repository.Transaction(() => repository.Delete(new MockEntity()));
        Assert.True(ReferenceEquals(repository.Single(), entity));
        
        repository.Transaction(() => repository.Delete(entity));
        Assert.False(repository.Any());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.DeleteAll()"/> method.</para>
    /// </summary>
    [Fact]
    public void DeleteAll_Method()
    {
      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
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
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Dispose()"/> method.</para>
    /// </summary>
    [Fact]
    public void Dispose_Method()
    {
      var entity = new MockEntity();

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        repository.Persist(entity).Dispose();
        Assert.Throws<ObjectDisposedException>(() => repository.Single());

        repository.Dispose();
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.GetEnumerator()"/> method.</para>
    /// </summary>
    [Fact]
    public void GetEnumerator_Method()
    {
      var entity = new MockEntity();

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        Assert.False(repository.GetEnumerator().MoveNext());

        repository.Transaction(() => repository.Persist(entity));
        var enumerator = repository.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.False(enumerator.MoveNext());
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Persist(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Persist_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new NHibernateRepository<MockEntity>(this.configuration).Persist(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        Assert.False(repository.Any());

        repository.Transaction(() => Assert.True(ReferenceEquals(repository.Persist(entity), repository)));
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("first", entity.Name);

        entity.Name = "second";
        repository.Transaction(() => repository.Persist(entity));
        entity = repository.Single();
        Assert.NotEqual(0, entity.Id);
        Assert.Equal("second", entity.Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Refresh(ENTITY)"/> method.</para>
    /// </summary>
    [Fact]
    public void Refresh_Method()
    {
      Assert.Throws<ArgumentNullException>(() => new NHibernateRepository<MockEntity>(this.configuration).Refresh(null));

      var entity = new MockEntity { Name = "first" };

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        Assert.True(ReferenceEquals(repository.Refresh(entity), repository));
        Assert.Equal(0, entity.Id);
        Assert.Equal("first", entity.Name);

        repository.Transaction(() => repository.Persist(entity));
        var originalId = entity.Id;
        var originalName = entity.Name;

        entity.Name = "second";
        repository.Refresh(entity);
        Assert.Equal(originalId, entity.Id);
        Assert.Equal(originalName, entity.Name);
      }
    }

    /// <summary>
    ///   <para>Performs testing of <see cref="NHibernateRepository{ENTITY}.Transaction(IsolationLevel?)"/> method.</para>
    /// </summary>
    [Fact]
    public void Transaction_Method()
    {
      var entity = new MockEntity();

      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        using (repository.Transaction())
        {
          repository.Persist(new MockEntity());
        }
        Assert.Equal(0, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Persist(new MockEntity());
          transaction.Rollback();
        }
        Assert.Equal(0, repository.Count());

        try
        {
          using (repository.Transaction())
          {
            repository.Persist(new MockEntity());
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
          repository.Persist(entity);
          transaction.Commit();
        }
        Assert.Equal(1, repository.Count());


        using (repository.Transaction())
        {
          repository.Delete(entity);
        }
        Assert.Equal(1, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Delete(entity);
          transaction.Rollback();
        }
        Assert.Equal(1, repository.Count());

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
        Assert.Equal(1, repository.Count());

        using (var transaction = repository.Transaction())
        {
          repository.Delete(entity);
          transaction.Commit();
        }
        Assert.Equal(0, repository.Count());
      }
    }

    public void Dispose()
    {
      using (var repository = new NHibernateRepository<MockEntity>(this.configuration))
      {
        repository.DeleteAll().Commit();
      }
    }
  }
}
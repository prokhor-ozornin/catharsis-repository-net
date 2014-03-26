**Catharsis.Repository** .NET library provides abstraction over persistent data storages by introducing common _repository_ pattern interfaces over popular ORM frameworks (_NHibernate_, _Entity Framework_, _LINQ2SQL_), as well as their implementations.

**Target** : .NET Framework 4.0

**NuGet package** : [https://www.nuget.org/packages/Catharsis.Repository](https://www.nuget.org/packages/Catharsis.Repository)

[![Image](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=APHM8MU9N76V8 "Donate")

The following ORM frameworks are supported at this time :

1. [NHibernate](http://nhforge.org)
2. Entity Framework 6 (Database-First/Code-First approaches)
3. LINQ to SQL

This library helps you to deal with diversity and complexity of different API for these frameworks, providing most widely useful functionality through `IRepository<ENTITY>` interface :

> public interface IRepository < ENTITY > : IDisposable, IQueryable < ENTITY > where ENTITY : class
>
> {
>
>   IRepository < ENTITY > Commit();

>   IRepository < ENTITY > Delete(ENTITY entity);

>   IRepository < ENTITY > DeleteAll();

>   IRepository < ENTITY > Persist(ENTITY entity);

>   IRepository < ENTITY > Refresh(ENTITY entity);

>   ITransaction Transaction(IsolationLevel? isolation = null);
>
> }

Concrete implementations are provided as well :

1. **NHibernate**: `NHibernateRepository`
2. **LINQ2SQL**: `LinqToSqlRepository`
3. **Entity Framework (Database-First)** : `EFModelRepository`
4. **Entity Framework (Code-First)** : `EFCodeFirstRepository`
5. `MemoryRepository`

**Usage examples :**

**NHibernateRepository**

You can create `NHibernateRepository<ENTITY>` generic class instance by passing to its constructor either :

1. `ISession`
2. `ISessionFactory`
3. `IConfiguration`

Use first option to share single long-running session (as well as its cache) by different instances of repositories (for different types of business entities), or either second or third to create short-living session which will be internally managed by the repository and be closed when `NHibernateRepository<ENTITY>.Dispose()` method is called.

To commit performed changes use either `NHibernateRepository<ENTITY>.Commit()` method, or create a transactional block.

Querying is performed as usual for `IQueryable<T>` objects.

> var putin = new Person { Name = "Putin" };
>
> var medvedev = new Person { Name = "Medvedev" };
>
> var obama = new Person { Name = "Obama" };
>
> IConfiguration configuration = ...
>
> using (var repository = new NHibernateRepository < Person > (configuration))
>
> {
>
>   repository.Persist(putin).Persist(medvedev).Persist(obama).Commit(); // Persist three entities and commit changes
>
>   repository.Transaction(() => repository.Delete(obama)); // Delete entity inside a transaction, commit changes automatically
>
> var putin = repository.FirstOrDefault(person => person.Name == "Putin");
> 
> var obama = repository.FirstOrDefault(person => person.Name == "Obama");
>
> if (repository.Any(person => person.Name == "Medvedev"))
>
> { // Baby-bear always near } 
>
> if (putin != null && obama == null)
>
> { // No Obama ? Let us cheer ! }
>
> }

Code is well commented, refer to XML docs for more details.
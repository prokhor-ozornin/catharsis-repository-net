using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Unity;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Catharsis.Repository.Tests;

internal static class Bootstrapper
{
  public static IUnityContainer Unity()
  {
    var container = new UnityContainer();
    var configuration = NHibernate();
    
    container.RegisterInstance<IRepository<TestEntity>>(new NHibernateRepository<TestEntity>(configuration));
    
    return container;
  }

  public static Configuration NHibernate()
  {
    return Fluently.Configure()
      .Database(MsSqlConfiguration.MsSql2008.ConnectionString(connection => connection.FromConnectionStringWithKey("SQLServer")).ShowSql().FormatSql())
      .Mappings(mapping => mapping.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(TestEntity))))
      .ExposeConfiguration(configuration =>
      {
        new SchemaExport(configuration).Drop(false, true);
        new SchemaExport(configuration).Create(false, true);
      }).BuildConfiguration();
  }
}
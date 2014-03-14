using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Practices.Unity;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Catharsis.Repository
{
  internal static class Bootstrapper
  {
    public static IUnityContainer Unity()
    {
      var container = new UnityContainer();
      var configuration = NHibernate();
      
      container.RegisterInstance<IRepository<MockEntity>>(new NHibernateRepository<MockEntity>(configuration));
      
      return container;
    }

    public static Configuration NHibernate()
    {
      return Fluently.Configure()
        .Database(MsSqlConfiguration.MsSql2008.ConnectionString(x => x.FromConnectionStringWithKey("SQLServer")).ShowSql().FormatSql())
        .Mappings(mappings => mappings.FluentMappings.AddFromAssembly(Assembly.GetAssembly(typeof(MockEntity))))
        .ExposeConfiguration(configuration =>
        {
          new SchemaExport(configuration).Drop(false, true);
          new SchemaExport(configuration).Create(false, true);
        }).BuildConfiguration();
    }
  }
}
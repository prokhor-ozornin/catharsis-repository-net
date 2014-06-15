using System.Configuration;
using System.Data.Entity;

namespace Catharsis.Repository
{
  internal sealed class TestContext : DbContext
  {
    public TestContext() : base(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString)
    {
    }

    public DbSet<TestEntity> MockEntity { get; set; }
  }
}
using System.Configuration;
using System.Data.Entity;

namespace Catharsis.Repository
{
  internal sealed class MockContext : DbContext
  {
    public MockContext() : base(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString)
    {
    }

    public DbSet<MockEntity> MockEntity { get; set; }
  }
}
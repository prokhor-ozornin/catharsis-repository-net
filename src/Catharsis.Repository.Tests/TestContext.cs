using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Catharsis.Repository.Tests;

internal sealed class TestContext : DbContext
{
  public TestContext() : base(ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString)
  {
  }

  public DbSet<TestEntity> MockEntity { get; set; }
}
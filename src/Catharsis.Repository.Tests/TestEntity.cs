using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catharsis.Repository.Tests;

[Table]
internal class TestEntity
{
  [Column(IsPrimaryKey = true, IsDbGenerated = true)]
  [Key]
  public virtual long Id { get; set; }

  [Column]
  public virtual string Name { get; set; }
}
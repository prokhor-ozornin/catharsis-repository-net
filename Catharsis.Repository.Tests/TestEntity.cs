using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;

namespace Catharsis.Repository
{
  [Table]
  internal class TestEntity
  {
    [Column(IsPrimaryKey = true, IsDbGenerated = true)]
    [Key]
    public virtual long Id { get; set; }

    [Column]
    public virtual string Name { get; set; }
  }
}
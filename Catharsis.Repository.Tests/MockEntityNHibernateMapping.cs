using FluentNHibernate.Mapping;

namespace Catharsis.Repository
{
  internal sealed class MockEntityNHibernateMapping : ClassMap<TestEntity>
  {
    public MockEntityNHibernateMapping()
    {
      this.Id(entity => entity.Id).GeneratedBy.Native();
      this.Map(entity => entity.Name).Index("MockEntities__Name");
    }
  }
}
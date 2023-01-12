using FluentNHibernate.Mapping;

namespace Catharsis.Repository.Tests;

internal sealed class MockEntityNHibernateMapping : ClassMap<TestEntity>
{
  public MockEntityNHibernateMapping()
  {
    Id(entity => entity.Id).GeneratedBy.Native();
    Map(entity => entity.Name).Index("MockEntities__Name");
  }
}
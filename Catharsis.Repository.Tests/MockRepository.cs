using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Catharsis.Repository
{
  internal sealed class MockRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
  {
    public override IEnumerator<ENTITY> GetEnumerator()
    {
      return Enumerable.Empty<ENTITY>().GetEnumerator();
    }

    public override IRepository<ENTITY> Commit()
    {
      return this;
    }

    public override IRepository<ENTITY> Delete(ENTITY entity)
    {
      return this;
    }

    public override IRepository<ENTITY> DeleteAll()
    {
      return this;
    }

    public override IRepository<ENTITY> Persist(ENTITY entity)
    {
      return this;
    }

    public override IRepository<ENTITY> Refresh(ENTITY entity)
    {
      return this;
    }

    public override ITransaction Transaction(IsolationLevel? isolation = null)
    {
      return new MockTransaction(isolation);
    }
  }
}
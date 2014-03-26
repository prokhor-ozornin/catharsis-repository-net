using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Catharsis.Repository
{
  internal sealed class TestRepository<ENTITY> : RepositoryBase<ENTITY> where ENTITY : class
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
      return new TestTransaction(isolation);
    }

    public override Expression Expression
    {
      get { return Enumerable.Empty<ENTITY>().AsQueryable().Expression; }
    }

    public override Type ElementType
    {
      get { return Enumerable.Empty<ENTITY>().AsQueryable().ElementType; }
    }

    public override IQueryProvider Provider
    {
      get { return Enumerable.Empty<ENTITY>().AsQueryable().Provider; }
    }
  }
}
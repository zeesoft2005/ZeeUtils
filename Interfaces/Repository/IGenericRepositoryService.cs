using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeeUtils.Interfaces.Repository
{
    public interface IGenericRepositoryService<T> where T : class
    {
        /// <summary>
        /// The slim version means fetch records but not nested objects
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>all records</returns>
        List<T> GetAllSlim(string orgId);
        List<T> GetAll(string orgId);
        T GetById(string id);
        T GetByIdSlim(string id);
        T AddOrUpdate(T entity);
        bool Delete(string id);
        bool Any(Func<T, bool> where);
    }

}

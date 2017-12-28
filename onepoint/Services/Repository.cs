using Newtonsoft.Json;
using onepoint.Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace onepoint.Services
{
    public interface IRepository<T> where T : IEntity
    {
        void AddOrUpdate(T entity);

        T FindById(Guid id);

        Task StoreAsync();
    }
}

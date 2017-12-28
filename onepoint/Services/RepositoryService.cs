using Newtonsoft.Json;
using onepoint.Models.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace onepoint.Services
{
    public class RepositoryService<T> : IRepository<T> where T: IEntity
    {
        private readonly string storeName = $"{typeof(T)}.json";
        private readonly ConcurrentDictionary<Guid, T> store = new ConcurrentDictionary<Guid, T>();

        public RepositoryService()
        {
            try
            {
                var content = File.ReadAllText(storeName);
                store = JsonConvert.DeserializeObject<ConcurrentDictionary<Guid, T>>(content);
            }
            catch
            {
                Console.WriteLine($"{storeName} Repository not found...");
            }
        }

        public void AddOrUpdate(T entity)
        {
            var keyProp = entity.GetType().GetProperties().SingleOrDefault(prop => Attribute.IsDefined(prop, typeof(KeyAttribute)));
            if (keyProp != null && (Guid)keyProp.GetValue(entity) == Guid.Empty) keyProp.SetValue(entity, Guid.NewGuid());

            store.AddOrUpdate((Guid)keyProp.GetValue(entity),
                entity, (key, oldValue) => entity);
        }

        public T FindById(Guid id)
        {
            return store.GetValueOrDefault(id);
        }

        public async Task StoreAsync()
        {
            var content = JsonConvert.SerializeObject(store);
            await File.WriteAllTextAsync(storeName, content);
        }

        public static Guid ToGuid(object property)
        {
            byte[] hashedBytes = property.GetHashCode().ToString().Select(Convert.ToByte).ToArray();
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }
    }
}

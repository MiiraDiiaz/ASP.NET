using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain;
namespace PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>: IRepository<T> where T: BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(List<T> data)
        {
            Data = data;
        }

        public Task<List<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public void Add(T entity)
        {
            Data.Add(entity); 
        }

        public void Update(Guid id,T entity)
        {
            Data.Remove(Data.FirstOrDefault(x => x.Id == id));
            Data.Add(entity);
        }

        public bool Delete(Guid id)
        {
            return Data.Remove(Data.FirstOrDefault(x => x.Id == id));
        }
    }
}
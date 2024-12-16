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
        protected IList<T> Data { get; set; }

        public InMemoryRepository(IList<T> data)
        {
            Data = data;
        }

        public Task<IList<T>> GetAllAsync()
        {
            return Task.FromResult(Data);
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public void Create(T entity)
        {
            Data.Add(entity); 
        }

        public void Update(Guid id,T entity)
        {
            Data.Remove(Data.FirstOrDefault(x => x.Id == id));
            Data.Add(entity);
        }

        public void Delete(Guid id)
        {
            Data.Remove(Data.FirstOrDefault(x => x.Id == id));
        }
    }
}
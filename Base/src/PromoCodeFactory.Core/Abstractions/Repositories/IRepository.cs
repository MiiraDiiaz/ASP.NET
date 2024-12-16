using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PromoCodeFactory.Core.Domain;

namespace PromoCodeFactory.Core.Abstractions.Repositories
{
    public interface IRepository<T> where T: BaseEntity
    {
        Task<IList<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        void Create(T entity); 

        void Update(Guid id,T entity); 

        void Delete(Guid id); 
    }
}
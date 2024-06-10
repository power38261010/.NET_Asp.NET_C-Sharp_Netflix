using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Services.Contracts
{
    public interface IBaseService<T> where T : class
    {
        Task<List<T>?> GetAll();
        Task Create(T EntityToCreate);
        Task Delete(int id);
        Task Edit(T EntityToEdit);
        Task<T?> GetById(int id);
    }
}
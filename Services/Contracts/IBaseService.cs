using System.Collections.Generic;
using System.Threading.Tasks;

namespace BussinessLayer.Services.Contracts
{
    public interface IBaseService<T, K> where T : class
    {
        Task<IEnumerable<T>?> GetAll();
        Task Create(K EntityToCreate);
        Task Delete(int id);
        Task Edit(K EntityToEdit);
        Task<T?> GetById(int id);
    }
}

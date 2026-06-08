using System.Collections.Generic;
using System.Threading.Tasks;
using TechMoveCRM.Models;

namespace TechMoveCRM.Services
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client> GetByIdAsync(int id);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
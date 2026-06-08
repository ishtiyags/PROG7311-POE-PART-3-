using System.Collections.Generic;
using System.Threading.Tasks;
using TechMoveCRM.Models;

namespace TechMoveCRM.Services
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId);
        Task<ServiceRequest> GetByIdAsync(int id);
        Task AddAsync(ServiceRequest request);
        Task UpdateAsync(ServiceRequest request);
        Task DeleteAsync(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechMoveCRM.Models;

namespace TechMoveCRM.Services
{
    public interface IContractRepository
    {
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<Contract> GetByIdAsync(int id);
        Task AddAsync(Contract contract);
        Task UpdateAsync(Contract contract);
        Task DeleteAsync(int id);

        // LINQ Search/Filter method
        Task<IEnumerable<Contract>> SearchAsync(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status);
    }
}
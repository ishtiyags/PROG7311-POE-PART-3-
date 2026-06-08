using Microsoft.EntityFrameworkCore;
using TechMoveCRM.API.Data;
using TechMoveCRM.API.Models;

namespace TechMoveCRM.API.Services
{
    public class ContractService : IContractService
    {
        private readonly AppDbContext _context;

        public ContractService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contract>> GetAllContracts()
        {
            return await _context.Contracts.ToListAsync();
        }

        public async Task<Contract?> GetContract(int id)
        {
            return await _context.Contracts.FindAsync(id);
        }

        public async Task<Contract> CreateContract(
            Contract contract)
        {
            _context.Contracts.Add(contract);

            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<bool> UpdateStatus(
    int id,
    ContractStatus status)
        {
            var contract =
                await _context.Contracts.FindAsync(id);

            if (contract == null)
                return false;

            contract.Status = status;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
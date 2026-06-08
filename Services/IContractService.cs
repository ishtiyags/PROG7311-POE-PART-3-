using TechMoveCRM.API.Models;

public interface IContractService
{
    Task<List<Contract>> GetAllContracts();

    Task<Contract?> GetContract(int id);

    Task<Contract> CreateContract(Contract contract);

    Task<bool> UpdateStatus(int id, ContractStatus status);
}
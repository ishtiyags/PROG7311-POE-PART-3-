using TechMoveCRM.Models;

namespace TechMoveCRM.Services
{
    public class ContractWorkflowService : IContractWorkflowService
    {
        public bool CanCreateServiceRequest(Contract contract)
        {
            // Business Rule: Cannot create a request on Expired or OnHold contracts
            if (contract == null) return false;

            return contract.Status != ContractStatus.Expired
                && contract.Status != ContractStatus.OnHold;
        }

        public string GetBlockedReason(Contract contract)
        {
            if (contract == null)
                return "Contract not found.";

            return contract.Status switch
            {
                ContractStatus.Expired => "Cannot create a Service Request: this contract has Expired.",
                ContractStatus.OnHold => "Cannot create a Service Request: this contract is On Hold.",
                _ => string.Empty
            };
        }
    }
}
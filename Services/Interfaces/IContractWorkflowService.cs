using TechMoveCRM.Models;

namespace TechMoveCRM.Services
{
    public interface IContractWorkflowService
    {
        /// <summary>
        /// Returns true if a Service Request CAN be created for this contract.
        /// </summary>
        bool CanCreateServiceRequest(Contract contract);

        /// <summary>
        /// Returns a user-friendly error message if creation is blocked.
        /// </summary>
        string GetBlockedReason(Contract contract);
    }
}
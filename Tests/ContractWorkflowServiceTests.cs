using Xunit;
using TechMoveCRM.Models;
using TechMoveCRM.Services;

namespace TechMoveCRM.Tests
{
    public class ContractWorkflowServiceTests
    {
        private readonly ContractWorkflowService _service = new ContractWorkflowService();

        [Fact]
        public void CanCreateServiceRequest_WithActiveContract_ReturnsTrue()
        {
            var contract = new Contract { Status = ContractStatus.Active };
            Assert.True(_service.CanCreateServiceRequest(contract));
        }

        [Fact]
        public void CanCreateServiceRequest_WithExpiredContract_ReturnsFalse()
        {
            // Core business rule test
            var contract = new Contract { Status = ContractStatus.Expired };
            Assert.False(_service.CanCreateServiceRequest(contract));
        }

        [Fact]
        public void CanCreateServiceRequest_WithOnHoldContract_ReturnsFalse()
        {
            var contract = new Contract { Status = ContractStatus.OnHold };
            Assert.False(_service.CanCreateServiceRequest(contract));
        }

        [Fact]
        public void CanCreateServiceRequest_WithDraftContract_ReturnsTrue()
        {
            // Draft contracts CAN have requests (only Expired/OnHold are blocked)
            var contract = new Contract { Status = ContractStatus.Draft };
            Assert.True(_service.CanCreateServiceRequest(contract));
        }

        [Fact]
        public void CanCreateServiceRequest_WithNullContract_ReturnsFalse()
        {
            // Edge case: null should not throw, just return false
            Assert.False(_service.CanCreateServiceRequest(null));
        }

        [Fact]
        public void GetBlockedReason_WithExpiredContract_ReturnsExpiredMessage()
        {
            var contract = new Contract { Status = ContractStatus.Expired };
            var reason = _service.GetBlockedReason(contract);
            Assert.Contains("Expired", reason);
        }

        [Fact]
        public void GetBlockedReason_WithOnHoldContract_ReturnsOnHoldMessage()
        {
            var contract = new Contract { Status = ContractStatus.OnHold };
            var reason = _service.GetBlockedReason(contract);
            Assert.Contains("On Hold", reason);
        }

        [Fact]
        public void GetBlockedReason_WithActiveContract_ReturnsEmptyString()
        {
            var contract = new Contract { Status = ContractStatus.Active };
            var reason = _service.GetBlockedReason(contract);
            Assert.Equal(string.Empty, reason);
        }

        [Fact]
        public void GetBlockedReason_WithNullContract_ReturnsNotFoundMessage()
        {
            var reason = _service.GetBlockedReason(null);
            Assert.NotEmpty(reason);
        }
    }
}
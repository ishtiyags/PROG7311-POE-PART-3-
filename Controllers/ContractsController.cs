using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMoveCRM.API.Models;
using TechMoveCRM.API.Services;

namespace TechMoveCRM.API.Controllers
{
   // [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _service;

        public ContractsController(IContractService service)
        {
            _service = service;
        }

        // GET: api/contracts
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var contracts = await _service.GetAllContracts();

            return Ok(contracts);
        }

        // GET: api/contracts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var contract = await _service.GetContract(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        // POST: api/contracts
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Contract contract)
        {
            var result = await _service.CreateContract(contract);

            return CreatedAtAction(
                nameof(Get),
                new { id = result.ContractId },
                result);
        }

        // PATCH: api/contracts/5/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] ContractStatus status)
        {
            var updated = await _service.UpdateStatus(id, status);

            if (!updated)
                return NotFound();

            return Ok(new
            {
                Message = "Contract status updated successfully."
            });
        }
    }
}
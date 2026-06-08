using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechMoveCRM.Models;
using TechMoveCRM.Services;
using TechMoveCRM.ViewModels;

namespace TechMoveCRM.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestRepository _requestRepo;
        private readonly IContractRepository _contractRepo;
        private readonly ICurrencyService _currencyService;
        private readonly IContractWorkflowService _workflowService;

        public ServiceRequestsController(
            IServiceRequestRepository requestRepo,
            IContractRepository contractRepo,
            ICurrencyService currencyService,
            IContractWorkflowService workflowService)
        {
            _requestRepo = requestRepo;
            _contractRepo = contractRepo;
            _currencyService = currencyService;
            _workflowService = workflowService;
        }

        // GET: Create page — loads currency rate from API
        public async Task<IActionResult> Create(int contractId)
        {
            var contract = await _contractRepo.GetByIdAsync(contractId);
            if (contract == null) return NotFound();

            // Workflow check — block if Expired or On Hold
            if (!_workflowService.CanCreateServiceRequest(contract))
            {
                TempData["Error"] = _workflowService.GetBlockedReason(contract);
                return RedirectToAction("Details", "Contracts", new { id = contractId });
            }

            // Fetch current exchange rate (async)
            var rate = await _currencyService.GetUsdToZarRateAsync();

            var viewModel = new ServiceRequestCreateViewModel
            {
                ContractId = contractId,
                Contract = contract,
                CurrentUsdToZarRate = rate
            };

            return View(viewModel);
        }

        // GET: AJAX endpoint to calculate ZAR from USD in real-time
        [HttpGet]
        public async Task<IActionResult> GetZarAmount(decimal usdAmount)
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();
            var zarAmount = _currencyService.ConvertUsdToZar(usdAmount, rate);
            return Json(new { zarAmount, rate });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Contract = await _contractRepo.GetByIdAsync(model.ContractId);
                model.CurrentUsdToZarRate = await _currencyService.GetUsdToZarRateAsync();
                return View(model);
            }

            var contract = await _contractRepo.GetByIdAsync(model.ContractId);
            if (contract == null) return NotFound();

            // Double-check workflow on POST (security — can't trust client)
            if (!_workflowService.CanCreateServiceRequest(contract))
            {
                TempData["Error"] = _workflowService.GetBlockedReason(contract);
                return RedirectToAction("Details", "Contracts", new { id = model.ContractId });
            }

            // Calculate ZAR cost server-side (never trust the client's calculated value)
            var rate = await _currencyService.GetUsdToZarRateAsync();
            var zarCost = _currencyService.ConvertUsdToZar(model.CostUSD, rate);

            var request = new ServiceRequest
            {
                ContractId = model.ContractId,
                Description = model.Description,
                CostUSD = model.CostUSD,
                CostZAR = zarCost,
                Status = ServiceRequestStatus.Open
            };

            await _requestRepo.AddAsync(request);
            TempData["Success"] = $"Service request created. Cost: R{zarCost:N2} (@ {rate:N2} ZAR/USD)";
            return RedirectToAction("Details", "Contracts", new { id = model.ContractId });
        }

        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _requestRepo.GetByIdAsync(id);
            int contractId = request?.ContractId ?? 0;
            await _requestRepo.DeleteAsync(id);
            TempData["Success"] = "Request deleted.";
            return RedirectToAction("Details", "Contracts", new { id = contractId });
        }
    }
}
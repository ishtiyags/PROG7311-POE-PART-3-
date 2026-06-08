using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using TechMoveCRM.Models;
using TechMoveCRM.Services;
using TechMoveCRM.MVC.Services;
using TechMoveCRM.ViewModels;
using Microsoft.IdentityModel.Tokens;

namespace TechMoveCRM.Controllers
{
    public class ContractsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IFileService _fileService;

        public ContractsController(ApiService apiService, IFileService fileService)
        {
            _apiService = apiService;
            _fileService = fileService;
        }

        // GET: Contracts (API)
        public async Task<IActionResult> Index()
        {
            var contracts = await _apiService.GetContracts();
            return View(contracts);
        }

        // GET: Details
        public async Task<IActionResult> Details(int id)
        {
            var contract = (await _apiService.GetContracts())
                .FirstOrDefault(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Search (done locally on API data)
        public async Task<IActionResult> Search(ContractSearchViewModel model)
        {
            var contracts = await _apiService.GetContracts();

            model.Results = contracts.Where(c =>
                (!model.StartDate.HasValue || c.StartDate >= model.StartDate) &&
                (!model.EndDate.HasValue || c.EndDate <= model.EndDate) &&
             (!model.Status.HasValue || c.Status == model.Status)
            ).ToList();

            return View(model);
        }

        // GET: Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create (API)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, IFormFile signedAgreement)
        {
            ModelState.Remove("SignedAgreementPath");

            if (signedAgreement != null && signedAgreement.Length > 0)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("", "Only PDF files allowed.");
                    return View(contract);
                }

                contract.SignedAgreementPath =
                    await _fileService.SavePdfAsync(signedAgreement, "contracts");
            }

            if (!ModelState.IsValid)
                return View(contract);

            await _apiService.CreateContract(contract);

            TempData["Success"] = "Contract created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var contract = (await _apiService.GetContracts())
                .FirstOrDefault(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract, IFormFile signedAgreement)
        {
            if (id != contract.ContractId)
                return BadRequest();

            ModelState.Remove("SignedAgreementPath");

            if (signedAgreement != null && signedAgreement.Length > 0)
            {
                if (!_fileService.IsValidPdf(signedAgreement))
                {
                    ModelState.AddModelError("", "Only PDF files allowed.");
                    return View(contract);
                }

                contract.SignedAgreementPath =
                    await _fileService.SavePdfAsync(signedAgreement, "contracts");
            }

            if (!ModelState.IsValid)
                return View(contract);

            await _apiService.UpdateContract(contract);

            TempData["Success"] = "Contract updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int id)
        {
            var contract = (await _apiService.GetContracts())
                .FirstOrDefault(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiService.DeleteContract(id);

            TempData["Success"] = "Contract deleted.";
            return RedirectToAction(nameof(Index));
        }

        // Download file (unchanged)
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = (await _apiService.GetContracts())
                .FirstOrDefault(c => c.ContractId == id);

            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                contract.SignedAgreementPath.TrimStart('/')
                    .Replace('/', Path.DirectorySeparatorChar));

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            return PhysicalFile(filePath, "application/pdf",
                Path.GetFileName(filePath));
        }
    }
}
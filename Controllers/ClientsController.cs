using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TechMoveCRM.Models;
using TechMoveCRM.Services;

namespace TechMoveCRM.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepo;

        public ClientsController(IClientRepository clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientRepo.GetAllAsync();
            return View(clients);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid) return View(client);

            await _clientRepo.AddAsync(client);
            TempData["Success"] = "Client created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.ClientId) return BadRequest();
            if (!ModelState.IsValid) return View(client);

            await _clientRepo.UpdateAsync(client);
            TempData["Success"] = "Client updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _clientRepo.DeleteAsync(id);
            TempData["Success"] = "Client deleted.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientRepo.GetByIdAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }
    }
}
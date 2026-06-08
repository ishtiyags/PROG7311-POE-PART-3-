using System.Net.Http.Json;
using TechMoveCRM.Models;

namespace TechMoveCRM.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _client;

        public ApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Contract>> GetContracts()
        {
            return await _client.GetFromJsonAsync<List<Contract>>("api/contracts");
        }

        public async Task CreateContract(Contract contract)
        {
            await _client.PostAsJsonAsync("api/contracts", contract);
        }

        public async Task UpdateContract(Contract contract)
        {
            await _client.PutAsJsonAsync($"api/contracts/{contract.ContractId}", contract);
        }

        public async Task DeleteContract(int id)
        {
            await _client.DeleteAsync($"api/contracts/{id}");
        }
    }
}
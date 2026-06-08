using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechMoveCRM.API.Models
{
    public class Client
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Contact details are required.")]
        [StringLength(200)]
        public string ContactDetails { get; set; }

        [Required(ErrorMessage = "Region is required.")]
        [StringLength(100)]
        public string Region { get; set; }

        // Navigation property — one client has many contracts
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
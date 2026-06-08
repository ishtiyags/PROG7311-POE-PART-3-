using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TechMoveCRM.Models;

namespace TechMoveCRM.ViewModels
{
    public class ServiceRequestCreateViewModel
    {
        public int ContractId { get; set; }

        [ValidateNever]
        public Contract Contract { get; set; } // For display info

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Cost in USD is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than zero.")]
        public decimal CostUSD { get; set; }

        // These are populated server-side and displayed to the user
        public decimal CurrentUsdToZarRate { get; set; }
        public decimal EstimatedCostZAR { get; set; }
    }
}

// ViewModels are used to pass only required data to the view and prevent exposing domain models
// (Microsoft, 2023)
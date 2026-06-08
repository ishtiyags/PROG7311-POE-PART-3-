using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMoveCRM.API.Models
{
    public enum ServiceRequestStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract Contract { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500)]
        public string Description { get; set; }

        // Cost in USD — what the user types in
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostUSD { get; set; }

        // Final cost in ZAR — calculated and saved
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Open;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
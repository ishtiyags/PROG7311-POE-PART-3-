using System;
using System.Collections.Generic;
using TechMoveCRM.Models;

namespace TechMoveCRM.ViewModels
{
    public class ContractSearchViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus? Status { get; set; }
        public IEnumerable<Contract> Results { get; set; } = new List<Contract>();
    }
}

// ViewModels are used to pass only required data to the view and prevent exposing domain models
// (Microsoft, 2023)
﻿using System.ComponentModel.DataAnnotations;

namespace FastFood.Web.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        [Required]
        [MinLength(2), MaxLength(30)]
        public string Customer { get; set; }

        public int ItemId { get; set; }

        public int EmployeeId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Required]
        public string Type { get; set; }
    }
}

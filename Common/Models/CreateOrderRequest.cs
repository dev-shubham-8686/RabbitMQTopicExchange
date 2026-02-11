using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class CreateOrderRequest
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string? OrderName { get; set; }

        public string? OrderDescription { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        public string? Status { get; set; } = "Added";

    }
}

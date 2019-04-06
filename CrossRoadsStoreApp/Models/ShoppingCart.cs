using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CrossRoadsStoreApp.Models
{
    public class ShoppingCart
    {
        public int ShoppingCartId { get; set; }
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Price { get; set; }
        [Required]
        public string Owner { get; set; }
        public virtual Product Product { get; set; }
    }
}
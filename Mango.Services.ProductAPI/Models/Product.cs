using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Mango.Services.ProductAPI.Models
{
    public class Product
    {
        [Key] public int ProductId { get; set; }
        [Required] public string Name { get; set; }
        [Range(1, 1000)] public double Price { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
    }
}
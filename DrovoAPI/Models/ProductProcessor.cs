using DrovoAPI.Classes;
using DrovoAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Models
{
    public class ProductProcessor
    {
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public Product Product { get; set; }
        public double Priority { get; set; }

        public ProductProcessor(int ProductId, int StoreId, Product Product, double Priority = 0)
        {
            this.StoreId = StoreId;
            this.Product = Product;
            this.ProductId = ProductId;
            this.Priority = Priority;
        }
    }
}

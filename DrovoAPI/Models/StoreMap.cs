using DrovoAPI.Interfaces;
using DrovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Classes
{
    public class StoreMap : IStoreMap
    {
        public string Name { get; set; }
        public string About { get; set; }
        public string Logo { get; set; }
        public string Link { get; set; }
        public int ProductsCount { get; set; }
        public List<Product> Products { get; set; }

        public StoreMap(Site site, List<Product> products)
        {
            this.Name = site.Name;
            this.About = site.Description;
            this.Logo = site.Image;
            this.Link = site.Domain;
            this.ProductsCount = products.Count;
            this.Products = products;
        }
    }
}

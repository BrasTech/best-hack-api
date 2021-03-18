using DrovoAPI.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Interfaces
{
    public interface IStoreMap
    {
        public string Name { get; set; }
        public string About { get; set; }
        public string Logo { get; set; }
        public string Link { get; set; }
        public int ProductsCount { get; set; }
        public List<Product> Products { get; set; }
    }
}

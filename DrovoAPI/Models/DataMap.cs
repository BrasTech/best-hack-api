using DrovoAPI.Classes;
using DrovoAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Models
{
    public class DataMap
    {
        public string categoryName { get; set; }
        public int storesCount { get; set; }
        public int productsTotal { get; set; }
        public List<IStoreMap> stores { get; set; }
        public DataMap(List<IStoreMap> stores = null, int categoryId = 0)
        {
            if (stores == null)
                stores = new List<IStoreMap>();
            this.stores = stores;
            this.storesCount = stores.Count;
            this.productsTotal = stores.SelectMany(u => u.Products).Count();
            this.categoryName = Categories.GetById(categoryId);
        }
    }
}

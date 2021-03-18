using DrovoAPI.Classes;
using DrovoAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Interfaces
{
    public interface ISiteParser
    {
        public List<Site> _sites { get; set; }

        public Task<TotalData> GetData(string query, int categoryId);

        public Task<List<IStoreMap>> GetProducts(string query, int categoryId);

        private StoreMap Map(Site site, List<Product> products)
        {
            return new StoreMap(site, products);
        }

        public void LoadSites();

        public List<Site> GetSites(int categoryId);
    }
}

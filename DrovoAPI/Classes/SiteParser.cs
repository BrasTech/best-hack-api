using DrovoAPI.Interfaces;
using DrovoAPI.Models;
using DrovoAPI.Sites;
using DrovoAPI.Sites.Clothes;
using DrovoAPI.Sites.Food;
using DrovoAPI.Sites.Other;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Classes
{
    public class SiteParser: ISiteParser
    {
        public List<Site> _sites { get; set; }

        public SiteParser()
        {
            LoadSites();
        }
        public async Task<TotalData> GetData(string query, int categoryId)
        {
            var storeMap = await GetProducts(query, categoryId);

            Calculator.NormalizePopularity(storeMap);

            var totalProductsList = Calculator.GroupProducts(storeMap, query, 0.9);

            return GetTotal(totalProductsList, categoryId);
        }

        private TotalData GetTotal(List<TotalProduct> tp, int categoryId)
        {
            return new TotalData
            {
                Category = Categories.GetById(categoryId),
                Total = tp.Select(u=>u.Markets).Count(),
                List = tp
            };
        }

        public async Task<List<IStoreMap>> GetProducts(string query, int categoryId)
        {
            ConcurrentBag<IStoreMap> items = new ConcurrentBag<IStoreMap>();

            var sitesForCategory = GetSites(categoryId);

            Task<List<Product>>[] tasks = new Task<List<Product>>[sitesForCategory.Count];
            for (int i = 0; i < sitesForCategory.Count; i++)
            {
                tasks[i] = sitesForCategory[i].Search(query);
            }

            var products = await Task.WhenAll(tasks);

            for (int i = 0; i < sitesForCategory.Count; i++)
            {
                if (products[i].Count != 0)
                    items.Add(Map(sitesForCategory[i], products[i]));
            }

            return items.ToList();
        }

        /// <summary>
        /// Упаковываем полученные данные
        /// </summary>
        /// <param name="site"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        private StoreMap Map(Site site, List<Product> products)
        {
            return new StoreMap(site, products);
        }

        public void LoadSites()
        {
            _sites = new List<Site>();
            _sites.AddRange(new List<Site> {
                // Сайты для техники
                new TechnoParkSite(5),
                new BeeLineSite(5),
                new EldoradoSite(5),
                // Сайты для еды
                new AzbukaVkusaSite(5),
                new UtkonosSite(5),
                new VkusvillSite(5),
                // Сайты для одежды
                new SelaSite(5),
                // Общие сайты
                new OnlineTradeSite(5),
                //new OzonSite(5),
                new VseVseSite(5),
                new WildberriesSite(5)
            });
        }

        public List<Site> GetSites(int categoryId)
        {
            return _sites.Where(i => i.CategoryIds.Contains(categoryId)).ToList();
        }

    }
}

using DrovoAPI.Classes;
using DrovoAPI.Interfaces;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Models
{
    public abstract class Site: SiteView
    {
        public abstract override string GetTitle(HtmlNode item);
        public abstract override string GetDescription(HtmlNode item);
        public abstract override string GetRating(HtmlNode item);
        public abstract override string GetPrice(HtmlNode item);
        public abstract override string GetPopularity(HtmlNode item);
        public abstract override string GetImage(HtmlNode item);
        public abstract override string GetLink(HtmlNode item);
        public abstract override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery);
        public abstract override Task<List<Product>> Search(string searchQuery);
    }
}

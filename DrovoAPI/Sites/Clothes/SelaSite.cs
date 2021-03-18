using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DrovoAPI.Sites.Clothes
{
    public class SelaSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Sela";
        public override string Name { get => name; set => name = value; }

        private string description = "Магазин одежды";
        public override string Description { get => description; set => description = value; }


        private string domain = "https://www.sela.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 24 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://1ya.ru/i/shops/sela.png";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/eshop/search/REQQUERY/";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public SelaSite(int loadLimit)
        {
            _doc = new HtmlDocument();
            LoadLimit = loadLimit;
        }

        public async override Task<List<Product>> Search(string searchQuery)
        {
            int page = 1;
            List<Product> products = new List<Product>();
            var items = await GetProducts(page, searchQuery);
            if (items != null)
            {
                int numLimit = 0;
                foreach (var item in items)
                {
                    if (numLimit >= LoadLimit)
                        break;
                    Product product = new Product
                    {
                        Title = GetTitle(item),
                        Price = GetPrice(item),
                        Link = GetLink(item),
                        Image = GetImage(item),
                        Currency = "Rub",
                    };
                    products.Add(product);
                    numLimit++;
                }
            }
            return products;
        }

        public override string GetDescription(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public override string GetImage(HtmlNode item)
        {
            var image = item.SelectSingleNode("./div[@class='product-inner']/div[@class='thumb']/meta[@itemprop='image']");
            if (image != null)
            {
                return image.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='a0s9']/div[@class='item a1d a1r3']/a");
            if (popularity != null)
            {
                return popularity.InnerText.Replace("отзывов", "").Replace(" ", "");
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='product-inner']/div[@class='info']/span[@class='price price_compare']/meta[@itemprop='price']");
            if (price != null)
            {
                return price.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public async override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            string pageUrl = GetPageUrl(page, searchQuery);

            string pageData = await ReadHtml(pageUrl, Encoding.UTF8);

            
            if (pageData != string.Empty)
            {
                _doc.LoadHtml(pageData);
                return _doc.DocumentNode.SelectNodes("//li[@class='product-item product-item_sizes ']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@class='a0s9']/div[@class='item a1d a1r3']/div[@class='_3OTE k0ds _2Ji0']/div[@class='_3xol']");
            if (rating != null)
            {
                double ratingParsed = double.Parse(rating.Attributes["style"].Value.Replace("width:", "").Replace("%;", "").Substring(0, 2));
                return (ratingParsed / 20).ToString();
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./div[@class='product-inner']/div[@class='info']/a");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./meta[@itemprop='name']");
            if (titleInfo != null)
            {
                return HttpUtility.HtmlDecode(titleInfo.Attributes["content"].Value);
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

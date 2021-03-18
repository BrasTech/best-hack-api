using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Sites.Food
{
    public class AzbukaVkusaSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Азбука вкуса";
        public override string Name { get => name; set => name = value; }

        private string description = "Сеть продовольственных супермаркетов";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://av.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 1 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://12.img.avito.st/432x324/5069759412.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/?text=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public AzbukaVkusaSite(int loadLimit)
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
                        Image = GetImage(item),
                        Link = GetLink(item),
                        CommentsCount = GetPopularity(item),
                        Rating = GetRating(item),
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
            var image = item.SelectSingleNode("./div[@itemprop='itemListElement']/a[@class='b-product__photo-wrap js-list-prod-open']/img");
            if (image != null)
            {
                return image.Attributes["data-lazy-src"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@itemprop='itemListElement']/div[@class='b-product__info']/div[@class='b-product__rating']/span[@class='b-product__rating-count']");
            if (popularity != null)
            {
                return popularity.InnerText;
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@itemprop='itemListElement']");
            if (price != null)
            {
                return price.Attributes["data-unit-price"].Value.Replace(".", ",");
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
                return _doc.DocumentNode.SelectNodes("//div[@class='b-grid__item']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@itemprop='itemListElement']/div[@class='b-product__info']/div[@class='b-product__rating']/div[@class='b-stars  b-product__rating-stars']");
            if (rating != null)
            {
                return rating.Attributes["data-value"].Value.Replace(".", ",");
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./div[@itemprop='itemListElement']/a[@class='b-product__photo-wrap js-list-prod-open']");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {

            var titleInfo = item.SelectSingleNode("./div[@itemprop='itemListElement']");
            if (titleInfo != null)
            {
                return titleInfo.Attributes["data-name"].Value;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

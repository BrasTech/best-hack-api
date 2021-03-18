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
    public class UtkonosSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Утконос";
        public override string Name { get => name; set => name = value; }

        private string description = "Онлайн-гипермаркет, предоставляющий услугу доставки продуктов на дом";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://www.utkonos.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 1,2,14,18,21,27 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://jobowork.net/files/img/utkonos-internet-gipermarket.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public UtkonosSite(int loadLimit)
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
            var image = item.SelectSingleNode("./div[@class='ng-star-inserted']/meta[@itemprop='image']");
            if (image != null)
            {
                return image.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='ng-star-inserted']/span/meta[@itemprop='reviewCount']");
            if (popularity != null)
            {
                return popularity.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='ng-star-inserted']/span/meta[@itemprop='price']");
            if (price != null)
            {
                return price.Attributes["content"].Value.Replace('.', ',');
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
                return _doc.DocumentNode.SelectNodes("//div[@class='catalog-grid__item ng-star-inserted']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@class='ng-star-inserted']/span/meta[@itemprop='ratingValue']");
            if (rating != null)
            {
                return rating.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./div[@class='ng-star-inserted']/meta[@itemprop='url']");
            if (link != null)
            {
                return link.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {

            var titleInfo = item.SelectSingleNode("./div[@class='ng-star-inserted']/meta[@itemprop='name']");
            if (titleInfo != null)
            {
                return titleInfo.Attributes["content"].Value;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

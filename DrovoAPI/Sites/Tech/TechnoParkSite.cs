using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Sites
{
    public class TechnoParkSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Технопарк";
        public override string Name { get => name; set => name = value; }

        private string description = "Интернет - магазин бытовой техники и электроники";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://www.technopark.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 3, 22 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "http://www.garant.ru/files/3/0/1410803/117_2.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/?q=REQQUERY&p=PAGENUM";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public TechnoParkSite(int loadLimit)
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
            var imageInfo = item.SelectSingleNode("./div[@class='card-listing__body']/div[@class='card-listing__image js-analytics-card--image']/img");
            if (imageInfo != null)
            {
                return imageInfo.Attributes["data-src"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='card-listing__aside']/div[@class='card-listing__payment']/div[@class='card-listing__prices']/div[@class='card-listing__price']/span");
            if (price != null)
                return price.InnerText.Replace(" ₽", "").Replace("&nbsp;", "").Replace("\n\t", "");
            return string.Empty;
        }

        public async override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            string pageUrl = GetPageUrl(page, searchQuery);

            string pageData = await ReadHtml(pageUrl, Encoding.UTF8);

            if (pageData != string.Empty)
            {
                _doc.LoadHtml(pageData);
                return _doc.DocumentNode.SelectNodes("//div[@class='listing__products-list listing__products-list--big js-products-list js-data--listing js-analytics-list']/article[@class='card-listing card-listing--big card-with-labels js-card-listing js-analytics-card  ']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            throw new NotImplementedException();
        }

        public override string GetLink(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='card-listing__body']/a[@class='card-listing__overlay-link js-analytics-card--link']");
            if (titleInfo != null)
            {
                return Domain + titleInfo.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {

            var titleInfo = item.SelectSingleNode("./div[@class='card-listing__body']/a[@class='card-listing__overlay-link js-analytics-card--link']");
            System.Diagnostics.Debug.WriteLine(titleInfo.InnerHtml);
            if (titleInfo != null)
            {
                return titleInfo.Attributes["title"].Value;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

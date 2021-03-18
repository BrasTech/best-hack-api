using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Sites.Other
{
    public class OzonSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Ozon";
        public override string Name { get => name; set => name = value; }

        private string description = "Старейший российский универсальный интернет-магазин";
        public override string Description { get => description; set => description = value; }


        private string domain = "https://www.ozon.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://is4-ssl.mzstatic.com/image/thumb/Purple124/v4/8f/5f/76/8f5f7604-0b64-61f6-ce26-cf533628914b/AppIcon-0-1x_U007emarketing-0-0-GLES2_U002c0-512MB-sRGB-0-0-0-85-220-0-0-0-7.png/320x0w.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/?from_global=true&text=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public OzonSite(int loadLimit)
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
            var image = item.SelectSingleNode("./a/div[@class='a0i4']/div[@class='a0i7']/img");
            if (image != null)
            {
                return image.Attributes["src"].Value;
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
            HtmlNode price = item.SelectSingleNode("./div[@class='a0s9']/a[@class='a0y9 tile-hover-target']/div[@class='b5v4 a5d2 item']/span[@class='b5v6 b5v7 c4v8']");
            if (price != null)
            {
                return price.InnerText.Replace(".", ",").Replace("₽", "").Replace(" ", "");
            }
            else
            {
                price = item.SelectSingleNode("./div[@class='a0s9']/a[@class='a0y9 tile-hover-target']/div[@class='b5v4 a5d2 item']/span[@class='b5v9 b5v7']");
                if (price != null)
                    return price.InnerText.Replace(".", ",").Replace("₽", "").Replace(" ", "");
                else
                {
                    price = item.SelectSingleNode("./div[@class='a0s9']/a[@class='a0y9 tile-hover-target']/div[@class='b5v4 a5d2 item']/span[@class='b5v6 b5v7']");

                    if (price != null)
                        return price.InnerText.Replace(".", ",").Replace("₽", "").Replace(" ", "");

                }
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
                return _doc.DocumentNode.SelectNodes("//div[@class='a0c4']");
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
            var link = item.SelectSingleNode("./a");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='a0s9']/a[@class='a2g0 tile-hover-target']");
            if (titleInfo != null)
            {
                return titleInfo.InnerText;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

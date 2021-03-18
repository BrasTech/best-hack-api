using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Sites
{
    public class EldoradoSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Эьдорадо";
        public override string Name { get => name; set => name = value; }

        private string description = "Торговая сеть по продаже бытовой электроники";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://www.eldorado.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 3, 22 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://cdn.mapix.ru/img/companies/logo/eldorado.png";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/catalog.php?q=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 18;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public EldoradoSite(int loadLimit)
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
                        Rating = GetRating(item),
                        CommentsCount = GetPopularity(item),
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
            var imageInfo = item.SelectSingleNode("./div[@class='AkWZIIC']/a[@class='_1RaAPF1']/img");
            if (imageInfo != null)
            {
                return imageInfo.Attributes["src"].Value;
            }
            return string.Empty;
        }

        public override string GetRating(HtmlNode item)
        {
            var stars = item.SelectNodes("./div[@class='_39MI3A8']/div[@class='_2r9Xg_l']/div[@class='_14nj2BP']/span[@class='_2MOOgn5']/span[@class='tevqf5-0 cbJQML']/span[@class='tevqf5-2 fBryir']");
            if (stars != null)
            {
                return stars.Count.ToString();
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='_39MI3A8' and 2]/div[2]/div[@class='_3Fsz1sA _1xiGUPl' and 2]/span[@class='Q35hFri hlL-W8I' and 1]");
            if (price != null)
                return price.InnerText.Replace(" р.", "").Replace(" ", "");
            return string.Empty;
        }

        public async override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            string pageUrl = GetPageUrl(page, searchQuery);
            string pageData = await ReadHtml(pageUrl, Encoding.UTF8);

            //File.WriteAllText(@"site.txt", pageData);

            if (pageData != string.Empty)
            {
                _doc.LoadHtml(pageData);
                return _doc.DocumentNode.SelectNodes("//li[@class='_3uUsGGA']");
            }
            return null;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='_39MI3A8' and 2]/div[@class='_2r9Xg_l' and 1]/div[1]/span[@class='_2MOOgn5' and 1]/a[1]");
            if (popularity != null)
                return popularity.InnerText.Replace(" ", "").Replace("отзывов", "").Replace("отзыва", "");
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='_39MI3A8']/div[@class='_2r9Xg_l']/div[@class='_2fFxlhy']/a");
            if (titleInfo != null)
            {
                return titleInfo.InnerText;
            }
            return string.Empty;

        }
        public override string GetLink(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='_39MI3A8']/div[@class='_2r9Xg_l']/div[@class='_2fFxlhy']/a");
            if (titleInfo != null)
            {
                return Domain + titleInfo.Attributes["href"].Value;
            }
            return string.Empty;
        }
        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

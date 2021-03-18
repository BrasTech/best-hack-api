using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrovoAPI.Sites.Clothes
{
    public class WildberriesSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "WildBerries";
        public override string Name { get => name; set => name = value; }

        private string description = "Коллекции женской, мужской и детской одежды, обуви, а также товары для дома и спорта";
        public override string Description { get => description; set => description = value; }


        private string domain = "https://www.wildberries.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = {2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://55.img.avito.st/208x156/7991477755.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/catalog/0/search.aspx?search=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public WildberriesSite(int loadLimit)
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
            var image = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/div[@class='l_class']/img");
            if (image != null)
            {
                return image.Attributes["src"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/span[@itemtype='http://schema.org/Intangible']/span/span[@class='dtList-comments-count c-text-sm']");
            if (popularity != null)
            {
                return popularity.InnerText;
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            HtmlNode price = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/div[@class='dtlist-inner-brand']/div[@class='j-cataloger-price']/span[@class='price']/ins[@class='lower-price']");
            if (price != null)
            {
                return price.InnerText.Replace(".", ",").Replace("₽", "").Replace(" ", "");
            }
            else
            {
                price = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/div[@class='dtlist-inner-brand']/div[@class='j-cataloger-price']/span[@class='price']/ins[@class='price-old-block']/del");
                if (price != null)
                {
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
                return _doc.DocumentNode.SelectNodes("//div[@class='dtList i-dtList j-card-item ']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/span[@itemtype='http://schema.org/Intangible']/span/span[1]");
            if (rating != null)
            {
                return rating.Attributes["class"].Value.Substring(rating.Attributes["class"].Value.Length - 1);
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {

            var titleInfo = item.SelectSingleNode("./div[@class='dtList-inner']/span/span/span/a/div[@class='dtlist-inner-brand']/div[@class='dtlist-inner-brand-name']");
            if (titleInfo != null)
            {
                string brand = titleInfo.SelectSingleNode("./strong").InnerText;
                string name = titleInfo.SelectSingleNode("./span").InnerText;
                return brand + name;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

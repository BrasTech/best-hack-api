using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DrovoAPI.Sites.Other
{
    public class OnlineTradeSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "OnlineTrade";
        public override string Name { get => name; set => name = value; }

        private string description = "Интернет-магазин товаров";
        public override string Description { get => description; set => description = value; }


        private string domain = "https://www.onlinetrade.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 2,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://ru.inglesina.com/wordpress/russia/wp-content/uploads/sites/6/2017/07/onlinetrade_ru_low.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/sitesearch.html?query=REQQUERY&archive=0&force_items=1";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public OnlineTradeSite(int loadLimit)
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
            var image = item.SelectSingleNode("./a[@class='indexGoods__item__image']/img");
            if (image != null)
            {
                return image.Attributes["src"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='indexGoods__item__stars']/a");
            if (popularity != null)
            {
                return popularity.InnerText;
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='indexGoods__item__dataCover']/div[@class='indexGoods__item__price']/span[@class='price regular']");
            if (price != null)
            {
                return price.InnerText.Replace("&#8381;", "").Replace(" ₽", "").Replace(" ", "");
            }
            return string.Empty;
        }

        public async override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            string pageUrl = GetPageUrl(page, searchQuery);

            string pageData = await ReadHtml(pageUrl, Encoding.GetEncoding(1251));

            
            if (pageData != string.Empty)
            {
                _doc.LoadHtml(pageData);
                return _doc.DocumentNode.SelectNodes("//div[@class='indexGoods__item']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@class='indexGoods__item__stars']/a/div");
            if (rating != null)
            {
                if (rating.Attributes["class"].Value.Replace("starsSVG", "").Contains("stars"))
                    return rating.Attributes["class"].Value.Substring(rating.Attributes["class"].Value.Length - 1);
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./a[@class='indexGoods__item__image']");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='indexGoods__item__descriptionCover']/a");
            if (titleInfo != null)
            {
                return HttpUtility.HtmlDecode(titleInfo.InnerText);
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            query = HttpUtility.UrlEncode(query, Encoding.GetEncoding(1251));
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

        public static string Converter(string value, Encoding src, Encoding trg) //функция
        {
            //throw new System.NotImplementedException();
            Decoder dec = src.GetDecoder();
            byte[] ba = trg.GetBytes(value);
            int len = dec.GetCharCount(ba, 0, ba.Length);
            char[] ca = new char[len];
            dec.GetChars(ba, 0, ba.Length, ca, 0);
            return new string(ca);
        }

    }
}

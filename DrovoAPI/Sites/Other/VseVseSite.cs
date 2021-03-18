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
    public class VseVseSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Всё-Всё";
        public override string Name { get => name; set => name = value; }

        private string description = "Интернет-гипермаркет ВсеВсе.ру – онлайн площадка созданная в 2016 году, специализирующаяся на электронной коммерции";
        public override string Description { get => description; set => description = value; }


        private string domain = "https://www.vsevse.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = {2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://salesexpress.ru/wp-content/uploads/awpcp/images/i-f9c5eb10-large.jpg";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/catalog/?q=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public VseVseSite(int loadLimit)
        {
            _doc = new HtmlDocument();
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
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
            var image = item.SelectSingleNode("./div[@class='catalog_item_wrapp catalog_item item_wrap main_item_wrapper  product_image ']/div[@class='inner_wrap TYPE_1']/div[@class='image_wrapper_block']/a/span[@class='section-gallery-wrapper flexbox']/span[@class='section-gallery-wrapper__item _active']/img");
            if (image != null)
            {
                return Domain + image.Attributes["src"].Value;
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
            var price = item.SelectSingleNode("./div[@class='catalog_item_wrapp catalog_item item_wrap main_item_wrapper  product_image ']/div[@class='inner_wrap TYPE_1']/div[@class='item_info']/div[@class='cost prices clearfix']/div[@class='price_matrix_wrapper ']/div[@class='price font-bold font_mxs']/span/span[@class='price_value']");
            if (price != null)
            {
                return price.InnerText.Replace("&nbsp;", "").Replace(" ₽", "").Replace(" ", "");
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
                return _doc.DocumentNode.SelectNodes("//div[@class='col-lg-3 col-md-4 col-sm-6 col-xs-6 col-xxs-12 item item-parent item_block ']");
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
            var link = item.SelectSingleNode("./div[@class='catalog_item_wrapp catalog_item item_wrap main_item_wrapper  product_image ']/div[@class='inner_wrap TYPE_1']/div[@class='image_wrapper_block']/a");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='catalog_item_wrapp catalog_item item_wrap main_item_wrapper  product_image ']/div[@class='inner_wrap TYPE_1']/div[@class='image_wrapper_block']/a/span[@class='section-gallery-wrapper flexbox']/span[@class='section-gallery-wrapper__item _active']/img[@class='lazy img-responsive']");
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

using DrovoAPI.Classes;
using DrovoAPI.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DrovoAPI.Sites
{
    public class BeeLineSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "Билайн";
        public override string Name { get => name; set => name = value; }

        private string description = "Магазин электроники";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://moskva.beeline.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 3, 22 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://static.beeline.ru/upload/images/b2c/bee-logo/single.png";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/shop/search/?query=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 18;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public BeeLineSite(int loadLimit)
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
            var imageInfo = item.SelectSingleNode("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns-fixed_1mVg']/a[@class='ThumbHolder_component_cm5e ProductThumb_thumb_3JdX ProductThumb_thumb-180x200_2lcT LinkBlock_link_1Y4C']/picture/img");
            if (imageInfo != null)
            {
                return imageInfo.Attributes["data-src"].Value;
            }
            return string.Empty;
        }

        public override string GetRating(HtmlNode item)
        {
            var stars = item.SelectNodes("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns_3lFI ProductCard_desc_1hUr']/div[@class='FeedbackWidgetLayout_component_13Ks styles_marb-xs_2fFu']/a[@class='FeedbackWidgetLayout_rateWrapper_1Urp']/div[@class='container']/svg[@class='FiveStarRate_star_1-eP']/path");
            if (stars != null)
            {
                double rating = 0;
                foreach(var star in stars)
                {
                    string starD = star.Attributes["d"].Value;
                    switch (starD)
                    {
                        // 1 full star
                        case "M9.872.542l2.027 4.11c.14.286.415.485.73.53l4.536.66c.8.116 1.116 1.095.54 1.657l-3.282 3.2c-.23.223-.335.543-.28.86l.775 4.516c.134.794-.697 1.4-1.412 1.024l-4.055-2.13c-.285-.15-.623-.15-.906 0L4.492 17.1c-.713.375-1.548-.23-1.41-1.024l.773-4.516c.054-.317-.05-.637-.28-.86L.296 7.5c-.58-.563-.258-1.542.54-1.66l4.534-.658c.315-.045.59-.244.73-.53L8.127.54c.356-.722 1.388-.722 1.744 0":
                            rating += 1;
                            break;
                            // 0,5 star
                        case "M13.168 9.41c-.654.637-.953 1.554-.798 2.454l.492 2.863-2.573-1.35c-.397-.21-.84-.322-1.29-.322V2.845l1.286 2.603c.405.818 1.184 1.385 2.088 1.518l2.874.416-2.08 2.028zm4.537-1.91c.578-.563.26-1.542-.538-1.657l-4.534-.66c-.32-.046-.59-.244-.733-.532L9.872.543C9.694.18 9.346 0 9.002 0c-.35 0-.696.18-.874.542L6.1 4.652c-.14.287-.415.485-.732.53l-4.534.66C.037 5.96-.282 6.938.296 7.5l3.28 3.2c.23.222.334.544.28.86l-.776 4.516c-.027.156-.014.306.026.442.122.41.51.697.934.697.15 0 .302-.038.452-.115l4.057-2.13c.14-.077.296-.115.45-.115.155 0 .31.038.452.114l4.056 2.13c.15.077.304.115.453.115.567 0 1.067-.51.96-1.14l-.775-4.516c-.054-.316.05-.638.28-.86l3.28-3.2z":
                            rating += 0.5;
                            break;
                    }
                }
                return rating.ToString();
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns-fixed_1mVg']/div[@class='component styles_marb-xs_2fFu']/div[@class='Heading_component_3cH8 Heading_h3_3ckk']/div[@class='InlineSet_container_3CPR']/div[@class='InlineSet_item_i47B']");
            if (price != null)
                return price.InnerText.Replace("  ₽", "").Replace(" ","");
            return string.Empty;
        }

        public async override Task<HtmlNodeCollection> GetProducts(int page, string searchQuery)
        {
            string pageUrl = GetPageUrl(page, searchQuery);
            string pageData = await ReadHtml(pageUrl, Encoding.UTF8);

            if (pageData != string.Empty)
            {
                _doc.LoadHtml(pageData);
                return _doc.DocumentNode.SelectNodes("//div[@class='ProductCard_component_1b9Y']");
            }
            return null;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns_3lFI ProductCard_desc_1hUr']/div[@class='FeedbackWidgetLayout_component_13Ks styles_marb-xs_2fFu']/div[@class='FeedbackWidgetLayout_sumAction_loFj']/a[@class='FeedbackWidgetLayout_dotted__3RB']");
            if (popularity != null)
                return popularity.InnerText.Replace(" ", "").Replace("отзывов", "");
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns_3lFI ProductCard_desc_1hUr']/div[@class='ProductCard_header_U0M_']/a");
            if (titleInfo != null)
            {
                return titleInfo.InnerText;
            }
            return string.Empty;

        }
        public override string GetLink(HtmlNode item)
        {
            var titleInfo = item.SelectSingleNode("./div[@class='Grid_container_15R4 Grid_gutterMedium_3rkN Grid_nowrap_2d2d']/div[@class='Grid_columns-fixed_1mVg']/a[@class='ThumbHolder_component_cm5e ProductThumb_thumb_3JdX ProductThumb_thumb-180x200_2lcT LinkBlock_link_1Y4C']");
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

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
    public class VkusvillSite : Site
    {
        private HtmlDocument _doc;

        private int id = 1;
        public override int Id { get => id; set => id = value; }

        private string name = "ВкусВилл";
        public override string Name { get => name; set => name = value; }

        private string description = "Продукты для здорового питания";
        public override string Description { get => description; set => description = value; }

        private string domain = "https://vkusvill.ru";
        public override string Domain { get => domain; set => domain = value; }

        private int[] categoryIds = { 1,2,7,14,18,27,32,35,39 };
        public override int[] CategoryIds { get => categoryIds; set => categoryIds = value; }

        private string image = "https://cdn.mapix.ru/img/companies/logo/vkusvill.png";
        public override string Image { get => image; set => image = value; }

        private string searchString = "/search/?q=REQQUERY";
        public override string SearchString { get => searchString; set => searchString = value; }

        private int itemsPerPage = 11;
        public override int ItemsPerPage { get => itemsPerPage; set => itemsPerPage = value; }

        public int LoadLimit = 0;

        public VkusvillSite(int loadLimit)
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
                        //CommentsCount = GetPopularity(item),
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
            var image = item.SelectSingleNode("./div[@class='Slider__itemInner']/div[@class='ProductCard js-product-cart js-datalayer-catalog-list-item']/div[@class='ProductCard__image']/div[@class='ProductCard__imageInner']/a/img");
            if (image != null)
            {
                return image.Attributes["data-src"].Value;
            }
            return string.Empty;
        }

        public override string GetPopularity(HtmlNode item)
        {
            var popularity = item.SelectSingleNode("./div[@class='ng-star-inserted']/span/meta[@itemprop='reviewCount']");
            if (popularity != null)
            {
                // return popularity.Attributes["content"].Value;
            }
            return string.Empty;
        }

        public override string GetPrice(HtmlNode item)
        {
            var price = item.SelectSingleNode("./div[@class='Slider__itemInner']/div[@class='ProductCard js-product-cart js-datalayer-catalog-list-item']/div[@class='ProductCard__content']/div[@class='ProductCard__price']/span[@class='Price']/span[@class='Price__value']");
            if (price != null)
            {
                return price.InnerText;
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
                return _doc.DocumentNode.SelectNodes("//div[@class='ProductCards__item ProductCards__item--col-lg-1-3']");
            }
            return null;
        }

        public override string GetRating(HtmlNode item)
        {
            var rating = item.SelectSingleNode("./div[@class='Slider__itemInner']/div[@class='ProductCard js-product-cart js-datalayer-catalog-list-item']/div[@class='ProductCard__image']/div[@class='ProductCard__rating']/div[@class='Rating']/div[@class='Rating__text']");
            if (rating != null)
            {
                return rating.InnerText.Replace('.', ',').Replace(" ", "");
            }
            return string.Empty;
        }

        public override string GetLink(HtmlNode item)
        {
            var link = item.SelectSingleNode("./div[@class='Slider__itemInner']/div[@class='ProductCard js-product-cart js-datalayer-catalog-list-item']/div[@class='ProductCard__image']/div[@class='ProductCard__imageInner']/a");
            if (link != null)
            {
                return Domain + link.Attributes["href"].Value;
            }
            return string.Empty;
        }

        public override string GetTitle(HtmlNode item)
        {

            var titleInfo = item.SelectSingleNode("./div[@class='Slider__itemInner']/div[@class='ProductCard js-product-cart js-datalayer-catalog-list-item']/div[@class='ProductCard__image']/div[@class='ProductCard__imageInner']/a/img");
            if (titleInfo != null)
            {
                return titleInfo.Attributes["alt"].Value;
            }
            return string.Empty;

        }

        private string GetPageUrl(int page, string query)
        {
            return Domain + searchString.Replace("REQQUERY", query).Replace("PAGENUM", page.ToString());
        }

    }
}

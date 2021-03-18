using DrovoAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DrovoAPI.Classes
{
    public class Product
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Currency { get; set; } = "rub";
        
        public string Link { get; set; } = "";
        public string Image { get; set; } = "https://semantic-ui.com/images/wireframe/image.png";

        private int commentsCount = 0;
        public string CommentsCount
        {
            get
            {
                return commentsCount.ToString();
            }
            set
            {
                if (int.TryParse(value, out int _commentsCount))
                {
                    commentsCount = _commentsCount;
                }
                else
                    commentsCount = 0;
            }
        }
        public int CommentsCountValue
        {
            get { return commentsCount; }
        }

        private double price = 0;
        public string Price
        {
            get { return price.ToString(); }
            set
            {
                if (double.TryParse(value, out double _price)){
                    price = _price;
                }
                else
                    price = 0;
            }
        }
        public double PriceValue
        {
            get { return price; }
        }

        private double rating = 0;
        public string Rating
        {
            get { return rating.ToString(); }
            set
            {
                if (double.TryParse(value, out double _rating))
                {
                    rating = _rating;
                }
                else
                    rating = 0;
            }
        }
        public double RatingValue
        {
            get { return rating; }
        }

        private double popularity = 0;
        public string Popularity
        {
            get { return popularity.ToString(); }
            set
            {
                if (double.TryParse(value, out double _popularity))
                {
                    popularity = _popularity;
                }
                else
                    popularity = 0;
            }
        }
        public double PopularityValue
        {
            get { return popularity; }
            set { popularity = value; }
        }
    }
}

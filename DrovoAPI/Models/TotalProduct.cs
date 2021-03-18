using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Models
{
    public class TotalProduct
    {
        public string Name { get; set; }
        public double Rating { get; set; }
        public double Popularity { get; set; }
        public double AveragePrice { get; set; }
        public string Logo { get; set; }
        public List<TotalMarket> Markets { get; set; }
    }
}

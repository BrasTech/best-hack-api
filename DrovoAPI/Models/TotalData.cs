using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.Models
{
    public class TotalData
    {
        public string Category { get; set; }
        public int Total { get; set; }
        public List<TotalProduct> List { get; set; }
    }
}

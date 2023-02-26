using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameSwap.Models
{
    public class PostalCode
    {
        public string Code { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSearch.Tests
{
    public class Airport
    {
        public int id { get; set; }
        public string ident { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string latitude_deg { get; set; }
        public string longitude_deg { get; set; }
        public string elevation_ft { get; set; }
        public string continent { get; set; }
        public string iso_country { get; set; }
        public string iso_region { get; set; }
        public string municipality { get; set; }
        public string iata_code { get; set; }
        public string local_code { get; set; }
    }
}

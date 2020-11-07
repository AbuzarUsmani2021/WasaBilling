﻿using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class ZoneData
    {
        public int ID { get; set; }
        public int TID { get; set; }
        public int CityID { get; set; }
        
        public int ZoneID { get; set; }
        public string Name { get; set; }
        public string CityName { get; set; }
        public string ZoneName { get; set; }
        public int? ClientId { get; set; }
        public string SapNo { get; set; }
        public string ClientName { get; set; }
        public string ShortCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Region> Regions { get; set; }

    }
}
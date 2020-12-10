using FOS.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{
    public class FaultTypeData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> FaultTypes { get; set; }

    }
}

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
    public class DropdownData
    {
        //Fault Type Table
        public int FId { get; set; }
        public string FaulttypeName { get; set; }
        //Fault Type Table


        //Fault type Detail table
        public int FaultyTypeDetailID { get; set; }
        public string FaultyTypeDetailName { get; set; }
        public Nullable<int> FaulttypeID { get; set; }
       
        //Fault type Detail table

        //Progress Status table
        public int ProgressStatusID { get; set; }
        public string ProgressStatusName { get; set; }
        public int ProgressStatusFaultTypeID { get; set; }

        //Progress Status table




        //Work Done Table Start
        public int WorkDoneID { get; set; }
        public string WorkDoneName { get; set; }
        public int? WorkDoneFaulttypeID { get; set; }
        public string WorkDoneFaulttypeName { get; set; }


        //Work Done Table END














    }




}


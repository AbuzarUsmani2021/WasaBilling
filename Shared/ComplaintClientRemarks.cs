using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOS.Shared
{

    using System;
    using System.Collections.Generic;
    public class ComplaintClientRemarks
    {


        public int ID { get; set; }
        public Nullable<int> ComplaintID { get; set; }
        public byte[] WasaRemarks { get; set; }
        public Nullable<System.DateTime> RemarksDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> Isdeleted { get; set; }
        public string ClientRemarks { get; set; }
        public Nullable<int> RemarksBy { get; set; }
        public string RemarksByName { get; set; }
        public string DateTime { get; set; }


    }
}

using System;

namespace FOS.Shared
{
    public class Workdone
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> FaulttypeID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> LaunchedDate { get; set; }
    }
}
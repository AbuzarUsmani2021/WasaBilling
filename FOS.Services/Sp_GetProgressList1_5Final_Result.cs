//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FOS.DataLayer
{
    using System;
    
    public partial class Sp_GetProgressList1_5Final_Result
    {
        public Nullable<int> ID { get; set; }
        public Nullable<int> ComplaintID { get; set; }
        public string ComplaintNo { get; set; }
        public Nullable<System.DateTime> LaunchDate { get; set; }
        public string InitialRemarks { get; set; }
        public Nullable<int> IsPublished { get; set; }
        public string SiteName { get; set; }
        public string FaultType { get; set; }
        public string FaultTypeDetail { get; set; }
        public string SiteCode { get; set; }
        public Nullable<System.DateTime> JobDate { get; set; }
        public Nullable<int> ProgressStatusID { get; set; }
        public string ProgressStatusName { get; set; }
        public string PRemarks { get; set; }
        public string SaleofficerName { get; set; }
        public string LaunchedByName { get; set; }
        public Nullable<int> ChildFaultType { get; set; }
        public Nullable<int> ChildFaultTypeDetailID { get; set; }
        public Nullable<int> ChildComplaintStatusID { get; set; }
        public string ChildFaulttypeName { get; set; }
        public string ChildFaultTypeDetailName { get; set; }
        public string ChildComplaintStatus { get; set; }
    }
}

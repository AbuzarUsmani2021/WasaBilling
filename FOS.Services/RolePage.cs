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
    using System.Collections.Generic;
    
    public partial class RolePage
    {
        public int RoleID { get; set; }
        public int PageID { get; set; }
        public Nullable<int> Access { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public Nullable<bool> Read { get; set; }
        public Nullable<bool> Write { get; set; }
        public Nullable<bool> Update { get; set; }
        public Nullable<bool> Delete { get; set; }
        public Nullable<bool> Status { get; set; }
    
        public virtual Page Page { get; set; }
        public virtual Role Role { get; set; }
    }
}

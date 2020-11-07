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
    
    public partial class Area
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Area()
        {
            this.DealerCityAreas = new HashSet<DealerCityArea>();
            this.Retailers = new HashSet<Retailer>();
            this.SubDivisions = new HashSet<SubDivision>();
            this.SaleOfficers = new HashSet<SaleOfficer>();
            this.JobsDetails = new HashSet<JobsDetail>();
        }
    
        public int TID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public string ShortCode { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
    
        public virtual City City { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DealerCityArea> DealerCityAreas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Retailer> Retailers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SubDivision> SubDivisions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SaleOfficer> SaleOfficers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JobsDetail> JobsDetails { get; set; }
    }
}

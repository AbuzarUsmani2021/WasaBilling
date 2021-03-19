using FOS.DataLayer;
using FOS.Shared;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class VisitRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(VisitRegistrationRequest rm)
        {
            TBL_KsbVisits retailerObj = new TBL_KsbVisits();
            try
            {
                if (rm.VisitTypeId == 1)
                {
                    var siteID = rm.SitesLists;
                    //ADD New Retailer 
                    retailerObj.ID = db.TBL_KsbVisits.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    retailerObj.SiteID = Convert.ToInt32(siteID);
                    retailerObj.TimeInHours = rm.Time;

                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.VisitTypeID = rm.VisitTypeId;
                    retailerObj.LaunchDate = DateTime.UtcNow.AddHours(5);
                    retailerObj.Datefrom = Convert.ToDateTime(rm.DateFrom);
                    retailerObj.Dateto = Convert.ToDateTime(rm.DateTo);
                    retailerObj.IsActive = true;
                    retailerObj.IsDeleted = false;

                    if (rm.Picture2 == "" || rm.Picture2 == null)
                    {
                        retailerObj.Picture2 = null;
                    }
                    else
                    {
                        retailerObj.Picture2 = ConvertIntoByte(rm.Picture2, "KSBVisits", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "VisitImages");
                    }

                    db.TBL_KsbVisits.Add(retailerObj);
                    db.SaveChanges();



                    string[] statusesList = rm.SiteStatuses.Split(',');

                    if (statusesList != null)
                    {

                        foreach (var item in statusesList)
                        {
                            AddSiteStatu Ac = new AddSiteStatu();

                            Ac.KSbVisitID = retailerObj.ID;
                            Ac.SiteID = Convert.ToInt32(siteID); 
                            Ac.SiteStatusID = Convert.ToInt32(item);

                            db.AddSiteStatus.Add(Ac);
                            db.SaveChanges();

                        }
                    }

                    string[] PurposeOfVisits = rm.PurposeOfVisits.Split(',');


                    if (PurposeOfVisits != null)
                    {

                        foreach (var item in PurposeOfVisits)
                        {
                            AddPurposeOfVisit Ac = new AddPurposeOfVisit();

                            Ac.KSBVisitID = retailerObj.ID;
                            Ac.SiteID = Convert.ToInt32(siteID);
                            Ac.VisitPurposeID = Convert.ToInt32(item);
                            Ac.LaunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddPurposeOfVisits.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                    string[] StaffLists = rm.StaffLists.Split(',');


                    if (StaffLists != null)
                    {


                        foreach (var item in StaffLists)
                        {
                            AddStaffList Ac = new AddStaffList();

                            Ac.KSBVisitID = retailerObj.ID;
                            Ac.SiteID = Convert.ToInt32(siteID);
                            Ac.StaffID = Convert.ToInt32(item);
                            Ac.LAunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddStaffLists.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                }


               else if (rm.VisitTypeId == 2)
                {

                    //ADD New Retailer 
                    retailerObj.ID = db.TBL_KsbVisits.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    //retailerObj.SiteID = rm.SiteId;
                    retailerObj.TimeInHours = rm.Time;
                    retailerObj.IsActive = true;
                    retailerObj.IsDeleted = false;
                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.VisitTypeID = rm.VisitTypeId;
                    retailerObj.Datefrom = Convert.ToDateTime(rm.DateFrom);
                    retailerObj.Dateto = Convert.ToDateTime(rm.DateTo);
                    retailerObj.LaunchDate = DateTime.UtcNow.AddHours(5);

                    if (rm.Picture2 == "" || rm.Picture2 == null)
                    {
                        retailerObj.Picture2 = null;
                    }
                    else
                    {
                        retailerObj.Picture2 = ConvertIntoByte(rm.Picture2, "KSBVisits", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "VisitImages");
                    }

                    db.TBL_KsbVisits.Add(retailerObj);
                    db.SaveChanges();



                    string[] statusesList = rm.SiteStatuses.Split(',');

                    if (statusesList != null)
                    {

                        foreach (var item in statusesList)
                        {
                            AddSiteStatu Ac = new AddSiteStatu();

                            Ac.KSbVisitID = retailerObj.ID;
                            //Ac.SiteID = rm.SiteId;
                            Ac.SiteStatusID = Convert.ToInt32(item);

                            db.AddSiteStatus.Add(Ac);
                            db.SaveChanges();

                        }
                    }

                    string[] PurposeOfVisits = rm.PurposeOfVisits.Split(',');


                    if (PurposeOfVisits != null)
                    {

                        foreach (var item in PurposeOfVisits)
                        {
                            AddPurposeOfVisit Ac = new AddPurposeOfVisit();

                            Ac.KSBVisitID = retailerObj.ID;
                            //Ac.SiteID = rm.SiteId;
                            Ac.VisitPurposeID = Convert.ToInt32(item);
                            Ac.LaunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddPurposeOfVisits.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                    string[] StaffLists = rm.StaffLists.Split(',');


                    if (StaffLists != null)
                    {


                        foreach (var item in StaffLists)
                        {
                            AddStaffList Ac = new AddStaffList();

                            Ac.KSBVisitID = retailerObj.ID;
                           // Ac.SiteID = rm.SiteId;
                            Ac.StaffID = Convert.ToInt32(item);
                            Ac.LAunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddStaffLists.Add(Ac);
                            db.SaveChanges();

                        }

                    }

                    string[] SitesLists = rm.SitesLists.Split(',');


                    if (SitesLists != null)
                    {


                        foreach (var item in SitesLists)
                        {
                           MultipleSiteVisit Ac = new MultipleSiteVisit();

                            Ac.KSBVisitID = retailerObj.ID;
                           Ac.SiteID = Convert.ToInt32(item);
                            Ac.LaunchedAt = DateTime.UtcNow.AddHours(5);
                            db.MultipleSiteVisits.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                }

                else if (rm.VisitTypeId == 3)
                {

                    //ADD New Retailer 
                    retailerObj.ID = db.TBL_KsbVisits.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    //retailerObj.SiteID = rm.SiteId;
                    retailerObj.TimeInHours = rm.Time;
                    retailerObj.IsActive = true;
                    retailerObj.IsDeleted = false;
                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.VisitTypeID = rm.VisitTypeId;
                    retailerObj.Datefrom = Convert.ToDateTime(rm.DateFrom);
                    retailerObj.Dateto = Convert.ToDateTime(rm.DateTo);
                    retailerObj.LaunchDate = DateTime.UtcNow.AddHours(5);

                    if (rm.Picture2 == "" || rm.Picture2 == null)
                    {
                        retailerObj.Picture2 = null;
                    }
                    else
                    {
                        retailerObj.Picture2 = ConvertIntoByte(rm.Picture2, "KSBVisits", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "VisitImages");
                    }

                    db.TBL_KsbVisits.Add(retailerObj);
                    db.SaveChanges();



                    string[] statusesList = rm.SiteStatuses.Split(',');

                    if (statusesList != null)
                    {

                        foreach (var item in statusesList)
                        {
                            AddSiteStatu Ac = new AddSiteStatu();

                            Ac.KSbVisitID = retailerObj.ID;
                            //Ac.SiteID = rm.SiteId;
                            Ac.SiteStatusID = Convert.ToInt32(item);

                            db.AddSiteStatus.Add(Ac);
                            db.SaveChanges();

                        }
                    }

                    string[] PurposeOfVisits = rm.PurposeOfVisits.Split(',');


                    if (PurposeOfVisits != null)
                    {

                        foreach (var item in PurposeOfVisits)
                        {
                            AddPurposeOfVisit Ac = new AddPurposeOfVisit();

                            Ac.KSBVisitID = retailerObj.ID;
                           // Ac.SiteID = rm.SiteId;
                            Ac.VisitPurposeID = Convert.ToInt32(item);
                            Ac.LaunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddPurposeOfVisits.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                    string[] StaffLists = rm.StaffLists.Split(',');


                    if (StaffLists != null)
                    {


                        foreach (var item in StaffLists)
                        {
                            AddStaffList Ac = new AddStaffList();

                            Ac.KSBVisitID = retailerObj.ID;
                          //  Ac.SiteID = rm.SiteId;
                            Ac.StaffID = Convert.ToInt32(item);
                            Ac.LAunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddStaffLists.Add(Ac);
                            db.SaveChanges();

                        }

                    }

                    string[] SitesLists = rm.SitesLists.Split(',');


                    if (SitesLists != null)
                    {


                        foreach (var item in SitesLists)
                        {
                            AddMultipleComplaintVisit Ac = new AddMultipleComplaintVisit();

                            Ac.KSBVisitID = retailerObj.ID;
                            Ac.ComplaintID = Convert.ToInt32(item);
                            Ac.LaunchedAt = DateTime.UtcNow.AddHours(5);
                            db.AddMultipleComplaintVisits.Add(Ac);
                            db.SaveChanges();

                        }

                    }
                }

                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Visit Launched Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
               
             

            }
            catch (Exception ex)
            {
               
                Log.Instance.Error(ex, "Add Complaint API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint Registration API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex, 
                    ValidationErrors = null
              
            };
               

                

            }

         

        }

        public string ConvertIntoByte(string Base64, string DealerName, string SendDateTime, string folderName)
        {
            byte[] bytes = Convert.FromBase64String(Base64);
            MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length);
            ms.Write(bytes, 0, bytes.Length);
            Image image = Image.FromStream(ms, true);
            //string filestoragename = Guid.NewGuid().ToString() + UserName + ".jpg";
            string filestoragename = DealerName + SendDateTime;
            string outputPath = System.Web.HttpContext.Current.Server.MapPath(@"~/Images/" + folderName + "/" + filestoragename + ".jpg");
            image.Save(outputPath, ImageFormat.Jpeg);

            //string fileName = UserName + ".jpg";
            //string rootpath = Path.Combine(Server.MapPath("~/Photos/ProfilePhotos/"), Path.GetFileName(fileName));
            //System.IO.File.WriteAllBytes(rootpath, Convert.FromBase64String(Base64));
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }

       
     

        public class SuccessResponse
        {

        }
        public class VisitRegistrationRequest
        {
            public VisitRegistrationRequest()
            {
                
            }
            public int ID { get; set; }

            public int SiteId { get; set; }

            public int Time { get; set; }

            public int VisitTypeId { get; set; }
            public string Remarks { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public string SiteStatuses { get; set; }
            public string PurposeOfVisits { get; set; }
            public string StaffLists { get; set; }
            public string SitesLists { get; set; }
            public string Picture1 { get; set; }
            public string Picture2 { get; set; }



        }

      


    }
}
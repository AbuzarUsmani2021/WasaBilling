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
    public class ComplaintRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ComplaintRegistrationRequest rm)
        {
            Job retailerObj = new Job();
            try
            {
                var data = db.Retailers.Where(x => x.ID == rm.SiteId).FirstOrDefault();

                    var dateAndTime = DateTime.UtcNow.AddHours(5);
                    int year = dateAndTime.Year;
                    int month = dateAndTime.Month;
                    string finalMonth = month.ToString().PadLeft(2, '0');
                    int day = dateAndTime.Day;
                    string finalday = day.ToString().PadLeft(2, '0');
                    var datein = string.Format("{0}{1}{2}", year, finalMonth, finalday);
                    DateTime dtFromTodayUtc = DateTime.UtcNow.AddHours(5);
                    DateTime dtFromToday = dtFromTodayUtc.Date;
                    DateTime dtToToday = dtFromToday.AddDays(1);

                if (data.ZoneID == 7)
                {
                    var Id1 = data.ProjectCode;

                    var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                    if (counter == null)
                    {
                        var ticketCount = 1;
                        string s = ticketCount.ToString().PadLeft(3, '0');
                        retailerObj.TicketNo = datein + "-"+ Id1+ "-" + s;
                    }
                    else
                    {
                        var splittedcounter = counter.Split('-');
                        var val = splittedcounter[2];
                     int value = Convert.ToInt32(val) + 1;
                       string s = value.ToString().PadLeft(3, '0');
                        retailerObj.TicketNo = datein + "-" + Id1 + "-" + s;
                    }
                }
                else if (data.ZoneID == 8)
                {
                    var Id2 = data.ProjectCode;

                    var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                    if (counter == null)
                    {
                        var ticketCount = 1;
                        string s = ticketCount.ToString().PadLeft(3, '0');
                        retailerObj.TicketNo = datein + "-" + Id2 + "-" + s;
                    }
                    else
                    {
                        var splittedcounter = counter.Split('-');
                        var val = splittedcounter[2];
                        int value = Convert.ToInt32(val) + 1;
                        string s = value.ToString().PadLeft(3, '0');

                        retailerObj.TicketNo = datein + "-" + Id2 + "-" + s;
                    }

                }
                else if (data.ZoneID == 9)
                {
                    var Id3 = data.ProjectCode;

                    var counter = db.Jobs.Where(x => x.CreatedDate >= dtFromToday && x.CreatedDate <= dtToToday && x.RegionID == data.RegionID && x.ZoneID == data.ZoneID).OrderByDescending(u => u.ID).Select(u => u.TicketNo).FirstOrDefault();


                    if (counter == null)
                    {
                        var ticketCount = 1;
                        string s = ticketCount.ToString().PadLeft(3, '0');
                        retailerObj.TicketNo = datein + "-" + Id3 + "-" + s;
                    }
                    else
                    {
                        var splittedcounter = counter.Split('-');
                        var val = splittedcounter[2];
                        int value = Convert.ToInt32(val) + 1;
                        string s = value.ToString().PadLeft(3, '0');

                        retailerObj.TicketNo = datein + "-" + Id3 + "-" + s;
                    }

                }
                //ADD New Retailer 
                    retailerObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    retailerObj.PersonName = rm.Name;
                    retailerObj.ResolvedAt= DateTime.UtcNow.AddHours(5);
                    retailerObj.SiteID = rm.SiteId;
                    retailerObj.RegionID = data.RegionID;
                    retailerObj.ZoneID = data.ZoneID;
                    retailerObj.CityID = data.CityID;
                    retailerObj.Areas = data.AreaID.ToString();
                    retailerObj.SubDivisionID = data.SubDivisionID;
                    retailerObj.ComplaintStatusId = 2003;
                    retailerObj.FaultTypeId = rm.FaulttypeId;
                    retailerObj.LaunchedById = rm.LaunchedByID;
                    retailerObj.FaultTypeDetailID = rm.FaulttypeDetailId;
                    retailerObj.PriorityId = 0;
                    retailerObj.IsActive = true;
                    retailerObj.Status = true;  
                    retailerObj.InitialRemarks = rm.Remarks;
                    retailerObj.ComplainttypeID = 2;
                    retailerObj.CreatedDate = DateTime.UtcNow.AddHours(5);
                    retailerObj.SaleOfficerID = rm.SaleOfficerID;
                    db.Jobs.Add(retailerObj);

               


                    JobsDetail jobDetail = new JobsDetail();
                    jobDetail.ID = db.JobsDetails.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                    jobDetail.JobID = retailerObj.ID;
                    //jobDetail.PRemarks = rm.FaultTypeDetailOtherRemarks;
                   // jobDetail.ProgressStatusRemarks = rm.ProgressStatusOtherRemarks;
                    jobDetail.ActivityType = rm.FaultTypeDetailOtherRemarks;
                    jobDetail.RetailerID = rm.SiteId;                    
                    jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                    jobDetail.SalesOficerID = rm.SaleOfficerID;
                    if (rm.Picture1 == "" || rm.Picture1 == null)
                    {
                        jobDetail.Picture1 = null;
                    }
                    else
                    {
                        jobDetail.Picture1 = ConvertIntoByte(rm.Picture1, "Complaint", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (rm.Picture2 == "" || rm.Picture2 == null)
                    {
                        jobDetail.Picture2 = null;
                    }
                    else
                    {
                        jobDetail.Picture2 = ConvertIntoByte1(rm.Picture2, "Complaint1", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    if (rm.Picture3 == "" || rm.Picture3 == null)
                    {
                        jobDetail.Picture3 = null;
                    }
                    else
                    {
                        jobDetail.Picture3 = ConvertIntoByte2(rm.Picture3, "Complaint2", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                    }
                    db.JobsDetails.Add(jobDetail);

                    Tbl_ComplaintHistory history = new Tbl_ComplaintHistory();
                var i = 1;
                    history.JobID = retailerObj.ID;
                    history.JobDetailID = jobDetail.ID;
                    history.FaultTypeDetailRemarks = rm.FaultTypeDetailOtherRemarks;
                    history.ProgressStatusRemarks = rm.ProgressStatusOtherRemarks;
                    history.FaultTypeId = rm.FaulttypeId;
                    history.FaultTypeDetailID = rm.FaulttypeDetailId;
                    history.TicketNo = retailerObj.TicketNo;
                    history.ComplaintStatusId = 2003;
                    history.InitialRemarks = rm.Remarks;
                    history.LaunchedById= rm.SaleOfficerID;
                    history.Picture1 = jobDetail.Picture1;
                    history.Picture2 = jobDetail.Picture2;
                    history.Picture3 = jobDetail.Picture3;
                    history.SiteID = rm.SiteId;
                    history.PriorityId = 0;
                    history.IsActive = true;
                    history.IsPublished = i;
                    history.PersonName = retailerObj.PersonName;
                    history.CreatedDate = DateTime.UtcNow.AddHours(5);
                    db.Tbl_ComplaintHistory.Add(history);


                    ComplaintNotification notify = new ComplaintNotification();
                notify.ID = db.ComplaintNotifications.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
                notify.JobID = retailerObj.ID;
                    notify.JobDetailID = jobDetail.ID;
                    notify.ComplaintHistoryID = history.ID;
                    notify.IsSiteIDChanged = true;
                    notify.IsSiteCodeChanged = true;
                    notify.IsFaulttypeIDChanged = true;
                    notify.IsFaulttypeDetailIDChanged = true;
                    notify.IsPriorityIDChanged = true;
                    notify.IsComplaintStatusIDChanged = true;
                    notify.IsPersonNameChanged = true;
                    notify.IsPicture1Changed = true;
                    notify.IsPicture2Changed = true;
                    notify.IsPicture3Changed = true;
                    notify.IsProgressStatusIDChanged = false;
                    notify.IsProgressStatusRemarksChanged = false;
                    notify.IsFaulttypeDetailRemarksChanged = true;
                    notify.IsAssignedSaleOfficerChanged = false;
                    notify.IsUpdateRemarksChanged = false;
                    notify.IsSeen = false;
                    notify.CreatedDate= DateTime.UtcNow.AddHours(5);
                    db.ComplaintNotifications.Add(notify);

                db.SaveChanges();

                var IDs = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == data.AreaID).Select(x => x.SOID).Distinct().ToList();

                foreach (var item in IDs)
                {

                    NotificationSeen seen = new NotificationSeen();

                    seen.JobID= retailerObj.ID;
                    seen.JobDetailID= jobDetail.ID;
                    seen.ComplainthistoryID= history.ID;
                    seen.ComplaintNotificationID = notify.ID;
                    seen.IsSeen = false;
                    seen.SOID = item;
                    db.NotificationSeens.Add(seen);
                    db.SaveChanges();
                }


                string type = "Registraion";
                string message = "Complaint registered Successfully and ComplaintID is " + retailerObj.TicketNo+ " Which is launched at:" +retailerObj.CreatedDate;

                if (retailerObj.ZoneID != 9)
                {
                    var SOIds = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 2).Select(x => x.ID).ToList();
                    List<string> list = new List<string>();
                    foreach (var item in SOIds)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id .Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list.Add(items);
                            }
                        }
                    }

                    if (list != null)
                    {
                        var result = new CommonController().PushNotificationForRegistration(message, list, retailerObj.ID, type, data.ZoneID);
                    }

                    // Notification For KSB Management
                    var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 5 && x.RoleID == 1).Select(x => x.ID).ToList();
                    List<string> list1 = new List<string>();
                    foreach (var item in SOIdss)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list1.Add(items);
                            }
                        }
                    }

                    if (list1 != null)
                    {
                        var result = new CommonController().PushNotificationForRegistration(message, list1, retailerObj.ID, type, data.ZoneID);
                    }

                    // Notification Send to Wasa

                    var AreaID = Convert.ToInt32(data.AreaID);

                    var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                    List<string> list2 = new List<string>();
                    foreach (var item in IdsforWasa)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list2.Add(items);
                            }
                        }
                    }
                    if (list2 != null)
                    {
                        var result2 = new CommonController().PushNotificationForWasa(message, list2, retailerObj.ID, type);
                    }
                }
                else
                {
                    // Notification For Progressive Management
                    var SOIdss = db.SaleOfficers.Where(x => x.RegionalHeadID == 6 && x.RoleID == 2).Select(x => x.ID).ToList();
                    List<string> list1 = new List<string>();
                    foreach (var item in SOIdss)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list1.Add(items);
                            }
                        }
                        if (list1 != null)
                        {
                            var result = new CommonController().PushNotificationForRegistration(message, list1, retailerObj.ID, type, data.ZoneID);
                        }
                    }


                    var AreaID = Convert.ToInt32(data.AreaID);

                    var IdsforWasa = db.SOZoneAndTowns.Where(x => x.CityID == data.CityID && x.AreaID == AreaID).Select(x => x.SOID).Distinct().ToList();
                    List<string> list2 = new List<string>();
                    foreach (var item in IdsforWasa)
                    {
                        var id = db.OneSignalUsers.Where(x => x.UserID == item && x.HeadID == 4).Select(x => x.OneSidnalUserID).ToList();
                        if (id.Count > 0)
                        {
                            foreach (var items in id)
                            {
                                list2.Add(items);
                            }
                        }
                    }
                    if (list2 != null)
                    {
                        var result2 = new CommonController().PushNotificationForWasa(message, list2, retailerObj.ID, type);
                    }
                }


                // Add Token Detail ...
                    TokenDetail tokenDetail = new TokenDetail();
                    tokenDetail.TokenName = rm.Token;
                    tokenDetail.Action = "Add New Retailer";
                    tokenDetail.ProcessedDateTime = DateTime.Now;
                    db.TokenDetails.Add(tokenDetail);
                    //END

                    db.SaveChanges();

                 



                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Complaint Registration Successful",
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

        public string ConvertIntoByte1(string Base64, string DealerName, string SendDateTime, string folderName)
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
        public string ConvertIntoByte2(string Base64, string DealerName, string SendDateTime, string folderName)
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
        public class ComplaintRegistrationRequest
        {
            public ComplaintRegistrationRequest()
            {
                
            }
            public int ID { get; set; }
          
           
            public int SaleOfficerID{ get; set; }
            public int LaunchedByID { get; set; }




            public int? ComplaintTypeID { get; set; }
        

            public string FaultTypeDetailOtherRemarks { get; set; }
            public string ProgressStatusOtherRemarks { get; set; }
        

           

            public Nullable<int> StatusID { get; set; }
           
            public Nullable<int> RetailerID { get; set; }
          
         
            public int SiteId { get; set; }
            public int? FaulttypeId { get; set; }

            public int? PriorityId { get; set; }

            public int? ProgressStatusId { get; set; }
            public string ProgressStatusName { get; set; }
            public int? FaulttypeDetailId { get; set; }

            public int? FaulttypeDetID { get; set; }

            public string Picture1 { get; set; }
            public string Picture2 { get; set; }
            public string Picture3 { get; set; }
            public string Picture4 { get; set; }

            public string Picture5 { get; set; }
            public string Name { get; set; }

            public string Token { get; set; }
            public string Remarks { get; set; }

            public List<CityData> Cities { get; set; }
            public List<RegionData> Regions { get; set; }
            public List<AreaData> Areas { get; set; }
            public List<RegionData> Client { get; set; }
            public List<FaultTypeData> faultTypes { get; set; }
            public List<PriorityData> priorityDatas { get; set; }
            public List<ComplaintStatus> complaintStatuses { get; set; }
            public List<ComplaintStatus> ProgressStatus { get; set; }
            public List<ComplaintStatus> ComplaintTypes { get; set; }
            public List<FaultTypeData> faultTypesDetail { get; set; }
            public List<ComplaintLaunchedBy> LaunchedBy { get; set; }
            public List<SubDivisionData> SubDivisions { get; set; }
            public List<RetailerData> Sites { get; set; }
            public List<SaleOfficerData> SaleOfficers { get; set; }



        }

        public class CompititorInfoModel
        {
            public int SaleOfficerID { get; set; }
            public int RetailerID { get; set; }
            public int SylabusID { get; set; }
        }


    }
}
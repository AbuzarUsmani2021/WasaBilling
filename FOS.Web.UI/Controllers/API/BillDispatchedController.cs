using FOS.DataLayer;
using FOS.Web.UI.Common;
using Shared.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class BillDispatchedController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BillDispatchedData rm)
        { // This controller is for retailers orders.
            JobsDetail jobDet = new JobsDetail();
         
            var JobObj = new Job();
            var RemObj = new TblReminder();
            try
            {
                var ZoneID = db.TBL_Consumers.Where(x => x.ID == rm.RetailerId).FirstOrDefault();
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var data = db.JobsDetails.Where(r => r.ConsumerID == rm.RetailerId && r.JobDate>= startDate && r.JobDate<= endDate).FirstOrDefault();
                //if (data == null)
                //{
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                    JobObj.ID = db.Jobs.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    JobObj.SaleOfficerID = rm.SaleOfficerId;
                    JobObj.RegionalHeadID = db.SaleOfficers.Where(x => x.ID == rm.SaleOfficerId).Select(x => x.RegionalHeadID).FirstOrDefault();
                    //JobObj.RegionalHeadType = ret.SaleOfficer.RegionalHead.Type;
                    JobObj.Status = true;
                    JobObj.StartingDate = localTime;
                    JobObj.DateOfAssign = localTime;
                    JobObj.CreatedDate = localTime;
                    JobObj.LastUpdated = localTime;
                    JobObj.CityID = ZoneID.DDRID;
                    JobObj.ZoneID = ZoneID.WardID;
                JobObj.SubDivisionID = ZoneID.AreaID;
                JobObj.IsActive = true;
                    JobObj.IsDeleted = false;


                    //ADD New Job in jobsdetail 
                    jobDet.JobID = JobObj.ID;
                    jobDet.RegionalHeadID = JobObj.RegionalHeadID;
                    jobDet.SalesOficerID = JobObj.SaleOfficerID;
                    jobDet.RetailerID = 1;
                    jobDet.ConsumerID = rm.RetailerId;
                    jobDet.ActivityDetails = "Online";
                    jobDet.JobDate = localTime;
                    jobDet.JobType = "Dispatch Bill";
                    jobDet.Status = true;
                   // jobDet.ActivityType = rm.ActivityType;
                    jobDet.VisitPurpose = "Ordering";
                    jobDet.IsbillDistributrd = true;
                    jobDet.BillDistributionTime = localTime;
                    jobDet.LatitudeForBillDistribution = rm.Latitude;
                    jobDet.LongitudeForBillDistribution = rm.Longitude;
                    db.Jobs.Add(JobObj);
                    db.SaveChanges();
                    db.JobsDetails.Add(jobDet);
                    db.SaveChanges();

                    string[] statusesList = rm.MultiselectIds.Split(',');

                    if (statusesList != null)
                    {

                        foreach (var item in statusesList)
                        {
                            BillDisMultiSelect Ac = new BillDisMultiSelect();

                            Ac.ConsumerID = rm.RetailerId;
                            Ac.MultiselectID = Convert.ToInt32(item);
                            Ac.CreatedOn = DateTime.UtcNow.AddHours(5);
                            Ac.JobID = JobObj.ID;
                            Ac.Latitude = rm.Latitude;
                            Ac.Longitude = rm.Longitude;
                            db.BillDisMultiSelects.Add(Ac);

                            db.SaveChanges();

                        }
                    }


                //}
                
                    //string[] statusesList = rm.MultiselectIds.Split(',');
                    //var list = db.BillDisMultiSelects.Where(x => x.ConsumerID == rm.RetailerId && x.CreatedOn >= startDate && x.CreatedOn <= endDate).ToList();

                    //if (list != null)
                    //{

                    //    foreach (var item in list)
                    //    {
                    //        db.BillDisMultiSelects.Remove(item);

                    //    }

                    //    foreach (var item in statusesList)
                    //    {
                    //        BillDisMultiSelect Ac = new BillDisMultiSelect();

                    //        Ac.ConsumerID = rm.RetailerId;

                    //        Ac.MultiselectID = Convert.ToInt32(item);
                    //        Ac.CreatedOn = DateTime.UtcNow.AddHours(5);
                    //        Ac.JobID = JobObj.ID;
                    //        Ac.Latitude = rm.Latitude;
                    //        Ac.Longitude = rm.Longitude;
                    //        db.BillDisMultiSelects.Add(Ac);

                    //        db.SaveChanges();

                    //    }
                    //}


                    //data.IsbillDistributrd = true;
                    //data.BillDistributionTime = DateTime.UtcNow.AddHours(5);
                    //data.LatitudeForBillDistribution = rm.Latitude;
                    //data.LongitudeForBillDistribution = rm.Longitude;
                    
                    //db.SaveChanges();
               

                TBL_Consumers ret = db.TBL_Consumers.Where(r => r.ID == rm.RetailerId).FirstOrDefault();
                if (ret.Latitude == null && ret.Longitude == null)
                {
                    ret.Latitude = rm.Latitude;
                    ret.Longitude = rm.Longitude;
                    db.SaveChanges();
                }
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Bill Dispatched Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Order API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Order API Failed",
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
        public class BillDispatchedData
        {
          
            public int JobId { get; set; }
            public int RetailerId { get; set; }
            public int SaleOfficerId { get; set; }
            public string MultiselectIds { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }

            public decimal LatitudeForBillDispatch { get; set; }
            public decimal LongitudeForBillDispatch { get; set; }
        }

       
    }
}
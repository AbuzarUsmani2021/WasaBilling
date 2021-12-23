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
    public class MeterReadingController : ApiController
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
                if (data == null)
                {
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
                    jobDet.JobType = "MeterReading";
                    jobDet.LatitudeForMeterreading = rm.Latitude;
                    jobDet.LongitudeForMeterreading = rm.Longitude;
                    jobDet.Status = true;
                    if (rm.Picture1 == "" || rm.Picture1 == null)
                    {
                        jobDet.Picture1 = null;
                    }
                    else
                    {
                        jobDet.Picture1 = ConvertIntoByte(rm.Picture1, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                    }
                    // jobDet.ActivityType = rm.ActivityType;
                    jobDet.VisitPurpose = "Ordering";
                    jobDet.MeterReadingValue = rm.MeterReading;
                    jobDet.MeterReadingDate = localTime;
                    db.Jobs.Add(JobObj);
                    db.SaveChanges();
                    db.JobsDetails.Add(jobDet);
                    db.SaveChanges();

                    string[] statusesList = rm.MultiselectIds.Split(',');

                    if (statusesList != null)
                    {

                        foreach (var item in statusesList)
                        {
                            Tbl_MeterredingMultiValues Ac = new Tbl_MeterredingMultiValues();

                            Ac.ConsumerID = rm.RetailerId;
                            Ac.MultiselectID = Convert.ToInt32(item);
                            Ac.CreatedOn = DateTime.UtcNow.AddHours(5);
                            Ac.JobID = JobObj.ID;
                            Ac.Latitude = rm.Latitude;
                            Ac.Longitude = rm.Longitude;
                            db.Tbl_MeterredingMultiValues.Add(Ac);

                            db.SaveChanges();

                        }
                    }

                }
                else
                {
                    data.MeterReadingDate = DateTime.UtcNow.AddHours(5);
                    data.MeterReadingValue = rm.MeterReading;
                    data.LatitudeForMeterreading = rm.Latitude;
                    data.LongitudeForMeterreading = rm.Longitude;
                    if (rm.Picture1 == "" || rm.Picture1 == null)
                    {
                        data.Picture1 = null;
                    }
                    else
                    {
                        data.Picture1 = ConvertIntoByte(rm.Picture1, "OrderPicture", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "OrderingPictures");
                    }
                    db.SaveChanges();
                }

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
                    Message = "Meter Reading Done Successfully",
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
            public decimal Latitude { get; set; }
            public string Picture1 { get; set; }
            public string MultiselectIds { get; set; }
            public decimal Longitude { get; set; }
            public decimal LatitudeForMeterReading { get; set; }
            public decimal LongitudeForMeterReading { get; set; }
            public int MeterReading { get; set; }


        }

       
    }
}
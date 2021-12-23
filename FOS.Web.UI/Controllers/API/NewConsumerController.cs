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
    public class NewConsumerController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(BillDispatchedData rm)
        { // This controller is for retailers orders.
            NewConsumerData jobDet = new NewConsumerData();
            
            try
            {
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                // var data = db.JobsDetails.Where(r => r.RetailerID == rm.RetailerId && r.JobDate>= startDate && r.JobDate<= endDate).FirstOrDefault();
                jobDet.RetailerID = rm.RetailerID;
                jobDet.Marla = rm.Marla;
                jobDet.ConnectionTypeId = rm.ConnectionTypeID;
                jobDet.CreatedOn = DateTime.UtcNow.AddHours(5);
                db.NewConsumerDatas.Add(jobDet);
                db.SaveChanges();
          

               
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "New Consumer Added Successfully",
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
          
          
            public int RetailerID { get; set; }
            public int Marla { get; set; }
            public int ConnectionTypeID { get; set; }
            public decimal Latitude { get; set; }
            public string Picture1 { get; set; }
            public decimal Longitude { get; set; }
      
         


        }

       
    }
}
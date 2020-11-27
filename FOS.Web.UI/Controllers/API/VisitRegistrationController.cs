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

                    //ADD New Retailer 
                    retailerObj.ID = db.TBL_KsbVisits.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;
 
                    retailerObj.SiteID = rm.SiteId;
                   

                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.VisitTypeID = rm.VisitTypeId;


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

                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Visit Registration Successful",
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

            public int VisitTypeId { get; set; }
            public string Remarks { get; set; }

            public string Picture1 { get; set; }
            public string Picture2 { get; set; }



        }

      


    }
}
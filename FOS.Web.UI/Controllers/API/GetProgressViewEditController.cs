using FOS.DataLayer;
using FOS.Shared;
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
using System.Web.Services.Protocols;

namespace FOS.Web.UI.Controllers.API
{
    public class GetProgressViewEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(DailyActivityRequests obj)
        {
            var jobDetail = new JobsDetail();
            var job = new Job();
            var comHis = new Tbl_ComplaintHistory();
            try
            {
                


                jobDetail = db.JobsDetails.Where(u => u.ID == obj.ID).FirstOrDefault();

               
                
                jobDetail.PRemarks = obj.Remarks;

                jobDetail.AssignedToSaleOfficer = obj.AssignedToID;
                jobDetail.ProgressStatusID = obj.ProgressStatusId;
                jobDetail.ProgressStatusRemarks = obj.ProgressStatusOtherRemarks;
                jobDetail.JobDate = DateTime.UtcNow.AddHours(5);
                jobDetail.IsPublished = 1;
                jobDetail.ChildFaultTypeDetailID = obj.FaulttypeDetailId;
                jobDetail.ChildFaultTypeID = obj.FaulttypeId;
                jobDetail.ChildStatusID = obj.StatusID;
                jobDetail.ChildAssignedSaleOfficerID = obj.AssignedToID;


                if (obj.Picture1 == "" || obj.Picture1 == null)
                {
                    jobDetail.Picture1 = null;
                }
                else
                {
                    jobDetail.Picture1 = ConvertIntoByte(obj.Picture1, "Complaint", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                }
                if (obj.Picture2 == "" || obj.Picture2 == null)
                {
                    jobDetail.Picture2 = null;
                }
                else
                {
                    jobDetail.Picture2 = ConvertIntoByte1(obj.Picture2, "Complaint1", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                }
                if (obj.Picture3 == "" || obj.Picture3 == null)
                {
                    jobDetail.Picture3 = null;
                }
                else
                {
                    jobDetail.Picture3 = ConvertIntoByte2(obj.Picture3, "Complaint2", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "ComplaintImages");
                }
                job = db.Jobs.Where(x => x.ID == jobDetail.JobID).FirstOrDefault();
                job.FaultTypeId = obj.FaulttypeId;
                job.FaultTypeDetailID = obj.FaulttypeDetailId;
                job.PriorityId = obj.PriorityId;
                job.ResolvedHours = obj.ResolvedHour;
                job.PersonName = obj.Name;
                job.ComplaintStatusId = obj.StatusID;
                comHis = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == jobDetail.ID).FirstOrDefault();

                comHis.SiteID = job.SiteID;
                comHis.FaultTypeId = obj.FaulttypeId;
                comHis.PriorityId = obj.PriorityId;
                comHis.ComplaintStatusId = obj.StatusID;
                comHis.FaultTypeDetailID = obj.FaulttypeDetailId;
                comHis.Picture1 = jobDetail.Picture1;
                comHis.Picture2 = jobDetail.Picture2;
                comHis.Picture3 = jobDetail.Picture3;
                comHis.ProgressStatusID = obj.ProgressStatusId;
                comHis.IsPublished = 1;
                comHis.FaultTypeDetailRemarks = obj.FaultTypeDetailOtherRemarks;
                comHis.ProgressStatusRemarks = obj.ProgressStatusOtherRemarks;
                comHis.AssignedToSaleOfficer = obj.AssignedToID;
                comHis.UpdateRemarks = obj.Remarks;
                db.SaveChanges();


                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "ProgressView Edit Successfully",
                    ResultType = ResultType.Success,
                    Exception = null,
                    ValidationErrors = null
                };
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "ProgressView Edit Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Complaint Edit Failed",
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

     
            return @"/Images/" + folderName + "/" + filestoragename + ".jpg";
        }
        public class SuccessResponse
        {

        }
        public class DailyActivityRequests
        {

            public int ID { get; set; }


            public int SaleOfficerID { get; set; }

            public int ResolvedHour { get; set; }
            public int AssignedToID { get; set; }


            public int? ComplaintTypeID { get; set; }
            public int? IsPublished { get; set; }

            public string FaultTypeDetailOtherRemarks { get; set; }
            public string ProgressStatusOtherRemarks { get; set; }
            public string Name { get; set; }



            public Nullable<int> StatusID { get; set; }

            public Nullable<int> RetailerID { get; set; }


            public int SiteId { get; set; }
            public int? FaulttypeId { get; set; }

            public int? PriorityId { get; set; }

            public int? ProgressStatusId { get; set; }

            public int? WorkDoneID { get; set; }
            public string ProgressStatusName { get; set; }
            public int? FaulttypeDetailId { get; set; }

            public string Picture1 { get; set; }
            public string Picture2 { get; set; }
            public string Picture3 { get; set; }
            public string Picture4 { get; set; }

            public string Picture5 { get; set; }
           

            public string Token { get; set; }
            public string Remarks { get; set; }




        }

       
    }
}
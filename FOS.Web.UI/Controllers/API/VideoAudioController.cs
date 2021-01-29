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
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;

namespace FOS.Web.UI.Controllers.API
{
    public class VideoAudioController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public async Task<string> Upload()
        {
            try
            {
                Random random = new Random();
                int randomNumber = random.Next(5, 99999999);
                var filename = "";
                var ctx = HttpContext.Current;
                var uploadPath = System.Web.HttpContext.Current.Server.MapPath(@"~/ComplaintVideos/");
                Directory.CreateDirectory(uploadPath);
                var provider = new MultipartFormDataStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(provider);

                var Type = HttpContext.Current.Request.Params["Type"];
                var ids = HttpContext.Current.Request.Params["ID"];
                var DetailID = Convert.ToInt32(ids);
                if (Type == "Video")
                {

                    foreach (var file in provider.FileData)
                    {
                         filename = (file.Headers.ContentDisposition.FileName);
                        filename = filename.Trim('"');
                        filename = randomNumber + filename;
                        var loc = file.LocalFileName;

                        var filepath = Path.Combine(uploadPath, filename);
                        File.Move(loc, filepath);
                    }

                    var jobDetailID = db.JobsDetails.Where(x => x.ID == DetailID).FirstOrDefault();

                    jobDetailID.Video = "/ComplaintVideos/" + filename;

                    var HistoryID = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == DetailID).FirstOrDefault();
                    HistoryID.Video= "/ComplaintVideos/" + filename;
                    db.SaveChanges();
                }
                else
                {
                    foreach (var file in provider.FileData)
                    {
                        filename = (file.Headers.ContentDisposition.FileName);
                        filename = filename.Trim('"');
                        filename = randomNumber + filename;
                        var loc = file.LocalFileName;

                        var filepath = Path.Combine(uploadPath, filename);
                        File.Move(loc, filepath);
                    }

                    var jobDetailID = db.JobsDetails.Where(x => x.ID == DetailID).FirstOrDefault();

                    jobDetailID.Audio = "/ComplaintVideos/" + filename;

                    var HistoryID = db.Tbl_ComplaintHistory.Where(x => x.JobDetailID == DetailID).FirstOrDefault();
                    HistoryID.Audio = "/ComplaintVideos/" + filename;
                    db.SaveChanges();
                }
               

                return "Uploaded";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }




    }
}
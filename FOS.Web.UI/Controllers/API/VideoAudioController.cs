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
                var ctx = HttpContext.Current;
                var uploadPath = HostingEnvironment.MapPath("/") + @"/ComplaintVideos";
                Directory.CreateDirectory(uploadPath);
                var provider = new MultipartFormDataStreamProvider(uploadPath);
                await Request.Content.ReadAsMultipartAsync(provider);

               
                // Files
                //
                foreach (var file in provider.FileData)
                {
                    var filename=(file.Headers.ContentDisposition.FileName);
                    filename = filename.Trim('"');
                    var loc=file.LocalFileName;

                    var filepath = Path.Combine(uploadPath, filename);
                    File.Move(loc, filepath);
                }

                // Form data
                //
                var filesToDelete = HttpContext.Current.Request.Params["filesToDelete"];
                var clientContactId = HttpContext.Current.Request.Params["clientContactId"];

                return "Uploaded";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }




    }
}
using FOS.DataLayer;
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
    public class LogoutController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(ChatBoxmodel rm)
        {
            
            try
            {

                var retailers = db.OneSignalUsers.Where(a => a.OneSidnalUserID == rm.OneSignalID).FirstOrDefault();
                db.OneSignalUsers.Remove(retailers);
                db.SaveChanges();
             
                return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Logout Successfully",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                
              

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Logout API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Logout API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }



        public class SuccessResponse
        {

        }
        public class ChatBoxmodel
        {
            
            public string OneSignalID { get; set; }
            
           


        }

    }
}

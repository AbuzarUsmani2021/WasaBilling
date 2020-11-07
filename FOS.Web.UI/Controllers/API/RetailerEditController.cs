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
    public class RetailerEditController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(EditRetailermodel rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //ADD New Retailer 
                    retailerObj = db.Retailers.Where(u => u.ID == rm.ID).FirstOrDefault();

                    retailerObj.Name = rm.OwnerName;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    // Zone ID  is saving in Regions Table bcx in menu region changes to zone
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.RegionID = rm.RegionID;
                    retailerObj.NewArea = rm.AreaName;
                    retailerObj.Phone1 = rm.CellNo1;
                    retailerObj.Phone2 = rm.CellNo2;
                    retailerObj.Email = rm.Email;
                    retailerObj.RetailerClass = rm.RetailerClass;
                    retailerObj.RetailerChannel = rm.RetailerChannel;
                    retailerObj.Address = rm.Address;
                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);


                    retailerObj.LastUpdate = localTime;
                  
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    //db.Retailers.Add(retailerObj);
                    //END

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
                        Message = "Retailer Edit Successful",
                        ResultType = ResultType.Success,
                        Exception = null,
                        ValidationErrors = null
                    };
                }
                else
                {
                    return new Result<SuccessResponse>
                    {
                        Data = null,
                        Message = "Authentication failed in Retailer Edit  API",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };

                }

            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "Retailer Edit API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Edit API Failed",
                    ResultType = ResultType.Exception,
                    Exception = ex,
                    ValidationErrors = null
                };
            }



        }


       

        public class SuccessResponse
        {

        }
        public class EditRetailermodel
        {
            public int ID { get; set; }
            public string ShopName { get; set; }
            public string OwnerName { get; set; }
            public string CellNo1 { get; set; }
            public string CellNo2 { get; set; }
            public int SalesOfficerID { get; set; }
            public string Email { get; set; }
            public string AreaName { get; set; }
            public int RegionID { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public string Address { get; set; }
            public string Token { get; set; }
            public int RetailerClass { get; set; }
            public int RetailerChannel { get; set; }
            public string Picture1 { get; set; }
         
            public string Remarks { get; set; }
            public bool IsVerified { get; set; }
        }

        }
    }

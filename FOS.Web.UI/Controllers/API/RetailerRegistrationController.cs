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
    public class RetailerRegistrationController : ApiController
    {
        FOSDataModel db = new FOSDataModel();

        public Result<SuccessResponse> Post(RetailerRegistrationRequest rm)
        {
            Retailer retailerObj = new Retailer();
            try
            {
                if (FOS.Web.UI.Common.Token.TokenAttribute.IsTokenValid(rm.Token))
                {
                    //ADD New Retailer 
                    retailerObj.ID = db.Retailers.OrderByDescending(u => u.ID).Select(u => u.ID).FirstOrDefault() + 1;

                    retailerObj.Name = rm.OwnerName;
                    retailerObj.SaleOfficerID = rm.SalesOfficerID;
                    retailerObj.DealerID = rm.DistributorID;
                    retailerObj.ShopName = rm.ShopName;
                    retailerObj.AreaID = rm.AreaID;
                    retailerObj.CityID = rm.CityID;
                    // Zone ID  is saving in Regions Table bcx in menu region changes to zone
                    retailerObj.ZoneID = rm.ZoneID;
                    retailerObj.RegionID = rm.RegionID;
                    retailerObj.Location = rm.Latitude + "," + rm.Longitude;
                    retailerObj.Latitude = rm.Latitude;
                    retailerObj.NewArea = rm.AreaName;
                    retailerObj.Longitude = rm.Longitude;
                    retailerObj.LocationName = rm.LocationName;
                    retailerObj.LocationMargin = null;
                    retailerObj.Phone1 = rm.CellNo1;
                    retailerObj.Phone2 = rm.CellNo2;
                    retailerObj.Email = rm.Email;
                    retailerObj.RetailerClass = rm.RetailerClass;
                    retailerObj.RetailerChannel = rm.RetailerChannel;
                 
                    if (rm.Picture1 == "" || rm.Picture1 == null)
                    {
                        retailerObj.Picture1 = null;
                    }
                    else
                    {
                        retailerObj.Picture1 = ConvertIntoByte(rm.Picture1, "Retailer", DateTime.Now.ToString("dd-mm-yyyy hhmmss").Replace(" ", ""), "RetailerImages");
                    }
                   
                
                    retailerObj.Remarks = rm.Remarks;
                    retailerObj.IsActive = true;
                    retailerObj.Status = true;
                    retailerObj.RetailerType = "Retailer";
                    retailerObj.IsDeleted = false;
                    retailerObj.Address = rm.Address;

                    DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
                    DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using timezone setting of server computer

                    TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                    retailerObj.LastUpdate = localTime;
                    retailerObj.CreatedBy = rm.SalesOfficerID;

                    db.Retailers.Add(retailerObj);
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
                        Message = "Retailer Registration Successful",
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
                        Message = "Authentication failed in Retailer registration API",
                        ResultType = ResultType.Failure,
                        Exception = null,
                        ValidationErrors = null
                    };

                }

            }
            catch (Exception ex)
            {
               
                Log.Instance.Error(ex, "Add Retailer API Failed");
                return new Result<SuccessResponse>
                {
                    Data = null,
                    Message = "Retailer Registration API Failed",
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
        public class RetailerRegistrationRequest
        {
            public RetailerRegistrationRequest()
            {
                CompititorInformation = new List<CompititorInfoModel>();
            }
            public int RetailerID { get; set; }
            public string ShopName { get; set; }
            public string OwnerName { get; set; }
            public string CellNo1 { get; set; }
            public string CellNo2 { get; set; }
            public int SalesOfficerID { get; set; }
            public string Email { get; set; }
            public int RegionID { get; set; }
            public int CityID { get; set; }
            public int ZoneID { get; set; }
            public int AreaID { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string LocationName { get; set; }
            public string Address { get; set; }
            public string AreaName { get; set; }
            public string Token { get; set; }
            public int RetailerClass { get; set; }
            public int RetailerChannel { get; set; }
            public int DistributorID { get; set; }
            public string Picture1 { get; set; }
            //public string Picture2 { get; set; }
           // public string Customercode { get; set; }
            public string Remarks { get; set; }
            public bool IsVerified { get; set; }

            public List<CompititorInfoModel> CompititorInformation { get; set; }



        }

        public class CompititorInfoModel
        {
            public int SaleOfficerID { get; set; }
            public int RetailerID { get; set; }
            public int SylabusID { get; set; }
        }


    }
}
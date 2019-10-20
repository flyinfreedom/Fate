using Fate.Helper;
using Fate.Models;
using Fate.Service;
using Fate.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Fate.Controllers
{
    public class FateController : BaseController
    {
        private IFateService _fateService;
        private MailHelper _mailHelper;

        public FateController()
        {
            _fateService = new FateService();
            _mailHelper = new MailHelper();
        }

        public ActionResult ST01(string orderid)
        {
            VMST01 instance = new VMST01();

            if (!string.IsNullOrEmpty(orderid))
            {
                string fateType = "ST01";
                bool isPayed = CheckIsPayed(orderid);  //---- 確認是否已付費

                if (isPayed)
                {
                    var orderDetail = db.Order.FirstOrDefault(o => o.OrderId == orderid).OrderDetail.FirstOrDefault(x => x.ProductId == "ST01");
                    instance.BirthHour = orderDetail.BirthHour.Value;
                    instance.BirthDay = orderDetail.BirthDay;
                    instance.DateType = orderDetail.DateType == 1 ? "CN" : "CE";
                    instance.Gender = orderDetail.Gender.Value;

                    string tempUserInputBirthDay = instance.BirthDay;

                    if (instance.DateType == "CN")
                    {
                        instance.BirthDay = GetCEDateFormCNDate(instance.BirthDay);
                        if (instance.BirthDay.Contains("1899"))
                        {
                            return Json(new { Success = false, Message = "您輸入的農曆日期不存在" });
                        }
                    }

                    IResult result = this._fateService.GetFateResultCode(fateType, isPayed, instance);

                    if (result.Success)
                    {
                        string fourth = ("" + result.FateResult[0]);
                        instance.FourthLife = db.Result.FirstOrDefault(x => x.Code == fourth).Brief;
                        instance.FourthLifeSuggest = db.Result.FirstOrDefault(x => x.Code == fourth).FullDescription;

                        if (isPayed)
                        {
                            string third = ("" + result.FateResult[1]);
                            string second = ("" + result.FateResult[2]);
                            string first = ("" + result.FateResult[3]);
                            instance.ThirdLife = string.IsNullOrEmpty(third) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == third).Brief;
                            instance.ThirdSuggest = string.IsNullOrEmpty(third) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == third).FullDescription;
                            instance.SecondLife = string.IsNullOrEmpty(second) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == second).Brief;
                            instance.SecondLifeSuggest = string.IsNullOrEmpty(second) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == second).FullDescription;
                            instance.FirstLife = string.IsNullOrEmpty(first) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == first).Brief;
                            instance.FirstLifeSuggest = string.IsNullOrEmpty(first) ? string.Empty : db.Result.FirstOrDefault(x => x.Code == first).FullDescription;
                            instance.BirthDay = tempUserInputBirthDay;
                        }
                    }

                    instance.OrderId = orderid;
                }
            }

            SetFadeCode(instance);
            return View(instance);
        }

        [HttpPost]
        public JsonResult ST01(VMST01 instance)
        {
            //string fateType = "ST01";
            //bool isPayed = CheckIsPayed(instance.OrderId);  //---- 確認是否已付費

            //if (isPayed)
            //{
            //    var orderDetail = db.Order.FirstOrDefault(o => o.OrderId == instance.OrderId).OrderDetail.FirstOrDefault(x => x.ProductId == "ST01");
            //    instance.BirthHour = orderDetail.BirthHour.Value;
            //    instance.BirthDay = orderDetail.BirthDay;
            //    instance.DateType = orderDetail.DateType == 1 ? "CN" : "CE";
            //    instance.Gender = orderDetail.Gender.Value;
            //}

            ////---- 如果使用者輸入的是農曆年，從資料庫比對出國曆年
            //string tempUserInputBirthDay = instance.BirthDay;

            //if (instance.DateType == "CN")
            //{
            //    instance.BirthDay = GetCEDateFormCNDate(instance.BirthDay);
            //    if (instance.BirthDay.Contains("1899"))
            //    {
            //        return Json(new { Success = false, Message = "您輸入的農曆日期不存在" });
            //    }
            //}

            IResult result = this._fateService.GetFateResultCode("ST01", false, instance);

            if (result.Success)
            {
                string fourth = ("" + result.FateResult[0]);
                instance.FourthLife = db.Result.FirstOrDefault(x => x.Code == fourth).Brief;
                instance.FourthLifeSuggest = db.Result.FirstOrDefault(x => x.Code == fourth).FullDescription;

                if (!string.IsNullOrEmpty(instance.ContactPhone))
                {
                    var amount = db.Product.FirstOrDefault(x => x.ProductId == "ST01").Amount;
                    var getTxIdObj = GetTxId(new GetTxIdRequest
                    {
                        condition = JsonConvert.SerializeObject(new { instance.BirthHour, instance.BirthDay, instance.Gender }),
                        name = instance.Name,
                        productId = "ST01",
                        uid = instance.Email
                    }, amount, instance.Email, instance.ContactPhone);

                    return Json(getTxIdObj);
                }
            }

            //return View(instance);
            return Json(instance);
        }

        public ActionResult NA01(string orderid)
        {
            VMNA01 viewModel = new VMNA01();
            if (!string.IsNullOrEmpty(orderid))
            {
                bool isPayed = CheckIsPayed(orderid);  //---- 確認是否已付費

                if (isPayed)
                {
                    var detail = db.OrderDetail.FirstOrDefault(x => x.OrderId == orderid);
                    viewModel.FirstName = detail.FirstName;
                    viewModel.LastName = detail.LastName;

                    IResult result = this._fateService.GetFateResultCode("NA01", true, viewModel);
                    string resultCode = result.FateResult[0];
                    viewModel.Result = db.Result.FirstOrDefault(x => x.Code == resultCode).Brief;
                }
            }
            SetFadeCode(viewModel); 
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult NA01(GetTxIdRequest request)
        {
            var amount = db.Product.FirstOrDefault(x => x.ProductId == "NA01").Amount;
            var getTxIdObj = GetTxId(request, amount, request.email, request.uid);
            return Json(getTxIdObj);
        }

        [HttpPost]
        public JsonResult WordingCheck(List<string> wordList)
        {
            List<string> result = new List<string>();
            foreach (string item in wordList)
            {
                string temp = CharSetConverter.ToTraditional(item);
                if (!db.WordLibrary.Any(x => x.Word == temp))
                {
                    result.Add(item);
                }
            }
            return Json(result);
        }

        [HttpPost]
        public JsonResult SaveWordStroke(List<WordStroke> instance)
        {
            Service.Result result = new Service.Result(true);
                        
            foreach (WordStroke item in instance)
            {
                if (!db.WordLibrary.Any(x => x.Word == item.Word))
                {
                    db.WordLibraryUserInput.Add(new WordLibraryUserInput {
                        Word = item.Word,
                        Stroke = item.Stroke,
                        Created = DateTime.Now
                    });

                    db.WordLibrary.Add(new WordLibrary {
                        Word = item.Word,
                        Stroke = item.Stroke
                    });
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Success = false;
            }

            return Json(result);
        }

        private bool CheckIsPayed(string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                var order = db.Order.FirstOrDefault(x => x.OrderId == orderId);
                if (order != null)
                {
                    if (order.IsPayed)
                    {
                        return true;
                    }
                    else
                    {
                        var paymentService = new PaymentService();
                        var paymentResult = paymentService.QueryTxIdStatus(order.OrderId, order.TxId);

                        if (paymentResult.resultCode == "0000")
                        {
                            order.IsPayed = true;
                            db.SaveChanges();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void SendEmail(string email, string subject, string body)
        {
            this._mailHelper.SetMailMessage(new string[] { email }, subject, body);
            try
            {
                this._mailHelper.Send();
            }
            catch (Exception ex)
            {
                //--- log
            }
            this._mailHelper.Dispose();
        }

        private string GetCEDateFormCNDate(string date)
        {
            CNDateToCEDate helper = new CNDateToCEDate(base.db);
            int[] dateSplit = date.Split('/').ToList().Select(x => Convert.ToInt32(x)).ToArray();
            bool isLeap = false;
            if (dateSplit[1] > 12)
            {
                isLeap = true;
                int temp = dateSplit[1] % 12;
                dateSplit[1] = temp == 0 ? 12 : temp;
            }

            return helper.GetCEDate(dateSplit[0], dateSplit[1], dateSplit[2], isLeap).ToString("yyyy/MM/dd");
        }

        private string CreateOrderId()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }

        private GetTxIdResponse GetTxId(GetTxIdRequest request, int amount, string email, string phone)
        {
            string orderId = CreateOrderId();

            var requestModel = new GetTxIdRequestModel();
            requestModel.amount = amount;
            requestModel.uid = phone;
            requestModel.userIp = GetClientIp();
            requestModel.orderId = orderId;
            requestModel.gameUrl = GetGameUrl(request.productId, orderId);
            requestModel.countryPrefix = phone.Split(' ')[0];
            requestModel.msisdn = phone.Split(' ')[1];

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(requestModel.GetFullUrl());
            httpRequest.Method = "POST";

            string responseStr = string.Empty;
            var responseObj = new GetTxIdResponseModel();

            using (WebResponse response = httpRequest.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                    responseObj = JsonConvert.DeserializeObject<GetTxIdResponseModel>(responseStr);
                }//end using  
            }

            string paymentDataStr = JsonConvert.SerializeObject(new { requestModel.amount, responseObj.orderId });
            string paymentData = AESHelper.Encrypt(paymentDataStr);

            if (string.IsNullOrEmpty(email))
            {
                email = string.Empty;
            }

            using (var db = new FortuneTellingEntities())
            {
                var condition = JsonConvert.DeserializeObject<ConditionModel>(request.condition);
                var order = new Order
                {
                    OrderId = orderId,
                    Datetime = DateTime.Now,
                    Email = email,
                    ContactPhone = phone,
                    Name = request.name,
                    Gender = condition.Gender,
                    IPAddress = GetClientIp(),
                    TxId = responseObj.txId
                };

                order.OrderDetail.Add(new OrderDetail
                {
                    BirthDay = condition.BirthDay,
                    BirthHour = condition.BirthHour,
                    DateType = (int)condition.DateType,
                    FirstName = condition.FirstName,
                    LastName = condition.LastName,
                    Gender = condition.Gender,
                    IsLeap = condition.IsLeap,
                    ProductId = request.productId
                });

                db.Order.Add(order);
                db.SaveChanges();
            }

            return new GetTxIdResponse
            {
                paymentData = paymentData,
                paymentUrl = WebConfigVariable.PaymentUrl,
                txId = responseObj.txId
            };
        }

        private string GetGameUrl(string productId, string orderId)
        {
            string baseUrl = WebConfigVariable.BaseUrl;
            switch (productId)
            {
                case "ST01":
                    return $"{baseUrl}/Fate/ST01?orderid={orderId}";
                case "NA01":
                    return $"{baseUrl}/Fate/NA01?orderid={orderId}";
            }

            return $"{baseUrl}/index.html#/results?orderid={orderId}";
        }

        private string GetClientIp()
        {
            return Request.UserHostAddress;
        }
    }

    /**
     *  六道輪迴    ST     (Six Taoism)
     *  姓名學      NA     (Name Academic)
     *  ....
     */
}
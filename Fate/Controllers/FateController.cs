using Fate.Helper;
using Fate.Models;
using Fate.Service;
using Fate.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult ST01()
        {
            VMST01 viewModel = new VMST01();
            SetFadeCode(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public JsonResult ST01(VMST01 instance)
        {
            string token = "temp";
            string fateType = "ST01";
            bool isPayed = CheckIsPayed(token);  //---- 確認是否已付費
            isPayed = !string.IsNullOrEmpty(instance.Email);

            //---- 如果使用者輸入的是農曆年，從資料庫比對出國曆年
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
                    SaveOrder(instance);
                    SendEmail(instance.Email, "測是您的前世今生", ViewToString("ST01", "Mail", null, instance));
                }
            }

            //return View(instance);
            return Json(instance);
        }

        private void SaveOrder(VMFateBase baseInstance)
        {
            Models.Order order = new Models.Order()
            {
                OrderId = Guid.NewGuid().ToString(),
                Email = baseInstance.Email,
                CompanyName = baseInstance.VATTitle ?? string.Empty,
                ContactAddress = baseInstance.ContactAddress ?? string.Empty,
                ContactPhone = baseInstance.ContactPhone ?? string.Empty,
                InvoiceHandle = baseInstance.InvoiceHandle,
                RecipientCode = baseInstance.CaringCode ?? string.Empty,
                Datetime = DateTime.Now,
                VATNumber = baseInstance.VATNumber ?? string.Empty,
                Name = baseInstance.Name ?? string.Empty,
                IPAddress = Request.UserHostAddress
            };

            order.OrderDetail.Add(new Models.OrderDetail
            {
                Product = db.Product.FirstOrDefault(x => x.ProductId == "ST01"),
                DateType = baseInstance.DateType == "CE" ? 0 : 1,
                BirthDay = baseInstance.BirthDay,
                BirthHour = baseInstance.BirthHour,
                Gender = baseInstance.Gender
            });

            db.Order.Add(order);


            db.SaveChanges();
            db.Dispose();
        }

        public ActionResult NA01()
        {
            VMNA01 viewModel = new VMNA01();
            SetFadeCode(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult NA01(VMNA01 instance)
        {
            IResult result = this._fateService.GetFateResultCode("NA01", true, instance);
            string resultCode = result.FateResult[0];
            instance.Result = db.Result.FirstOrDefault(x => x.Code == resultCode).Brief;
            return View(instance);
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

        private bool CheckIsPayed(string token)
        {
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
    }

    /**
     *  六道輪迴    ST     (Six Taoism)
     *  姓名學      NA     (Name Academic)
     *  ....
     */
}
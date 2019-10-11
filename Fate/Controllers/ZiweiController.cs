using Fate.Helper;
using Fate.Models;
using Fate.Service;
using Fate.Service.Strategy;
using Fate.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Fate.Controllers
{
    [RoutePrefix("api/ziwei")]
    public class ZiweiController : ApiController
    {
        [Route("")]
        [HttpPost]
        public ZiweiResponse GetResultByCulture(ZiweiRequest request)
        {
            var result = new ZiweiResponse();
            var language = new LanguageData();

            bool isPayed = false;

            using (var db = new FortuneTellingEntities())
            {
                var order = db.Order.FirstOrDefault(x => x.OrderId == request.OrderId && x.OrderDetail.Any(d => d.ProductId == "Ziwei"));

                if (order != null && !order.IsPayed) {
                    PaymentService payment = new PaymentService();
                    var query = payment.QueryTxIdStatus(order.OrderId, order.TxId);
                    if (query.resultCode == "0000") {
                        order.IsPayed = true;
                        isPayed = true;
                    }
                }

                DateTimeHelper dateTimeHelper;

                var palaceList = (new Palace[] { Palace.Siblings, Palace.Parents, Palace.Travel }).ToList();

                if (isPayed)
                {
                    var ziweiDetail = order.OrderDetail.FirstOrDefault(x => x.ProductId == "Ziwei");
                    var date = ziweiDetail.BirthDay.Split('-').Select(s => Convert.ToInt32(s)).ToArray();
                    dateTimeHelper = new DateTimeHelper(db, (DateType)ziweiDetail.DateType, date[0], date[1], date[2], ziweiDetail.BirthHour.Value, ziweiDetail.IsLeap ?? false);
                    palaceList = Enum.GetValues(typeof(Palace)).Cast<Palace>().ToList();
                }
                else
                {
                    dateTimeHelper = new DateTimeHelper(db, request.DateType, request.Year, request.Month, request.Day, request.BirthTime, request.IsLeap);
                }

                //palaceList = Enum.GetValues(typeof(Palace)).Cast<Palace>().ToList();
#if DEBUG
                palaceList = Enum.GetValues(typeof(Palace)).Cast<Palace>().ToList();
#endif 
                var ziwei = new Ziwei(dateTimeHelper.Heavenly, dateTimeHelper.Branch, dateTimeHelper.CNMonth, dateTimeHelper.CNDay, dateTimeHelper.Birthtime);

                var allStarCode = ziwei.AstrologyChart
                    .Where(x => palaceList.Contains(x.palace))
                    .Select(x => (x.palace, x.GetStars()
                        .Select(s => GetStarCode(s.Star, x.palace))))
                    .ToList();

                var allStarCodeString = allStarCode.SelectMany(s => s.Item2);

                var allStarResult = db.Result.Where(x => allStarCodeString.Any(s => x.Code.Contains(s))).ToList();
                
                result.AstrologyChart = ziwei.AstrologyChart
                    .Where(x => palaceList.Contains(x.palace))
                    .Select(s => new AstrologyChartCulture { 
                        palace = language.GetZiweiString(s.palace.ToString()),
                        branch = language.GetBranchString(s.branch.ToString()),
                        heavenly = language.GetHeavenlyString(s.heavenly.ToString()),
                        isBodyPalace = s.isBodyPalace,
                        major = s.MajorStars.Select(m => new StarResultCulture { 
                            Star = language.GetZiweiString(m.Star.ToString()),
                            Status = language.GetZiweiString(m.Status.ToString())
                        }).ToList(),
                        minor = s.MinorStars.Select(m => new StarResultCulture
                        {
                            Star = language.GetZiweiString(m.Star.ToString()),
                            Status = language.GetZiweiString(m.Status.ToString())
                        }).ToList(),
                        righteous = s.RighteousStars.Select(m => new StarResultCulture
                        {
                            Star = language.GetZiweiString(m.Star.ToString()),
                            Status = language.GetZiweiString(m.Status.ToString())
                        }).ToList(),
                        secondary = s.SecondaryStars.Select(m => new StarResultCulture
                        {
                            Star = language.GetZiweiString(m.Star.ToString()),
                            Status = language.GetZiweiString(m.Status.ToString())
                        }).ToList(),
                        score = s.Score,
                        majorDescription = string.Join("\n\n", s.MajorStars
                            .Select(starResult =>  
                                GetBriefByCode(allStarResult, starResult, s.palace))
                            .Distinct()),
                        minorDescription = string.Join("\n\n", s.MinorStars
                            .Select(starResult =>
                                GetBriefByCode(allStarResult, starResult, s.palace))
                            .Distinct()),
                        righteousDescription = string.Join("\n\n", s.RighteousStars
                            .Select(starResult =>
                                GetBriefByCode(allStarResult, starResult, s.palace))
                            .Distinct()),
                        secondaryDescription = string.Join("\n\n", s.SecondaryStars
                            .Select(starResult =>
                                GetBriefByCode(allStarResult, starResult, s.palace))
                            .Distinct()),
                    }).ToList();

                result.Heavenly = language.GetHeavenlyString(dateTimeHelper.Heavenly.ToString());
                result.Branch = language.GetBranchString(dateTimeHelper.Branch.ToString());
                result.BirthTime = $"{GetZiPeriod(result.BirthTimeValue)}{language.GetBranchString(dateTimeHelper.Birthtime.ToString())}";
                result.BirthTimeValue = (int)dateTimeHelper.Birthtime;
                result.Month = dateTimeHelper.RequestCNMonth.ToString();
                result.Day = dateTimeHelper.RequestCNDay.ToString();
                result.IsLeap = dateTimeHelper.IsLeap;
                result.FiveElements = language.GetZiweiString(ziwei.FiveElements.ToString());
                result.LifeMajorStar = language.GetZiweiString(ziwei.LifeMajorStar.ToString());
                result.BodyMajorStar = language.GetZiweiString(ziwei.BodyMajorStar.ToString());
                result.HuaLu = language.GetZiweiString(ziwei.HuaLu.ToString());
                result.HuaChiuan = language.GetZiweiString(ziwei.HuaChiuan.ToString());
                result.HuaKe = language.GetZiweiString(ziwei.HuaKe.ToString());
                result.HuaJi = language.GetZiweiString(ziwei.HuaJi.ToString());
                result.BirthDay = dateTimeHelper.DateTime.ToString("yyyy-MM-dd");

                string videoBaseUrl = "/Videos/";

                if (isPayed)
                {
                    result.Name = order.Name;
                    result.Gender = order.Gender ?? true;

                    List<VideoResult> videoResult = new List<VideoResult>();
                    var v1 = db.Video.FirstOrDefault(v => v.VideoTypeId == 1 && v.Code == dateTimeHelper.RequestCNMonth.ToString());
                    videoResult.Add(new VideoResult { 
                        url = videoBaseUrl + v1.Name,
                        description = v1.VideoType.VideoType1
                    });
                    var v2 = db.Video.FirstOrDefault(v => v.VideoTypeId == 2 && v.Code == dateTimeHelper.DateTime.Year.ToString().Remove(0, 3));
                    videoResult.Add(new VideoResult
                    {
                        url = videoBaseUrl + v2.Name,
                        description = v2.VideoType.VideoType1
                    });
                    var v3 = db.Video.FirstOrDefault(v => v.VideoTypeId == 3 && v.Code == dateTimeHelper.RequestCNMonth.ToString());
                    videoResult.Add(new VideoResult
                    {
                        url = videoBaseUrl + v3.Name,
                        description = v3.VideoType.VideoType1
                    });
                    var v4 = db.Video.FirstOrDefault(v => v.VideoTypeId == 4 && v.Code == dateTimeHelper.RequestCNMonth.ToString());
                    videoResult.Add(new VideoResult
                    {
                        url = videoBaseUrl + v4.Name,
                        description = v4.VideoType.VideoType1
                    });

                    result.Videos = videoResult;
                }
                db.SaveChanges();
            }

            return result;

            string GetBriefByCode(List<Fate.Models.Result> source, StarResult starResult, Palace palace)
            {
                var tempss = source;
                string temp = GetStarCode(starResult.Star, palace, starResult.Status);
                return source.Any(x => x.Code == temp)
                    ? source.FirstOrDefault(x => x.Code == temp).Brief
                    : source.Any(x => x.Code == GetStarCode(starResult.Star, palace))
                        ? source.FirstOrDefault(x => x.Code == GetStarCode(starResult.Star, palace)).Brief
                        : string.Empty;
            }
        }

        private object GetZiPeriod(int birthTimeValue)
        {
            if (birthTimeValue == 1 || birthTimeValue == 13)
            {
                return birthTimeValue == 1 ? "早" : "晚";
            }
            return string.Empty;
        }

        private string GetStarCode(Star star, Palace palace, StarStatus status = StarStatus.Normal)
        {
            var statusCode = status == StarStatus.Normal ? string.Empty : status > 0 ? "01" : "02"; 
            return $"ziwei-{star.ToString("d").PadLeft(2, '0')}{palace.ToString("d").PadLeft(2, '0')}{statusCode}";
        }
    }
}

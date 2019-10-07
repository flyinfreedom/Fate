using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.ViewModels
{
    public class ZiweiResponse
    {
        public string Heavenly { get; set; }
        public string Branch { get; set; }
        public string BirthDay { get; set; }
        public string BirthTime { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public List<AstrologyChartCulture> AstrologyChart { get; set; }
        public string FiveElements { get; set; }
        public string LifeMajorStar { get; set; }
        public string BodyMajorStar { get; set; }
        public string HuaLu { get; set; }
        public string HuaChiuan { get; set; }
        public string HuaKe { get; set; }
        public string HuaJi { get; set; }        
        public List<VideoResult> Videos { get; set; }
        public string Name { get; set; }
        public bool Gender { get; set; }
    }

    public class AstrologyChartCulture
    {
        public AstrologyChartCulture()
        {
            major = new List<StarResultCulture>();
            minor = new List<StarResultCulture>();
            righteous = new List<StarResultCulture>();
            secondary = new List<StarResultCulture>();
        }

        public string palace { get; set; }
        public bool isBodyPalace { get; set; }
        public string heavenly { get; set; }
        public string branch { get; set; }
        public List<StarResultCulture> major { get; set; }
        public List<StarResultCulture> minor { get; set; }
        public List<StarResultCulture> righteous { get; set; }
        public List<StarResultCulture> secondary { get; set; }
        public int score { get; set; }
        public string majorDescription { get; set; }
        public string minorDescription { get; set; }
        public string secondaryDescription { get; set; }
        public string righteousDescription { get; set; }
    }

    public class StarResultCulture
    {
        public string Star { get; set; }
        public string Status { get; set; }
    }

    public class VideoResult
    { 
        public string url { get; set; }
        public string description { get; set; }
    }
}
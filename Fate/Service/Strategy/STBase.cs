using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Service
{
    public class STBase
    {
        private string[] _STList = new string[] {  "鼠-佛道天貴", "牛-鬼道天厄", "虎-人道天權", "兔-畜牲道天破",
                                                    "龍-修羅道天奸", "蛇-仙道天文", "馬-佛道天福", "羊-鬼道天驛",
                                                    "猴-人道天孤", "雞-畜牲道天刃", "狗-修羅道天藝", "豬-仙道天壽"};

        protected List<STModel> STList = new List<STModel>();

        public STBase()
        {
            foreach (string st in _STList)
            {
                STList.Add(new STModel
                {
                    Zodiac = st.Split('-')[0],
                    Title = st.Split('-')[1]
                });
            }
        }
    }

    public class STModel
    {
        public string Zodiac { get; set; }
        public string Title { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fate.Service.Strategy
{
    public class Ziwei
    {
        //--- init Data
        private Heavenly _heavenly;
        private Branch _branch;
        private int _month;
        private int _day;
        private BirthTimePeriod _time;

        //--- public Property
        public List<AstrologyChart> AstrologyChart { get; private set; }
        public FiveElements FiveElements { get; private set; }
        public Star LifeMajorStar { get; private set; }
        public Star BodyMajorStar { get; private set; }
        public Star HuaLu { get; private set; }
        public Star HuaChiuan { get; private set; }
        public Star HuaKe { get; private set; }
        public Star HuaJi { get; private set; }

        //--- mapping table
        private Dictionary<int, Dictionary<Heavenly, FiveElements>> _fiveElementMappingTable;
        private Dictionary<FiveElements, Dictionary<int, Branch>> _ziWeiStarMappingTable;
        private Dictionary<Branch, Dictionary<Star, Branch>> _ziWeiStarsMappingTable;
        private Dictionary<Branch, Branch> _tianFuStarMappingTable;
        private Dictionary<Star, Dictionary<Branch, StarStatus>> _starStatusMapping;
        private Dictionary<Star, StarType> _starTypeMapping;
        private Dictionary<Star, Dictionary<StarStatus, int>> _starScoreMapping;

        public Ziwei(Heavenly heavenly, Branch branch, int month, int day, BirthTimePeriod time)
        {
            InitData(heavenly, branch, month, day, time);
            Operation();
        }
               
        #region PublicMethod
        public List<StarResult> GetStarsCodeByPalace(Palace palace)
        {
            AstrologyChart p = AstrologyChart.FirstOrDefault(x => x.palace == palace);
            List<StarResult> result = p.monthStars.Concat(p.timeStars).Concat(p.yearHeavenlyStars).Concat(p.yearBranchStars).ToList();

            if (p.ziWeiStar != null)
            {
                result.Add(p.ziWeiStar);
            }
            if (p.tianFuStar != null)
            {
                result.Add(p.tianFuStar);
            }

            return result;
        }
        #endregion

        private void InitData(Heavenly heavenly, Branch branch, int month, int day, BirthTimePeriod time)
        {
            _heavenly = heavenly;
            _branch = branch;
            _month = month;
            _day = day;
            _time = time;
            AstrologyChart = new List<AstrologyChart>();
        }

        private void Operation()
        {
            InitAstrologyChart();
            InitMappingTable();

            SetPalace();
            SetElement();
            SetZiWeiStar();
            SetTianFuStar();
            SetMonthStar();
            SetTimeStar();
            SetYearHeavenly();
            SetYearBranch();
            SetMajorStar();
            SetStarTypeAndScoreForPalace();
        }

        #region init Operation Function
        private void InitAstrologyChart()
        {
            int heavenlyBase = (int)_heavenly > 5 ? (int)_heavenly - 5 : (int)_heavenly;
            int heavenlyNum = (heavenlyBase * 2 + 1) % 10;
            foreach (var item in GetValues<Branch>())
            {

                AstrologyChart.Add(new AstrologyChart
                {
                    heavenly = (Heavenly)heavenlyNum,
                    branch = item
                });

                if ((int)item == 2)
                {
                    heavenlyNum = heavenlyNum - 2;
                }

                LoopNum(ref heavenlyNum, 10);
            }

            IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }

        private void SetPalace()
        {
            int lifePalaceFormula = _month - (int)_time + 3;
            int lifePalaceNum = lifePalaceFormula == 0 ? 12 : lifePalaceFormula;
            lifePalaceNum = lifePalaceNum < 0 ? lifePalaceNum + 12 : lifePalaceNum;

            lifePalaceNum = 14 - lifePalaceNum;
            lifePalaceNum = lifePalaceNum > 12 ? lifePalaceNum - 12 : lifePalaceNum;

            int bodyPalaceNum = (_month + (int)_time + 1) % 12;
            bodyPalaceNum = bodyPalaceNum == 0 ? 12 : bodyPalaceNum;

            foreach (var item in AstrologyChart)
            {
                item.palace = (Palace)lifePalaceNum;
                item.isBodyPalace = (int)item.branch == bodyPalaceNum;

                LoopNum(ref lifePalaceNum, 12);
            }
        }

        private void SetElement()
        {
            int lifePalaceBranchNum = (int)AstrologyChart.FirstOrDefault(x => x.palace == Palace.Life).branch;
            lifePalaceBranchNum = Convert.ToInt32(Math.Ceiling((double)lifePalaceBranchNum / 2));

            FiveElements = _fiveElementMappingTable[lifePalaceBranchNum][_heavenly];
        }

        private void SetZiWeiStar()
        {
            var ziWeiStarIn = _ziWeiStarMappingTable[FiveElements][_day];
            AstrologyChart.FirstOrDefault(x => x.branch == ziWeiStarIn).ziWeiStar = GetStarResult(Star.ZiWei, ziWeiStarIn);

            var tianJiIn = _ziWeiStarsMappingTable[ziWeiStarIn][Star.TianJi];
            AstrologyChart.FirstOrDefault(x => x.branch == tianJiIn).ziWeiStar = GetStarResult(Star.TianJi, tianJiIn);

            var taiYangIn = _ziWeiStarsMappingTable[ziWeiStarIn][Star.TaiYang];
            AstrologyChart.FirstOrDefault(x => x.branch == taiYangIn).ziWeiStar = GetStarResult(Star.TaiYang, taiYangIn);

            var wuQuIn = _ziWeiStarsMappingTable[ziWeiStarIn][Star.WuQu];
            AstrologyChart.FirstOrDefault(x => x.branch == wuQuIn).ziWeiStar = GetStarResult(Star.WuQu, wuQuIn);

            var tianTongIn = _ziWeiStarsMappingTable[ziWeiStarIn][Star.TianTong];
            AstrologyChart.FirstOrDefault(x => x.branch == tianTongIn).ziWeiStar = GetStarResult(Star.TianTong, tianTongIn);

            var lianZhenIn = _ziWeiStarsMappingTable[ziWeiStarIn][Star.LianZhen];
            AstrologyChart.FirstOrDefault(x => x.branch == lianZhenIn).ziWeiStar = GetStarResult(Star.LianZhen, lianZhenIn);
        }

        private void SetTianFuStar()
        {
            var tianFuIn = _tianFuStarMappingTable[_ziWeiStarMappingTable[FiveElements][_day]];
            AstrologyChart.FirstOrDefault(x => x.branch == tianFuIn).tianFuStar = GetStarResult(Star.TianFu, tianFuIn);

            for (int s = 1; s < 7; s++)
            {
                var branch = (Branch)(CustomMod(((int)tianFuIn + s), 12));
                AstrologyChart.FirstOrDefault(x => x.branch == branch).tianFuStar = GetStarResult((Star)(s + 7), branch);
            }

            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Yin, (int)tianFuIn, true)).tianFuStar
                = GetStarResult(Star.PoJun, BranchOperation((int)Branch.Yin, (int)tianFuIn, true));
        }

        private void SetMonthStar()
        {
            Branch zuoFuIn = (Branch)(CustomMod(_month + 4, 12));
            AstrologyChart.FirstOrDefault(x => x.branch == zuoFuIn).monthStars.Add(GetStarResult(Star.ZuoFu, zuoFuIn));

            Branch youBuIn = (Branch)(CustomMod(12 - _month, 12));
            AstrologyChart.FirstOrDefault(x => x.branch == youBuIn).monthStars.Add(GetStarResult(Star.YouBu, youBuIn));

            Branch TianXingIn = (Branch)(CustomMod(_month + 9, 12));
            AstrologyChart.FirstOrDefault(x => x.branch == TianXingIn).monthStars.Add(GetStarResult(Star.TianXing, TianXingIn));

            Branch TianYaoIn = (Branch)(CustomMod(_month + 1, 12));
            AstrologyChart.FirstOrDefault(x => x.branch == TianYaoIn).monthStars.Add(GetStarResult(Star.TianYao, TianYaoIn));

            Branch[] YueMaArray = {
                Branch.Shen,
                Branch.Si,
                Branch.Yin,
                Branch.Hai
            };
            AstrologyChart.FirstOrDefault(x => x.branch == YueMaArray[(_month - 1) % 4]).monthStars.Add(GetStarResult(Star.YueMa, YueMaArray[(_month - 1) % 4]));

            Branch JieShenIn = (Branch)(((Math.Ceiling(((double)_month / 2)) + 3) % 6) * 2 + 1);
            AstrologyChart.FirstOrDefault(x => x.branch == JieShenIn).monthStars.Add(GetStarResult(Star.JieShen, JieShenIn));

            Branch[] tianWuArray = {
                Branch.Si,
                Branch.Shen,
                Branch.Yin,
                Branch.Hai
            };
            Branch TianWuIn = tianWuArray[(_month - 1) % 4];
            AstrologyChart.FirstOrDefault(x => x.branch == TianWuIn).monthStars.Add(GetStarResult(Star.TianWu, TianWuIn));

            Branch[] TianYueArray = {
                Branch.Xu,
                Branch.Si,
                Branch.Chen,
                Branch.Yin,
                Branch.Wei,
                Branch.Mao,
                Branch.Hai,
                Branch.Wei,
                Branch.Yin,
                Branch.Wu,
                Branch.Xu,
                Branch.Yin
            };
            AstrologyChart.FirstOrDefault(x => x.branch == TianYueArray[_month - 1]).monthStars.Add(GetStarResult(Star.TianYue, TianYueArray[_month - 1]));

            Branch[] yinShaArray = {
                Branch.Yin,
                Branch.Zi,
                Branch.Xu,
                Branch.Shen,
                Branch.Wu,
                Branch.Chen,
                Branch.Yin,
                Branch.Zi,
                Branch.Xu,
                Branch.Shen,
                Branch.Wu,
                Branch.Chen
            };
            AstrologyChart.FirstOrDefault(x => x.branch == yinShaArray[_month - 1]).monthStars.Add(GetStarResult(Star.YinSha, yinShaArray[_month - 1]));
        }

        private void SetTimeStar()
        {
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Xu, (int)_time, false)).timeStars
                .Add(GetStarResult(Star.WenChang, BranchOperation((int)Branch.Xu, (int)_time, false)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Shen, (int)_time, true)).timeStars
                .Add(GetStarResult(Star.WenQu, BranchOperation((int)Branch.Shen, (int)_time, true)));

            int huoXingBase = 0;
            int lingXingBase = 0;
            if (_branch == Branch.Yin || _branch == Branch.Wu || _branch == Branch.Xu)
            {
                huoXingBase = (int)Branch.Hai;
                lingXingBase = (int)Branch.You;
            }

            if (_branch == Branch.Shen || _branch == Branch.Zi || _branch == Branch.Chen)
            {
                huoXingBase = (int)Branch.Xu;
                lingXingBase = (int)Branch.Yin;
            }

            if (_branch == Branch.Si || _branch == Branch.You || _branch == Branch.Chou)
            {
                huoXingBase = (int)Branch.You;
                lingXingBase = (int)Branch.Yin;
            }

            if (_branch == Branch.Hai || _branch == Branch.Mao || _branch == Branch.Wei)
            {
                huoXingBase = (int)Branch.Mao;
                lingXingBase = (int)Branch.Yin;
            }

            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation(huoXingBase, (int)_time, true)).timeStars.Add(GetStarResult(Star.HuoXing, BranchOperation(huoXingBase, (int)_time, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation(lingXingBase, (int)_time, true)).timeStars.Add(GetStarResult(Star.LingXing, BranchOperation(lingXingBase, (int)_time, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Chou, (int)_time, true)).timeStars.Add(GetStarResult(Star.DiJie, BranchOperation((int)Branch.Chou, (int)_time, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Hai, (int)_time, false)).timeStars.Add(GetStarResult(Star.DiKong, BranchOperation((int)Branch.Hai, (int)_time, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Wu, (int)_time, true)).timeStars.Add(GetStarResult(Star.TaiFu, BranchOperation((int)Branch.Wu, (int)_time, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Xu, (int)_time, true)).timeStars.Add(GetStarResult(Star.FengGqu, BranchOperation((int)Branch.Xu, (int)_time, true)));
        }

        private void SetYearHeavenly()
        {
            Branch[] LuCunArray = {
                Branch.Yin,
                Branch.Mao,
                Branch.Si,
                Branch.Wu,
                Branch.Si,
                Branch.Wu,
                Branch.Shen,
                Branch.You,
                Branch.Hai,
                Branch.Zi
            };
            AstrologyChart.FirstOrDefault(x => x.branch == LuCunArray[(int)_heavenly - 1]).yearHeavenlyStars.Add(GetStarResult(Star.LuCun, LuCunArray[(int)_heavenly - 1]));

            Branch[] QingYangArray = {
                Branch.Mao,
                Branch.Chen,
                Branch.Wu,
                Branch.Wei,
                Branch.Wu,
                Branch.Wei,
                Branch.You,
                Branch.Xu,
                Branch.Zi,
                Branch.Chou
            };
            AstrologyChart.FirstOrDefault(x => x.branch == QingYangArray[(int)_heavenly - 1]).yearHeavenlyStars.Add(GetStarResult(Star.QingYang, QingYangArray[(int)_heavenly - 1]));
            
            Branch[] TuoLuoArray = {
                Branch.Chou,
                Branch.Yin,
                Branch.Chen,
                Branch.Si,
                Branch.Chen,
                Branch.Si,
                Branch.Wei,
                Branch.Shen,
                Branch.Xu,
                Branch.Hai
                       };
            AstrologyChart.FirstOrDefault(x => x.branch == TuoLuoArray[(int)_heavenly - 1]).yearHeavenlyStars.Add(GetStarResult(Star.TuoLuo, TuoLuoArray[(int)_heavenly - 1]));

            Branch[] TianYue_yearArray = {
                Branch.Wei,
                Branch.Shen,
                Branch.You,
                Branch.Hai,
                Branch.Chou,
                Branch.Zi,
                Branch.Chou,
                Branch.Yin,
                Branch.Mao,
                Branch.Si

            };
            AstrologyChart.FirstOrDefault(x => x.branch == TianYue_yearArray[(int)_heavenly - 1]).yearHeavenlyStars.Add(GetStarResult(Star.TianYue_year, TianYue_yearArray[(int)_heavenly - 1]));

            Branch[] TianKuiArray = {
                Branch.Chou,
                Branch.Zi,
                Branch.Hai,
                Branch.You,
                Branch.Wei,
                Branch.Shen,
                Branch.Wei,
                Branch.Wu,
                Branch.Si,
                Branch.Mao
            };
            AstrologyChart.FirstOrDefault(x => x.branch == TianKuiArray[(int)_heavenly - 1]).yearHeavenlyStars.Add(GetStarResult(Star.TianKui, TianKuiArray[(int)_heavenly - 1]));

            Star[] HuaLuArray = {
                Star.LianZhen,
                Star.TianJi,
                Star.TianTong,
                Star.TaiYin,
                Star.TanLang,
                Star.WuQu,
                Star.TaiYang,
                Star.JuMen,
                Star.TianLiang,
                Star.PoJun
            };
            HuaLu = HuaLuArray[(int)_heavenly - 1];

            Star[] HuaChiuanArray = {
                Star.PoJun,
                Star.TianLiang,
                Star.TianJi,
                Star.TianTong,
                Star.TaiYin,
                Star.TanLang,
                Star.WuQu,
                Star.TaiYang,
                Star.ZiWei,
                Star.JuMen
            };
            HuaChiuan = HuaChiuanArray[(int)_heavenly - 1];

            Star[] HuaKeArray = {
                Star.WuQu,
                Star.ZiWei,
                Star.WenChang,
                Star.TianJi,
                Star.YouBu,
                Star.TianLiang,
                Star.TaiYin,
                Star.WenQu,
                Star.ZuoFu,
                Star.TaiYin
            };
            HuaKe = HuaKeArray[(int)_heavenly - 1];

            Star[] HuaJiArray =
            {
                Star.TaiYang,
                Star.TaiYin,
                Star.LianZhen,
                Star.JuMen,
                Star.TianJi,
                Star.WenQu,
                Star.TianTong,
                Star.WenChang,
                Star.WuQu,
                Star.TanLang
            };
            HuaJi = HuaJiArray[(int)_heavenly - 1];
        }

        private void SetYearBranch()
        {
            Branch[] MingMaArray = {
                Branch.Yin,
                Branch.Hai,
                Branch.Shen,
                Branch.Si
            };
            AstrologyChart.FirstOrDefault(x => x.branch == MingMaArray[((int)_branch - 1) % 4]).yearBranchStars.Add(GetStarResult(Star.MingMa, MingMaArray[((int)_branch - 1) % 4]));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Hai, (int)_branch, true)).yearBranchStars.Add(GetStarResult(Star.TianKung, BranchOperation((int)Branch.Hai, (int)_branch, true)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Mao, (int)_branch, false)).yearBranchStars.Add(GetStarResult(Star.HungLuan, BranchOperation((int)Branch.Mao, (int)_branch, false)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.You, (int)_branch, false)).yearBranchStars.Add(GetStarResult(Star.TianShi, BranchOperation((int)Branch.You, (int)_branch, false)));
            Branch[] GuChenArray = {
                Branch.Yin,
                Branch.Si,
                Branch.Shen,
                Branch.Hai
            };
            int test2 = (int)Math.Ceiling(((double)(CustomMod((int)_branch + 1, 12)) / 3)) - 1;
            AstrologyChart.FirstOrDefault(x => x.branch == GuChenArray[test2]).yearBranchStars.Add(GetStarResult(Star.GuChen, GuChenArray[test2]));

            Branch[] GuaSuArray = {
                Branch.Xu,
                Branch.Chou,
                Branch.Chen,
                Branch.Wei
            };
            int test = (int)Math.Ceiling(((double)(CustomMod((int)_branch + 1, 12)) / 3)) - 1;

            AstrologyChart.FirstOrDefault(x => x.branch == GuaSuArray[test]).yearBranchStars.Add(GetStarResult(Star.GuaSu, GuaSuArray[test]));

            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Wu, (int)_branch, false)).yearBranchStars.Add(GetStarResult(Star.TianKu, BranchOperation((int)Branch.Wu, (int)_branch, false)));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Wu, (int)_branch, true)).yearBranchStars.Add(GetStarResult(Star.TianShiu, BranchOperation((int)Branch.Wu, (int)_branch, true)));
            Branch[] HuaGaiArray = {
                Branch.Chen,
                Branch.Chou,
                Branch.Xu,
                Branch.Wei
            };
            AstrologyChart.FirstOrDefault(x => x.branch == HuaGaiArray[((int)_branch - 1) % 4]).yearBranchStars.Add(GetStarResult(Star.HuaGai, HuaGaiArray[((int)_branch - 1) % 4]));
            Branch[] PoSueiArray = {
                Branch.Si,
                Branch.Chou,
                Branch.You,
            };
            AstrologyChart.FirstOrDefault(x => x.branch == PoSueiArray[((int)_branch - 1) % 3]).yearBranchStars.Add(GetStarResult(Star.PoSuei, PoSueiArray[((int)_branch - 1) % 3]));
            AstrologyChart.FirstOrDefault(x => x.branch == BranchOperation((int)Branch.Shen, (int)_branch, true)).yearBranchStars.Add(GetStarResult(Star.LungChih, BranchOperation((int)Branch.Shen, (int)_branch, true)));

            Branch[] FeiLianArray = {
                Branch.Shen,
                Branch.You,
                Branch.Xu,
                Branch.Si,
                Branch.Wu,
                Branch.Wei,
                Branch.Yin,
                Branch.Mao,
                Branch.Chen,
                Branch.Hai,
                Branch.Zi,
                Branch.Chou
            };
            AstrologyChart.FirstOrDefault(x => x.branch == FeiLianArray[((int)_branch - 1) % 4]).yearBranchStars.Add(GetStarResult(Star.FeiLian, FeiLianArray[((int)_branch - 1) % 4]));

            Branch[] ShianChihArray = {
                Branch.You,
                Branch.Wu,
                Branch.Mao,
                Branch.Zi
            };
            AstrologyChart.FirstOrDefault(x => x.branch == ShianChihArray[((int)_branch - 1) % 4]).yearBranchStars.Add(GetStarResult(Star.ShianChih, ShianChihArray[((int)_branch - 1) % 4]));
        }

        private void SetMajorStar()
        {
            Star[] LifeMajorStarArray = {
                Star.TanLang,
                Star.JuMen,
                Star.LuCun,
                Star.WenQu,
                Star.LianZhen,
                Star.PoJun,
                Star.WuQu,
                Star.WuQu,
                Star.LianZhen,
                Star.WenQu,
                Star.LuCun,
                Star.JuMen
            };
            Branch branchOfLifePalace = AstrologyChart.FirstOrDefault(x => x.palace == Palace.Life).branch;
            LifeMajorStar = LifeMajorStarArray[(int)branchOfLifePalace - 1];

            Star[] BodyMajorStarArray = {
                Star.HuoXing,
                Star.TianXiang,
                Star.TianLiang,
                Star.TianTong,
                Star.WenChang,
                Star.TianJi,
                Star.HuoXing,
                Star.TianXiang,
                Star.TianLiang,
                Star.TianTong,
                Star.WenChang,
                Star.TianJi
            };
            BodyMajorStar = BodyMajorStarArray[(int)_branch - 1];
        }

        private void SetStarTypeAndScoreForPalace()
        {
            foreach (var item in AstrologyChart)
            {
                item.MajorStars = item.GetStars().Where(x => _starTypeMapping.Any(t => t.Key == x.Star) && _starTypeMapping[x.Star] == StarType.Major).ToList();
                item.MinorStars = item.GetStars().Where(x => _starTypeMapping.Any(t => t.Key == x.Star) && _starTypeMapping[x.Star] == StarType.Minor).ToList();
                item.RighteousStars = item.GetStars().Where(x => _starTypeMapping.Any(t => t.Key == x.Star) && _starTypeMapping[x.Star] == StarType.Righteous).ToList();
                item.SecondaryStars = item.GetStars().Where(x => _starTypeMapping.Any(t => t.Key == x.Star) && _starTypeMapping[x.Star] == StarType.Secondary).ToList();
                item.Score = 65 + item.GetStars().Sum(s => _starScoreMapping.Any(d => d.Key == s.Star)
                    ? _starScoreMapping[s.Star].Any(status => status.Key == s.Status)
                        ? _starScoreMapping[s.Star][s.Status]
                        : _starScoreMapping[s.Star][StarStatus.Normal]
                    : 0);
                item.Score = item.Score > 95 ? 95 : item.Score;
            }
        }
        #endregion

        #region ShareFunction
        private void LoopNum(ref int num, int max)
        {
            if (num == max)
            {
                num = 1;
            }
            else
            {
                num++;
            }
        }
        private int CustomMod(int num, int max)
        {
            if (num < 0)
            {
                return CustomMod(num + max, max);
            }
            else if (num % max == 0)
            {
                return max;
            }
            return (num % max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseNum">子的位子</param>
        /// <param name="countNum">要帶入的數字</param>
        /// <param name="direction">是順向的或逆向的</param>
        /// <returns></returns>
        private Branch BranchOperation(int baseNum, int countNum, bool direction)
        {
            if (direction)
            {
                return (Branch)(CustomMod(countNum + (13 - baseNum), 12));
            }
            else
             {
                return (Branch)(CustomMod(((13 + (CustomMod(baseNum + 1, 12) - 1)) - countNum), 12));
            }
        }

        private StarResult GetStarResult(Star star, Branch branch)
        {
            return new StarResult
            {
                Star = star,
                Status = _starStatusMapping.Any(s => s.Key == star && s.Value.Any(b => b.Key == branch))
                    ? _starStatusMapping[star][branch]
                    : StarStatus.Normal
            };
        }
        #endregion

        #region MappingTable
        private void InitMappingTable()
        {
            _fiveElementMappingTable = new Dictionary<int, Dictionary<Heavenly, FiveElements>>()
            {
                { 1,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.WaterTwo  },
                        { Heavenly.Ji, FiveElements.WaterTwo },
                        { Heavenly.Yi, FiveElements.FireSix },
                        { Heavenly.Geng, FiveElements.FireSix },
                        { Heavenly.Bing, FiveElements.EarthFive },
                        { Heavenly.Xin, FiveElements.EarthFive },
                        { Heavenly.Ding, FiveElements.WoodThree },
                        { Heavenly.Zen, FiveElements.WoodThree },
                        { Heavenly.Wu, FiveElements.MetalFour },
                        { Heavenly.Gui, FiveElements.MetalFour }
                    }
                },
                { 2,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.FireSix  },
                        { Heavenly.Ji, FiveElements.FireSix },
                        { Heavenly.Yi, FiveElements.EarthFive },
                        { Heavenly.Geng, FiveElements.EarthFive },
                        { Heavenly.Bing, FiveElements.WoodThree },
                        { Heavenly.Xin, FiveElements.WoodThree },
                        { Heavenly.Ding, FiveElements.MetalFour },
                        { Heavenly.Zen, FiveElements.MetalFour },
                        { Heavenly.Wu, FiveElements.WaterTwo },
                        { Heavenly.Gui, FiveElements.WaterTwo }
                    }
                },
                { 3,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.WoodThree  },
                        { Heavenly.Ji, FiveElements.WoodThree },
                        { Heavenly.Yi, FiveElements.MetalFour },
                        { Heavenly.Geng, FiveElements.MetalFour },
                        { Heavenly.Bing, FiveElements.WaterTwo },
                        { Heavenly.Xin, FiveElements.WaterTwo },
                        { Heavenly.Ding, FiveElements.FireSix },
                        { Heavenly.Zen, FiveElements.FireSix },
                        { Heavenly.Wu, FiveElements.EarthFive },
                        { Heavenly.Gui, FiveElements.EarthFive }
                    }
                },
                {                     4,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.EarthFive  },
                        { Heavenly.Ji, FiveElements.EarthFive },
                        { Heavenly.Yi, FiveElements.WoodThree },
                        { Heavenly.Geng, FiveElements.WoodThree },
                        { Heavenly.Bing, FiveElements.MetalFour },
                        { Heavenly.Xin, FiveElements.MetalFour },
                        { Heavenly.Ding, FiveElements.WaterTwo },
                        { Heavenly.Zen, FiveElements.WaterTwo },
                        { Heavenly.Wu, FiveElements.FireSix },
                        { Heavenly.Gui, FiveElements.FireSix }
                    }
                },
                { 5,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.MetalFour  },
                        { Heavenly.Ji, FiveElements.MetalFour },
                        { Heavenly.Yi, FiveElements.WaterTwo },
                        { Heavenly.Geng, FiveElements.WaterTwo },
                        { Heavenly.Bing, FiveElements.FireSix },
                        { Heavenly.Xin, FiveElements.FireSix },
                        { Heavenly.Ding, FiveElements.EarthFive },
                        { Heavenly.Zen, FiveElements.EarthFive },
                        { Heavenly.Wu, FiveElements.WoodThree },
                        { Heavenly.Gui, FiveElements.WoodThree }
                    }
                },
                { 6,
                    new Dictionary<Heavenly, FiveElements>()
                    {
                        { Heavenly.Jia, FiveElements.FireSix  },
                        { Heavenly.Ji, FiveElements.FireSix },
                        { Heavenly.Yi, FiveElements.EarthFive },
                        { Heavenly.Geng, FiveElements.EarthFive },
                        { Heavenly.Bing, FiveElements.WoodThree },
                        { Heavenly.Xin, FiveElements.WoodThree },
                        { Heavenly.Ding, FiveElements.MetalFour },
                        { Heavenly.Zen, FiveElements.MetalFour },
                        { Heavenly.Wu, FiveElements.WaterTwo },
                        { Heavenly.Gui, FiveElements.WaterTwo }
                    }
                }
            };
            _ziWeiStarMappingTable = new Dictionary<FiveElements, Dictionary<int, Branch>>()
            {
                {
                    FiveElements.WaterTwo,
                    new Dictionary<int, Branch>() {
                        { 1, Branch.Chou },
                        { 2, Branch.Yin },
                        { 3, Branch.Yin },
                        { 4, Branch.Mao },
                        { 5, Branch.Mao },
                        { 6, Branch.Chen },
                        { 7, Branch.Chen },
                        { 8, Branch.Si },
                        { 9, Branch.Si },
                        { 10, Branch.Wu },
                        { 11, Branch.Wu },
                        { 12, Branch.Wei },
                        { 13, Branch.Wei },
                        { 14, Branch.Shen },
                        { 15, Branch.Shen },
                        { 16, Branch.You },
                        { 17, Branch.You },
                        { 18, Branch.Xu },
                        { 19, Branch.Xu },
                        { 20, Branch.Hai },
                        { 21, Branch.Hai },
                        { 22, Branch.Zi },
                        { 23, Branch.Zi },
                        { 24, Branch.Chou },
                        { 25, Branch.Chou },
                        { 26, Branch.Yin },
                        { 27, Branch.Yin },
                        { 28, Branch.Mao },
                        { 29, Branch.Mao },
                        { 30, Branch.Chen },
                    }
                },
                {
                    FiveElements.WoodThree,
                    new Dictionary<int, Branch>() {
                        { 1, Branch.Chen },
                        { 2, Branch.Chou },
                        { 3, Branch.Yin },
                        { 4, Branch.Si },
                        { 5, Branch.Yin },
                        { 6, Branch.Mao },
                        { 7, Branch.Wu },
                        { 8, Branch.Mao },
                        { 9, Branch.Chen },
                        { 10, Branch.Wei },
                        { 11, Branch.Chen },
                        { 12, Branch.Si },
                        { 13, Branch.Shen },
                        { 14, Branch.Si },
                        { 15, Branch.Wu },
                        { 16, Branch.You },
                        { 17, Branch.Wu },
                        { 18, Branch.Wei },
                        { 19, Branch.Xu },
                        { 20, Branch.Wei },
                        { 21, Branch.Shen },
                        { 22, Branch.Hai },
                        { 23, Branch.Shen },
                        { 24, Branch.You },
                        { 25, Branch.Zi },
                        { 26, Branch.You },
                        { 27, Branch.Xu },
                        { 28, Branch.Chou },
                        { 29, Branch.Xu },
                        { 30, Branch.Hai },
                    }
                },
                {
                    FiveElements.MetalFour,
                    new Dictionary<int, Branch>() {
                        { 1, Branch.Hai },
                        { 2, Branch.Chen },
                        { 3, Branch.Chou },
                        { 4, Branch.Yin },
                        { 5, Branch.Zi },
                        { 6, Branch.Si },
                        { 7, Branch.Yin },
                        { 8, Branch.Mao },
                        { 9, Branch.Chou },
                        { 10, Branch.Wu },
                        { 11, Branch.Mao },
                        { 12, Branch.Chen },
                        { 13, Branch.Yin },
                        { 14, Branch.Wei },
                        { 15, Branch.Chen },
                        { 16, Branch.Si },
                        { 17, Branch.Mao },
                        { 18, Branch.Shen },
                        { 19, Branch.Si },
                        { 20, Branch.Wu },
                        { 21, Branch.Chen },
                        { 22, Branch.You },
                        { 23, Branch.Wu },
                        { 24, Branch.Wei },
                        { 25, Branch.Si },
                        { 26, Branch.Xu },
                        { 27, Branch.Wei },
                        { 28, Branch.Shen },
                        { 29, Branch.Wu },
                        { 30, Branch.Hai },
                    }
                },
                {
                    FiveElements.EarthFive,
                    new Dictionary<int, Branch>() {
                        { 1, Branch.Wu },
                        { 2, Branch.Hai },
                        { 3, Branch.Chen },
                        { 4, Branch.Chou },
                        { 5, Branch.Yin },
                        { 6, Branch.Wei },
                        { 7, Branch.Zi },
                        { 8, Branch.Si },
                        { 9, Branch.Yin },
                        { 10, Branch.Mao },
                        { 11, Branch.Shen },
                        { 12, Branch.Chou },
                        { 13, Branch.Wu },
                        { 14, Branch.Mao },
                        { 15, Branch.Chen },
                        { 16, Branch.You },
                        { 17, Branch.Yin },
                        { 18, Branch.Wei },
                        { 19, Branch.Chen },
                        { 20, Branch.Si },
                        { 21, Branch.Xu },
                        { 22, Branch.Mao },
                        { 23, Branch.Shen },
                        { 24, Branch.Si },
                        { 25, Branch.Wu },
                        { 26, Branch.Hai },
                        { 27, Branch.Chen },
                        { 28, Branch.You },
                        { 29, Branch.Wu },
                        { 30, Branch.Wei },
                    }
                },
                {
                    FiveElements.FireSix,
                    new Dictionary<int, Branch>() {
                        { 1, Branch.You },
                        { 2, Branch.Wu },
                        { 3, Branch.Hai },
                        { 4, Branch.Chen },
                        { 5, Branch.Chou },
                        { 6, Branch.Yin },
                        { 7, Branch.Xu },
                        { 8, Branch.Wei },
                        { 9, Branch.Chen },
                        { 10, Branch.Si },
                        { 11, Branch.Yin },
                        { 12, Branch.Mao },
                        { 13, Branch.Hai },
                        { 14, Branch.Shen },
                        { 15, Branch.Chou },
                        { 16, Branch.Wu },
                        { 17, Branch.Mao },
                        { 18, Branch.Chen },
                        { 19, Branch.Zi },
                        { 20, Branch.You },
                        { 21, Branch.Yin },
                        { 22, Branch.Wei },
                        { 23, Branch.Chen },
                        { 24, Branch.Si },
                        { 25, Branch.Chou },
                        { 26, Branch.Xu },
                        { 27, Branch.Mao },
                        { 28, Branch.Shen },
                        { 29, Branch.Si },
                        { 30, Branch.Wu },
                    }
                }
            };
            _ziWeiStarsMappingTable = new Dictionary<Branch, Dictionary<Star, Branch>>()
            {
                {
                    Branch.Zi,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Hai },
                        { Star.TaiYang, Branch.You },
                        { Star.WuQu, Branch.Shen },
                        { Star.TianTong, Branch.Wei },
                        { Star.LianZhen, Branch.Chen }
                    }
                },
                {
                    Branch.Chou,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Zi },
                        { Star.TaiYang, Branch.Xu },
                        { Star.WuQu, Branch.You },
                        { Star.TianTong, Branch.Shen },
                        { Star.LianZhen, Branch.Si }
                    }
                },
                {
                    Branch.Yin,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Chou },
                        { Star.TaiYang, Branch.Hai },
                        { Star.WuQu, Branch.Xu },
                        { Star.TianTong, Branch.You },
                        { Star.LianZhen, Branch.Wu }
                    }
                },
                {
                    Branch.Mao,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Yin },
                        { Star.TaiYang, Branch.Zi },
                        { Star.WuQu, Branch.Hai },
                        { Star.TianTong, Branch.Xu },
                        { Star.LianZhen, Branch.Wei }
                    }
                },
                {
                    Branch.Chen,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Mao },
                        { Star.TaiYang, Branch.Chou },
                        { Star.WuQu, Branch.Zi },
                        { Star.TianTong, Branch.Hai },
                        { Star.LianZhen, Branch.Shen }
                    }
                },
                {
                    Branch.Si,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Chen },
                        { Star.TaiYang, Branch.Yin },
                        { Star.WuQu, Branch.Chou },
                        { Star.TianTong, Branch.Zi },
                        { Star.LianZhen, Branch.You }
                    }
                },
                {
                    Branch.Wu,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Si },
                        { Star.TaiYang, Branch.Mao },
                        { Star.WuQu, Branch.Yin },
                        { Star.TianTong, Branch.Chou },
                        { Star.LianZhen, Branch.Xu }
                    }
                },
                {
                    Branch.Wei,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Wu },
                        { Star.TaiYang, Branch.Chen },
                        { Star.WuQu, Branch.Mao },
                        { Star.TianTong, Branch.Yin },
                        { Star.LianZhen, Branch.Hai }
                    }
                },
                {
                    Branch.Shen,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Wei },
                        { Star.TaiYang, Branch.Si },
                        { Star.WuQu, Branch.Chen },
                        { Star.TianTong, Branch.Mao },
                        { Star.LianZhen, Branch.Zi }
                    }
                },
                {
                    Branch.You,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Shen },
                        { Star.TaiYang, Branch.Wu },
                        { Star.WuQu, Branch.Si },
                        { Star.TianTong, Branch.Chen },
                        { Star.LianZhen, Branch.Chou }
                    }
                },
                {
                    Branch.Xu,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.You },
                        { Star.TaiYang, Branch.Wei },
                        { Star.WuQu, Branch.Wu },
                        { Star.TianTong, Branch.Si },
                        { Star.LianZhen, Branch.Yin }
                    }
                },
                {
                    Branch.Hai,
                    new Dictionary<Star, Branch>
                    {
                        { Star.TianJi, Branch.Xu },
                        { Star.TaiYang, Branch.Shen },
                        { Star.WuQu, Branch.Wei },
                        { Star.TianTong, Branch.Wu },
                        { Star.LianZhen, Branch.Mao }
                    }
                },
            };
            _tianFuStarMappingTable = new Dictionary<Branch, Branch>()
            {
                { Branch.Zi, Branch.Chen },
                { Branch.Chou, Branch.Mao },
                { Branch.Yin, Branch.Yin },
                { Branch.Mao, Branch.Chou },
                { Branch.Chen, Branch.Zi },
                { Branch.Si, Branch.Hai },
                { Branch.Wu, Branch.Xu },
                { Branch.Wei, Branch.You },
                { Branch.Shen, Branch.Shen },
                { Branch.You, Branch.Wei },
                { Branch.Xu, Branch.Wu },
                { Branch.Hai, Branch.Si }
            };
            _starStatusMapping = new Dictionary<Star, Dictionary<Branch, StarStatus>>()
            {
                {
                    Star.ZiWei,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Bad },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Good },
                        { Branch.Si, StarStatus.VeryGood },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.VeryGood },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.Good },
                        { Branch.Hai, StarStatus.VeryGood }
                    }
                },
                {
                    Star.TianJi,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.Terrible },
                        { Branch.Yin, StarStatus.Good },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Normal },
                        { Branch.Si, StarStatus.Bad },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.Terrible },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.Normal },
                        { Branch.Hai, StarStatus.Bad }
                    }
                },
                {
                    Star.TaiYang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Terrible },
                        { Branch.Chou, StarStatus.VeryBad },
                        { Branch.Yin, StarStatus.VeryGood },
                        { Branch.Mao, StarStatus.Excellent },
                        { Branch.Chen, StarStatus.VeryGood },
                        { Branch.Si, StarStatus.VeryGood },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.Good },
                        { Branch.Shen, StarStatus.Normal },
                        { Branch.You, StarStatus.Bad },
                        { Branch.Xu, StarStatus.Terrible },
                        { Branch.Hai, StarStatus.Terrible }
                    }
                },
                {
                    Star.WuQu,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Good },
                        { Branch.Mao, StarStatus.Normal },
                        { Branch.Chen, StarStatus.Excellent },
                        { Branch.Si, StarStatus.Bad },
                        { Branch.Wu, StarStatus.VeryGood },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.Normal },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Bad }
                    }
                },
                {
                    Star.TianTong,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.VeryBad },
                        { Branch.Yin, StarStatus.Normal },
                        { Branch.Mao, StarStatus.Excellent },
                        { Branch.Chen, StarStatus.Bad },
                        { Branch.Si, StarStatus.Excellent },
                        { Branch.Wu, StarStatus.Terrible },
                        { Branch.Wei, StarStatus.VeryBad },
                        { Branch.Shen, StarStatus.VeryGood },
                        { Branch.You, StarStatus.Bad },
                        { Branch.Xu, StarStatus.Bad },
                        { Branch.Hai, StarStatus.Excellent }
                    }
                },
                {
                    Star.LianZhen,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Bad },
                        { Branch.Chou, StarStatus.Normal },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Bad },
                        { Branch.Chen, StarStatus.Normal },
                        { Branch.Si, StarStatus.Terrible },
                        { Branch.Wu, StarStatus.Bad },
                        { Branch.Wei, StarStatus.Normal },
                        { Branch.Shen, StarStatus.Excellent },
                        { Branch.You, StarStatus.Bad },
                        { Branch.Xu, StarStatus.Normal },
                        { Branch.Hai, StarStatus.Terrible }
                    }
                },
                {
                    Star.TianFu,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Good },
                        { Branch.Chen, StarStatus.VeryGood },
                        { Branch.Si, StarStatus.Good },
                        { Branch.Wu, StarStatus.VeryGood },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.VeryGood },
                        { Branch.Hai, StarStatus.Good }
                    }
                },
                {
                    Star.TaiYin,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.VeryBad },
                        { Branch.Mao, StarStatus.Terrible },
                        { Branch.Chen, StarStatus.Terrible },
                        { Branch.Si, StarStatus.Terrible },
                        { Branch.Wu, StarStatus.Terrible },
                        { Branch.Wei, StarStatus.Bad },
                        { Branch.Shen, StarStatus.Normal },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.VeryGood },
                        { Branch.Hai, StarStatus.Excellent }
                    }
                },
                {
                    Star.TanLang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Bad },
                        { Branch.Mao, StarStatus.Good },
                        { Branch.Chen, StarStatus.Excellent },
                        { Branch.Si, StarStatus.Terrible },
                        { Branch.Wu, StarStatus.VeryGood },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Bad },
                        { Branch.You, StarStatus.Normal },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Terrible }
                    }
                },
                {
                    Star.JuMen,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.VeryBad },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Excellent },
                        { Branch.Chen, StarStatus.Bad },
                        { Branch.Si, StarStatus.Bad },
                        { Branch.Wu, StarStatus.VeryGood },
                        { Branch.Wei, StarStatus.Bad },
                        { Branch.Shen, StarStatus.Excellent },
                        { Branch.You, StarStatus.Excellent },
                        { Branch.Xu, StarStatus.Bad },
                        { Branch.Hai, StarStatus.VeryGood }
                    }
                },
                {
                    Star.TianXiang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Terrible },
                        { Branch.Chen, StarStatus.Bad },
                        { Branch.Si, StarStatus.Good },
                        { Branch.Wu, StarStatus.Good },
                        { Branch.Wei, StarStatus.Good },
                        { Branch.Shen, StarStatus.Excellent },
                        { Branch.You, StarStatus.Terrible },
                        { Branch.Xu, StarStatus.Good },
                        { Branch.Hai, StarStatus.Good }
                    }
                },
                {
                    Star.TianLiang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.VeryGood },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Excellent },
                        { Branch.Si, StarStatus.Terrible },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.VeryGood },
                        { Branch.Shen, StarStatus.Terrible },
                        { Branch.You, StarStatus.Good },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Terrible }
                    }
                },
                {
                    Star.QuSha,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Good },
                        { Branch.Si, StarStatus.Bad },
                        { Branch.Wu, StarStatus.VeryGood },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Excellent },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Bad }
                    }
                },
                {
                    Star.PoJun,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Excellent },
                        { Branch.Chou, StarStatus.VeryGood },
                        { Branch.Yin, StarStatus.Good },
                        { Branch.Mao, StarStatus.Terrible },
                        { Branch.Chen, StarStatus.VeryGood },
                        { Branch.Si, StarStatus.Bad },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.VeryGood },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.Terrible },
                        { Branch.Xu, StarStatus.VeryGood },
                        { Branch.Hai, StarStatus.Bad }
                    }
                },
                {
                    Star.HuoXing,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Terrible },
                        { Branch.Chou, StarStatus.Good },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Normal },
                        { Branch.Chen, StarStatus.Terrible },
                        { Branch.Si, StarStatus.Good },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.Normal },
                        { Branch.Shen, StarStatus.Terrible },
                        { Branch.You, StarStatus.Good },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Normal }
                    }
                },
                {
                    Star.LingXing,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Terrible },
                        { Branch.Chou, StarStatus.Good },
                        { Branch.Yin, StarStatus.Excellent },
                        { Branch.Mao, StarStatus.Normal },
                        { Branch.Chen, StarStatus.Terrible },
                        { Branch.Si, StarStatus.Good },
                        { Branch.Wu, StarStatus.Excellent },
                        { Branch.Wei, StarStatus.Normal },
                        { Branch.Shen, StarStatus.Terrible },
                        { Branch.You, StarStatus.Good },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Normal }
                    }
                },
                {
                    Star.QingYang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.VeryGood },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Normal },
                        { Branch.Mao, StarStatus.Terrible },
                        { Branch.Chen, StarStatus.Excellent },
                        { Branch.Si, StarStatus.Normal },
                        { Branch.Wu, StarStatus.Terrible },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Normal },
                        { Branch.You, StarStatus.VeryGood },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Normal }
                    }
                },
                {
                    Star.TuoLuo,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Normal },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Terrible },
                        { Branch.Mao, StarStatus.Normal },
                        { Branch.Chen, StarStatus.Excellent },
                        { Branch.Si, StarStatus.Terrible },
                        { Branch.Wu, StarStatus.Normal },
                        { Branch.Wei, StarStatus.Excellent },
                        { Branch.Shen, StarStatus.Terrible },
                        { Branch.You, StarStatus.Normal },
                        { Branch.Xu, StarStatus.Excellent },
                        { Branch.Hai, StarStatus.Terrible }
                    }
                },
                {
                    Star.WenChang,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Good },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Terrible },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Good },
                        { Branch.Si, StarStatus.Excellent },
                        { Branch.Wu, StarStatus.Terrible },
                        { Branch.Wei, StarStatus.Normal },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.Excellent },
                        { Branch.Xu, StarStatus.Terrible },
                        { Branch.Hai, StarStatus.Normal }
                    }
                },
                {
                    Star.WenQu,
                    new Dictionary<Branch, StarStatus>
                    {
                        { Branch.Zi, StarStatus.Good },
                        { Branch.Chou, StarStatus.Excellent },
                        { Branch.Yin, StarStatus.Bad },
                        { Branch.Mao, StarStatus.VeryGood },
                        { Branch.Chen, StarStatus.Good },
                        { Branch.Si, StarStatus.Excellent },
                        { Branch.Wu, StarStatus.Terrible },
                        { Branch.Wei, StarStatus.VeryGood },
                        { Branch.Shen, StarStatus.Good },
                        { Branch.You, StarStatus.Excellent },
                        { Branch.Xu, StarStatus.Terrible },
                        { Branch.Hai, StarStatus.VeryGood }
                    }
                },
            };
            _starTypeMapping = new Dictionary<Star, StarType>()
            {
                { Star.ZiWei, StarType.Major },
                { Star.TianJi, StarType.Major },
                { Star.TaiYang, StarType.Major },
                { Star.WuQu, StarType.Major },
                { Star.TianTong, StarType.Major },
                { Star.LianZhen, StarType.Major },
                { Star.TianFu, StarType.Major },
                { Star.TaiYin, StarType.Righteous },
                { Star.TanLang, StarType.Righteous },
                { Star.JuMen, StarType.Minor },
                { Star.TianXiang, StarType.Minor },
                { Star.TianLiang, StarType.Minor },
                { Star.QuSha, StarType.Righteous },
                { Star.PoJun, StarType.Righteous },
                { Star.ZuoFu, StarType.Minor },
                { Star.YouBu, StarType.Minor },
                { Star.TianXing, StarType.Secondary },
                { Star.TianYao, StarType.Secondary },
                { Star.WenChang, StarType.Minor },
                { Star.WenQu, StarType.Minor },
                { Star.HuoXing, StarType.Minor },
                { Star.LingXing, StarType.Minor },
                { Star.DiJie, StarType.Minor },
                { Star.DiKong, StarType.Minor },
                { Star.LuCun, StarType.Minor },
                { Star.QingYang, StarType.Secondary },
                { Star.TuoLuo, StarType.Secondary },
                { Star.HungLuan, StarType.Secondary },
                { Star.TianShi, StarType.Secondary },
                { Star.GuaSu, StarType.Secondary },
                { Star.GuChen, StarType.Secondary },
                { Star.TianKui, StarType.Secondary },
                { Star.TianYue_year, StarType.Minor },
                { Star.MingMa, StarType.Minor }
            };
            _starScoreMapping = new Dictionary<Star, Dictionary<StarStatus, int>>()
            {
                { Star.ZiWei, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 18 }
                    }
                },
                { Star.TianJi, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 18 }
                    }
                },
                { Star.TaiYang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Excellent, 15 },
                        { StarStatus.VeryGood, 15 },
                        { StarStatus.Good, 15 },
                        { StarStatus.Normal, 15 },
                        { StarStatus.Bad, 5 },
                        { StarStatus.VeryBad, 5 },
                        { StarStatus.Terrible, 5 }
                    }
                },
                { Star.WuQu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 10 }
                    }
                },
                { Star.TianTong, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 15 }
                    }
                },
                { Star.LianZhen, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 15 }
                    }
                },
                { Star.TianFu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 12 }
                    }
                },
                { Star.TaiYin, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Excellent, 12 },
                        { StarStatus.VeryGood, 12 },
                        { StarStatus.Good, 12 },
                        { StarStatus.Normal, 12 },
                        { StarStatus.Bad, 5 },
                        { StarStatus.VeryBad, 5 },
                        { StarStatus.Terrible, 5 }
                    }
                },
                { Star.TanLang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -12 }
                    }
                },
                { Star.JuMen, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 12 }
                    }
                },
                { Star.TianXiang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 12 }
                    }
                },
                { Star.TianLiang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 12 }
                    }
                },
                { Star.QuSha, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -15 }
                    }
                },
                { Star.PoJun, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -12 }
                    }
                },
                { Star.ZuoFu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.YouBu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.TianXing, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.TianYao, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.WenChang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.WenQu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.HuoXing, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.LingXing, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.DiJie, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.DiKong, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.LuCun, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.QingYang, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.TuoLuo, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -5 }
                    }
                },
                { Star.MingMa, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.HungLuan, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 3 }
                    }
                },
                { Star.TianShi, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 3 }
                    }
                },
                { Star.GuChen, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -3 }
                    }
                },
                { Star.GuaSu, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, -3 }
                    }
                },
                { Star.TianKui, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
                { Star.TianYue_year, new Dictionary<StarStatus, int>()
                    {
                        { StarStatus.Normal, 5 }
                    }
                },
            };
        }
        #endregion
    }

    public class AstrologyChart
    {
        public AstrologyChart()
        {
            monthStars = new List<StarResult>();
            timeStars = new List<StarResult>();
            yearHeavenlyStars = new List<StarResult>();
            yearBranchStars = new List<StarResult>();
        }

        public Palace palace { get; set; }
        public bool isBodyPalace { get; set; }
        public Heavenly heavenly { get; set; }
        public Branch branch { get; set; }
        public StarResult ziWeiStar { get; set; }
        public StarResult tianFuStar { get; set; }
        public List<StarResult> monthStars { get; set; }
        public List<StarResult> timeStars { get; set; }
        public List<StarResult> yearHeavenlyStars { get; set; }
        public List<StarResult> yearBranchStars { get; set; }
        public List<StarResult> GetStars()
        {
            var stars = new List<StarResult>();
            if (ziWeiStar != null)
            {
                stars.Add(ziWeiStar);
            }

            if (tianFuStar != null)
            {
                stars.Add(tianFuStar);
            }

            stars = stars.Concat(monthStars).Concat(timeStars).Concat(yearBranchStars).Concat(yearHeavenlyStars).ToList();
            return stars;
        }
        public List<StarResult> MajorStars { get; set; }
        public List<StarResult> MinorStars { get; set; }
        public List<StarResult> RighteousStars { get; set; }
        public List<StarResult> SecondaryStars { get; set; }
        public int Score { get; set; }
    }

    public class StarResult
    {
        public Star Star { get; set; }
        public StarStatus Status { get; set; }
    }

    #region Base Property
    public enum Palace
    {
        Life = 1,
        Parents = 2,
        Happiness = 3,
        Property = 4,
        Career = 5,
        Friends = 6,
        Travel = 7,
        Health = 8,
        Weealth = 9,
        Children = 10,
        Marriage = 11,
        Siblings = 12
    }

    public enum FiveElements
    {
        WaterTwo = 2,
        WoodThree = 3,
        MetalFour = 4,
        EarthFive = 5,
        FireSix = 6
    }

    public enum Heavenly
    {
        Jia = 1,
        Yi,
        Bing,
        Ding,
        Wu,
        Ji,
        Geng,
        Xin,
        Zen,
        Gui
    }

    public enum Branch
    {
        Zi = 1,
        Chou,
        Yin,
        Mao,
        Chen,
        Si,
        Wu,
        Wei,
        Shen,
        You,
        Xu,
        Hai
    }

    public enum BirthTimePeriod
    {
        Zi = 1,
        Chou,
        Yin,
        Mao,
        Chen,
        Si,
        Wu,
        Wei,
        Shen,
        You,
        Xu,
        Hai
    }

    #endregion

    #region All Star
    public enum Star
    {
        ZiWei = 1,
        TianJi = 2,
        TaiYang = 3,
        WuQu = 4,
        TianTong = 5,
        LianZhen = 6,
        TianFu = 7,
        TaiYin = 8,
        TanLang = 9,
        JuMen = 10,
        TianXiang = 11,
        TianLiang = 12,
        QuSha = 13,
        PoJun = 14,
        ZuoFu = 15,
        YouBu = 16,
        TianXing = 17,
        TianYao = 18,
        YueMa = 19,
        JieShen = 20,
        TianWu = 21,
        TianYue = 22,
        YinSha = 23,
        WenChang = 24,
        WenQu = 25,
        HuoXing = 26,
        LingXing = 27,
        DiJie = 28,
        DiKong = 29,
        TaiFu = 30,
        FengGqu = 31,
        LuCun = 32,
        QingYang = 33,
        TuoLuo = 34,
        TianYue_year = 35,
        TianKui = 36,
        MingMa = 37,
        TianKung = 38,
        HungLuan = 39,
        TianShi = 40,
        GuChen = 41,
        GuaSu = 42,
        TianKu = 43,
        TianShiu = 44,
        HuaGai = 45,
        PoSuei = 46,
        LungChih = 47,
        FeiLian = 48,
        ShianChih = 49
    }

    public enum StarType
    {
        Major,
        Minor,
        Righteous,
        Secondary
    }

    public enum StarStatus
    {
        Terrible = -3,
        VeryBad = -2,
        Bad = -1,
        Normal = 0,
        Good = 1,
        VeryGood = 2,
        Excellent = 3,
    }
    #endregion

}
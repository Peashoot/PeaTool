using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PeaTool.DataObject
{
    /// <summary>
    /// 基金基础信息
    /// </summary>
    public class FundBasicInfoData
    {
        /// <summary>
        /// 基金代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 基金名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 净值日期
        /// </summary>
        public string netWorthDate { get; set; }
        /// <summary>
        /// 净值
        /// </summary>
        public float netWorth { get; set; }
        /// <summary>
        /// 净值涨幅
        /// </summary>
        public string dayGrowth { get; set; }
        /// <summary>
        /// 估值时间
        /// </summary>
        public string expectWorthDate { get; set; }
        /// <summary>
        /// 估值
        /// </summary>
        [JsonProperty("expectWorth")]
        public float price { get; set; }
        /// <summary>
        /// 估计涨幅
        /// </summary>
        [JsonProperty("expectGrowth")]
        public string changePercent { get; set; }
        /// <summary>
        /// 近一周涨幅
        /// </summary>
        public string lastWeekGrowth { get; set; }
        /// <summary>
        /// 近一月涨幅
        /// </summary>
        public string lastMonthGrowth { get; set; }
        /// <summary>
        /// 近三月涨幅
        /// </summary>
        public string lastThreeMonthsGrowth { get; set; }
        /// <summary>
        /// 近半年涨幅
        /// </summary>
        public string lastSixMonthsGrowth { get; set; }
        /// <summary>
        /// 近一年涨幅
        /// </summary>
        public string lastYearGrowth { get; set; }
    }
    public class FundBasicInfoResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<FundBasicInfoData> data { get; set; }
    }
}

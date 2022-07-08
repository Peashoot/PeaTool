using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PeaTool.DataObject
{
    /// <summary>
    /// 板块数据
    /// </summary>
    public class BoardQuotationData
    {
        public int f1 { get; set; }
        /// <summary>
        /// 最新价
        /// </summary>
        [JsonProperty("f2")]
        public float price { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        [JsonProperty("f3")]
        public float changePercent { get; set; }
        /// <summary>
        /// 涨跌额
        /// </summary>
        [JsonProperty("f4")]
        public float priceChange { get; set; }
        /// <summary>
        /// 成交量(手)
        /// </summary>
        [JsonProperty("f5")]
        public long volume { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        [JsonProperty("f6")]
        public float turnover { get; set; }
        /// <summary>
        /// 振幅
        /// </summary>
        public float f7 { get; set; }
        /// <summary>
        /// 换手率
        /// </summary>
        [JsonProperty("f8")]
        public float handsRate { get; set; }
        /// <summary>
        /// 市盈率(动态)
        /// </summary>
        [JsonProperty("f9")]
        public double peRatio { get; set; }
        /// <summary>
        /// 量比
        /// </summary>
        public float f10 { get; set; }
        /// <summary>
        /// 5分钟涨跌
        /// </summary>
        [JsonProperty("f11")]
        public float riseInFive { get; set; }
        /// <summary>
        /// 代码
        /// </summary>
        [JsonProperty("f12")]
        public string code { get; set; }
        /// <summary>
        /// 所属交易所
        /// </summary>
        [JsonProperty("f13")]
        public int exchange { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("f14")]
        public string name { get; set; }
        /// <summary>
        /// 最高
        /// </summary>
        [JsonProperty("f15")]
        public float high { get; set; }
        /// <summary>
        /// 最低
        /// </summary>
        [JsonProperty("f16")]
        public float low { get; set; }
        /// <summary>
        /// 今开
        /// </summary>
        [JsonProperty("f17")]
        public float open { get; set; }
        /// <summary>
        /// 昨收
        /// </summary>
        [JsonProperty("f18")]
        public float close { get; set; }
        /// <summary>
        /// 总市值
        /// </summary>
        public long f20 { get; set; }
        /// <summary>
        /// 流通市值
        /// </summary>
        public long f21 { get; set; }
        /// <summary>
        /// 涨速
        /// </summary>
        public float f22 { get; set; }
        /// <summary>
        /// 市净率
        /// </summary>
        public string f23 { get; set; }
        /// <summary>
        /// 60日涨跌幅
        /// </summary>
        public float f24 { get; set; }
        /// <summary>
        /// 年初至今涨跌幅
        /// </summary>
        public float f25 { get; set; }
        /// <summary>
        /// 上市日期
        /// </summary>
        public string f26 { get; set; }
        /// <summary>
        /// 委比
        /// </summary>
        public float f33 { get; set; }
        /// <summary>
        /// 主力净流入
        /// </summary>
        public float f62 { get; set; }
        /// <summary>
        /// 上涨家数
        /// </summary>
        public long f104 { get; set; }
        /// <summary>
        /// 下跌家数
        /// </summary>
        public long f105 { get; set; }
        /// <summary>
        /// 市盈率
        /// </summary>
        public string f115 { get; set; }
        /// <summary>
        /// 最新行情时间
        /// </summary>
        public int f124 { get; set; }
        /// <summary>
        /// 领涨股
        /// </summary>
        public string f128 { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public float f136 { get; set; }
        /// <summary>
        /// 领跌股
        /// </summary>
        public string f207 { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public float f222 { get; set; }
    }

    public class BoardQuotationResponse
    {
        public int rc { get; set; }
        public int rt { get; set; }
        public long svr { get; set; }
        public int lt { get; set; }
        public int full { get; set; }
        public BoardQuotationResponseData data { get; set; }
    }
    public class BoardQuotationResponseData
    {
        public int total { get; set; }
        public List<BoardQuotationData> diff { get; set; }
    }
}

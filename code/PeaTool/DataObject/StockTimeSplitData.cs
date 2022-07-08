using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeaTool.DataObject
{
    /// <summary>
    /// 股票分时数据
    /// </summary>
    public class StockTimeSplitData
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 涨跌额
        /// </summary>
        public string priceChange { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public string changePercent { get; set; }
        /// <summary>
        /// 昨开
        /// </summary>
        public string open { get; set; }
        /// <summary>
        /// 昨收
        /// </summary>
        public string close { get; set; }
        /// <summary>
        /// 现在
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 最高点
        /// </summary>
        public string high { get; set; }
        /// <summary>
        /// 最低点
        /// </summary>
        public string low { get; set; }
        /// <summary>
        /// 交易量
        /// </summary>
        public string volume { get; set; }
        /// <summary>
        /// 交易额
        /// </summary>
        public string turnover { get; set; }
        /// <summary>
        /// 数据刷新时间
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// 分时数据
        /// [1] 时间（HHmm）
        /// [2] 当时价格
        /// [3] 交易量
        /// </summary>
        public string[][] minData { get; set; }
    }

    public class StockTimeSplitResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public StockTimeSplitData data { get; set; }
    }
}

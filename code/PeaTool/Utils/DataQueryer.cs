using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PeaTool.DataObject;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PeaTool.Utils
{
    public class DataQueryer
    {
        static DataQueryer()
        {
            Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";


                //空值处理
                setting.NullValueHandling = NullValueHandling.Ignore;

                //高级用法九中的Bool类型转换 设置
                //setting.Converters.Add(new BoolConvert("是,否"));

                if (setting.Converters.FirstOrDefault(p => p.GetType() == typeof(JsonCustomNumberConvert<>)) == null)
                {
                    setting.Converters.Add(new JsonCustomNumberConvert<int>());
                    setting.Converters.Add(new JsonCustomNumberConvert<float>());
                    setting.Converters.Add(new JsonCustomNumberConvert<double>());
                    setting.Converters.Add(new JsonCustomNumberConvert<long>());
                    setting.Converters.Add(new JsonCustomNumberConvert<decimal>());
                }

                return setting;
            });
        }
        /// <summary>
        /// 查询板块内容
        /// </summary>
        public static async Task<List<BoardQuotationData>> QueryBoardData()
        {
            try
            {
                string queryUrl = "http://89.push2.eastmoney.com/api/qt/clist/get";
                RestClient client = new RestClient(queryUrl);
                client.Timeout = 3000;
                long timestmap = (long)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
                RestRequest request = new RestRequest(Method.GET);
                request.AddQueryParameter("pn", "1");
                request.AddQueryParameter("pz", "1000"); // 每页大小
                request.AddQueryParameter("cb", "jQuery112406062011075107034_" + timestmap.ToString());
                request.AddQueryParameter("po", "1");
                request.AddQueryParameter("np", "1");
                request.AddQueryParameter("ut", "bd1d9ddb04089700cf9c27f6f7426281");
                request.AddQueryParameter("fltt", "2");
                request.AddQueryParameter("invt", "2");
                request.AddQueryParameter("fid", "f3"); // 排序字段
                request.AddQueryParameter("fs", "m:90+t:2+f:!50"); // 证券过滤器
                request.AddQueryParameter("fields", "f2,f3,f4,f12,f13,f14,f014,f105");
                request.AddQueryParameter("_", (timestmap + 1).ToString());
                var uri = client.BuildUri(request);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
                string content = response.Content;
                Regex pattern = new Regex(@"jQuery[^\(]+\(([^\)]+)\)");
                Match match = pattern.Match(content);
                content = match.Groups[1].ToString();
                BoardQuotationResponse responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<BoardQuotationResponse>(content);
                if (responseData.data != null && responseData.data.diff != null && responseData.data.diff.Count > 0)
                {
                    return responseData.data.diff.OrderBy(item => item.changePercent).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// 获取股票分时数据
        /// </summary>
        /// <param name="code">股票代码</param>
        public static async Task<List<BoardQuotationData>> QueryStockData(List<string> codeList)
        {
            try
            {
                string queryUrl = "http://89.push2.eastmoney.com/api/qt/clist/get";
                RestClient client = new RestClient(queryUrl);
                client.Timeout = 3000;
                long timestmap = (long)(DateTime.Now - new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
                RestRequest request = new RestRequest(Method.GET);
                request.AddQueryParameter("pn", "1");
                request.AddQueryParameter("pz", "50"); // 每页大小
                request.AddQueryParameter("cb", "jQuery1124039519549154685896_" + timestmap.ToString());
                request.AddQueryParameter("po", "1");
                request.AddQueryParameter("np", "1");
                request.AddQueryParameter("ut", "bd1d9ddb04089700cf9c27f6f7426281");
                request.AddQueryParameter("fltt", "2");
                request.AddQueryParameter("invt", "2");
                request.AddQueryParameter("fs", "b:MK0010"); // 证券过滤器
                request.AddQueryParameter("fields", "f2,f3,f4,f5,f6,f7,f11,f12,f13,f14");
                request.AddQueryParameter("_", (timestmap + 1).ToString());
                var uri = client.BuildUri(request);
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
                string content = response.Content;
                Regex pattern = new Regex(@"jQuery[^\(]+\(([^\)]+)\)");
                Match match = pattern.Match(content);
                content = match.Groups[1].ToString();
                BoardQuotationResponse responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<BoardQuotationResponse>(content);
                if (responseData.data != null && responseData.data.diff != null && responseData.data.diff.Count > 0)
                {
                    return responseData.data.diff.Where(item => codeList.Contains(item.code)).OrderBy(item => codeList.IndexOf(item.code)).ToList();
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// 查询基金基础信息
        /// </summary>
        public static async Task<List<FundBasicInfoData>> QueryFundBasicData(List<string> codeList)
        {
            try
            {
                string queryUrl = "https://api.doctorxiong.club/v1/fund";
                RestClient client = new RestClient(queryUrl);
                client.Timeout = 3000;
                RestRequest request = new RestRequest(Method.GET);
                request.AddQueryParameter("code", string.Join(",", codeList));
                IRestResponse response = await client.ExecuteAsync(request);
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
                string content = response.Content;
                FundBasicInfoResponse responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<FundBasicInfoResponse>(content);
                return responseData.data;
            }
            catch (Exception ex)
            {
            }
            return null;
        }
        /// <summary>
        /// 刷新托管数据
        /// </summary>
        /// <param name="manageData">托管数据</param>
        public static async Task RefreshData(FinancialManagementData manageData, Dispatcher dispatcher)
        {
            #region 更新指数信息
            manageData.AllStockDataList = await QueryStockData(manageData.Setting.StockCodes);
            // 如果之前没有数据，就新增三条数据
            if (manageData.SelectedStockDataList.Count == 0 && manageData.AllStockDataList?.Count > 0)
            {
                dispatcher.Invoke(() =>
                {
                    manageData.SelectedStockDataList.Add(manageData.AllStockDataList[++manageData.SelectedStockStartIndex]);
                    manageData.SelectedStockDataList.Add(manageData.AllStockDataList[++manageData.SelectedStockStartIndex]);
                    manageData.SelectedStockDataList.Add(manageData.AllStockDataList[++manageData.SelectedStockStartIndex]);
                });
            }
            else // 否则替换最新查询出的数据
            {
                for (int i = 0; i < manageData.SelectedStockDataList.Count; i++)
                {
                    BoardQuotationData item = manageData.SelectedStockDataList[i];
                    int index = manageData.AllStockDataList.FindIndex(a => a.code == item.code);
                    if (index >= 0)
                    {
                        dispatcher.Invoke(() => manageData.SelectedStockDataList[i] = manageData.AllStockDataList[index]);
                    }
                }
            }
            #endregion
            #region 更新热门板块
            List<BoardQuotationData> hotBoardDataList = new List<BoardQuotationData>();
            List<BoardQuotationData> boardDataList = await QueryBoardData();
            if (boardDataList != null)
            {
                // 先把前二和后二加入列表
                hotBoardDataList.AddRange(boardDataList.Take(2));
                hotBoardDataList.AddRange(boardDataList.Skip(boardDataList.Count - 2));
                // 判断第一个和第二个是否是跌的，不是的话，取倒四，倒三补上
                if (hotBoardDataList[0].changePercent > 0)
                {
                    hotBoardDataList[0] = boardDataList[boardDataList.Count - 5];
                }
                if (hotBoardDataList[1].changePercent > 0)
                {
                    hotBoardDataList[1] = boardDataList[boardDataList.Count - 4];
                }
                // 判断第三第四个是否是涨的，不是的话，取第三，第四补上
                if (hotBoardDataList[2].changePercent < 0)
                {
                    hotBoardDataList[2] = boardDataList[3];
                }
                if (hotBoardDataList[3].changePercent < 0)
                {
                    hotBoardDataList[3] = boardDataList[2];
                }
            }
            if (hotBoardDataList.Count > 3)
            {
                manageData.FirstHotBoardData = hotBoardDataList[0];
                manageData.SecondHotBoardData = hotBoardDataList[1];
                manageData.ThirdHotBoardData = hotBoardDataList[2];
                manageData.FourthHotBoardData = hotBoardDataList[3];
            }
            #endregion
            #region 更新自选基金信息
            List<FundBasicInfoData> fundDataList = await QueryFundBasicData(manageData.Setting.FundCodes);
            if (fundDataList != null && fundDataList.Count > 0)
            {
                DateTime fundDate = ConvertToDateTime(fundDataList.First().expectWorthDate);
                manageData.TransactionDate = fundDate.ToString("MM-dd");
                if (fundDate.Date < DateTime.Now.Date)
                    manageData.StockMarketStatus = "未开市";
                else if (DateTime.Now.TimeOfDay > new TimeSpan(15, 0, 0))
                    manageData.StockMarketStatus = "已闭市";
                else
                    manageData.StockMarketStatus = string.Empty;
                foreach (FundBasicInfoData fundData in fundDataList)
                {
                    int i = 0;
                    for (; i < manageData.FundDataList.Count; i++)
                    {
                        FundBasicInfoData item = manageData.FundDataList[i];
                        if (item.code == fundData.code)
                        {
                            dispatcher.Invoke(() => manageData.FundDataList[i] = fundData);
                            break;
                        }
                    }
                    if (i == manageData.FundDataList.Count)
                    {
                        dispatcher.Invoke(() => manageData.FundDataList.Add(fundData));
                    }
                }
            }
            #endregion
        }
        /// <summary>
        /// 从时间类型字符串转成时间
        /// </summary>
        private static DateTime ConvertToDateTime(object dateObj)
        {
            string dateStr = dateObj.ToString();
            System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex("\\D");
            dateStr = pattern.Replace(dateStr, "");
            if (dateStr.Length == 8)
            {
                return DateTime.ParseExact(dateStr, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            }
            else if (dateStr.Length == 10)
            {
                return new DateTime(1970, 1, 1).AddSeconds(int.Parse(dateStr));
            }
            else if (dateStr.Length == 11)
            {
                return new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(dateStr));
            }
            else if (dateStr.Length == 14)
            {
                return DateTime.ParseExact(dateStr, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            }
            throw new FormatException();
        }
    }

    public class FinancialManagementData : INotifyPropertyChanged
    {
        /// <summary>
        /// 程序配置信息
        /// </summary>
        public ApplicationSetting Setting { get; set; }
        /// <summary>
        /// 第一热门板块数据
        /// </summary>
        public BoardQuotationData FirstHotBoardData { get; set; } = new BoardQuotationData();
        /// <summary>
        /// 第二热门板块数据
        /// </summary>
        public BoardQuotationData SecondHotBoardData { get; set; } = new BoardQuotationData();
        /// <summary>
        /// 第三热门板块数据
        /// </summary>
        public BoardQuotationData ThirdHotBoardData { get; set; } = new BoardQuotationData();
        /// <summary>
        /// 第四热门板块数据
        /// </summary>
        public BoardQuotationData FourthHotBoardData { get; set; } = new BoardQuotationData();
        /// <summary>
        /// 指数数据
        /// </summary>
        public List<BoardQuotationData> AllStockDataList { get; set; } = new List<BoardQuotationData>();
        /// <summary>
        /// 被选的指数开始索引
        /// </summary>
        public int SelectedStockStartIndex = -1;
        /// <summary>
        /// 交易日
        /// </summary>
        public string TransactionDate { get; set; } = DateTime.Now.ToString("MM-dd");
        /// <summary>
        /// 显示未开市标识
        /// </summary>
        public string StockMarketStatus { get; set; }
        /// <summary>
        /// 被选择的指数数据
        /// </summary>
        public BindingList<BoardQuotationData> SelectedStockDataList { get; set; } = new BindingList<BoardQuotationData>();
        /// <summary>
        /// 自选基金数据
        /// </summary>
        public BindingList<FundBasicInfoData> FundDataList { get; set; } = new BindingList<FundBasicInfoData>();
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// 自定义数值类型序列化转换器(默认保留3位)
    /// </summary>
    public class JsonCustomNumberConvert<T> : CustomCreationConverter<T>
    {
        /// <summary>
        /// 重载创建方法
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override T Create(Type objectType) => default;
        /// <summary>
        /// 重载可读
        /// </summary>
        public override bool CanRead { get => true; }
        /// <summary>
        /// 解析数字
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                if (reader.Value == null) return default(T);
                var strValue = reader.Value.ToString();
                strValue = new Regex("[^0-9\\.\\-]").Replace(strValue, "");
                if (string.IsNullOrEmpty(strValue)) return default(T);
                return TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(strValue);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}

using Snet.Core.subscription;
using Snet.Model.attribute;
using Snet.Model.data;
using Snet.Utility;
using System.ComponentModel;

namespace Snet.PerformanceTesting
{
    /// <summary>
    /// 性能测试数据
    /// </summary>
    public class PerformanceTestingData
    {
        /// <summary>
        /// 基础数据
        /// </summary>
        public class Basics : SubscribeData.SCData
        {
            /// <summary>
            /// 唯一标识符
            /// </summary>
            [Category("基础数据")]
            [Description("唯一标识符")]
            public string? SN { get; set; } = Guid.NewGuid().ToUpperNString();

            /// <summary>
            /// 地址
            /// </summary>
            [Description("地址")]
            [Display(true, true, false, ParamModel.dataCate.text)]
            public string? Uri { get; set; } = "tcp://127.0.0.1:8866";

            /// <summary>
            /// 主题
            /// </summary>
            [Description("主题")]
            [Display(true, true, false, ParamModel.dataCate.text)]
            public string Topic { get; set; } = "PerformanceTesting";

            /// <summary>
            /// 超时时间
            /// </summary>
            [Description("超时时间")]
            [Unit("ms")]
            [Display(true, true, true, ParamModel.dataCate.unmber)]
            public int TimeOut { get; set; } = 1000;
        }


        /// <summary>
        /// 数据包
        /// </summary>
        public class Pack
        {
            /// <summary>
            /// 点位地址
            /// </summary>
            public string AddressName { get; set; }
            /// <summary>
            /// 点位的值
            /// </summary>
            public object Value { get; set; }
        }
    }
}

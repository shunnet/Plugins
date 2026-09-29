using NetMQ;
using NetMQ.Sockets;
using Snet.Core.@abstract;
using Snet.Core.handler;
using Snet.Core.subscription;
using Snet.Core.virtualAddress;
using Snet.Log;
using Snet.Model.data;
using Snet.Model.@enum;
using Snet.Model.@interface;
using Snet.PerformanceTesting;
using Snet.Utility;
using System.Collections.Concurrent;
using System.Threading.Channels;
using static Snet.PerformanceTesting.PerformanceTestingData;
namespace Snet.PerformanceTesting
{
    /// <summary>
    /// 性能测试操作
    /// </summary>
    public class PerformanceTestingOperate : DaqAbstract<PerformanceTestingOperate, Basics>, IDaq
    {
        /// <summary>
        /// 无惨构造函数
        /// </summary>
        public PerformanceTestingOperate() : base() { }

        /// <summary>
        /// 有参构造函数
        /// </summary>
        /// <param name="basics">基础数据</param>
        public PerformanceTestingOperate(Basics basics) : base(basics) { }
        /// <inheritdoc/>
        protected override string CN => "性能测试";
        /// <inheritdoc/>
        protected override string CD => "“性能测试”只支持数据订阅模式";

        /// <summary>
        /// netMQ 通信类
        /// </summary>
        private SubscriberSocket? subscriber;
        /// <summary>
        /// 实现订阅功能
        /// </summary>
        private SubscribeOperate? subscribeOperate = null;
        /// <summary>
        /// 虚拟地址
        /// </summary>
        private VirtualAddressManage VAM = new VirtualAddressManage();

        /// <summary>
        /// 数据队列
        /// </summary>
        private Channel<AddressValue> DataQueue;

        /// <summary>
        /// 任务取消
        /// </summary>
        private CancellationTokenSource? g_token;

        /// <summary>
        /// 容器
        /// </summary>
        ConcurrentDictionary<string, AddressDetails> ioc = new ConcurrentDictionary<string, AddressDetails>();
        /// <summary>
        /// 数据
        /// </summary>
        ConcurrentDictionary<string, AddressValue> values = new ConcurrentDictionary<string, AddressValue>();

        /// <summary>
        /// 通道设置
        /// </summary>
        private BoundedChannelOptions channelOptions => p_Channel ??= new BoundedChannelOptions(ushort.MaxValue)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        };
        private BoundedChannelOptions? p_Channel;

        /// <summary>
        /// 任务处理
        /// </summary>
        /// <param name="token">任务令牌</param>
        /// <returns>任务</returns>
        private async Task TaskHandleAsync(CancellationToken token)
        {
            try
            {
                //如果没数据,可以异步等待
                while (await DataQueue.Reader.WaitToReadAsync(token))
                {
                    //出列
                    while (DataQueue.Reader.TryRead(out var queueData))
                    {
                        if (queueData != null && !token.IsCancellationRequested)
                        {
                            try
                            {
                                //添加或更新
                                values.AddOrUpdate(queueData.AddressName, queueData, (k, v) => queueData);

                            }
                            catch (Exception ex)
                            {
                                await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[ 订阅通知 ]异常:{ex.Message}"));
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[ 订阅通知 ]已停止"));
            }
            catch (OperationCanceledException)
            {
                await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[ 订阅通知 ]已停止"));
            }
            catch (Exception ex)
            {
                await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"[ 订阅通知 ]异常:{ex.Message}"));
            }

        }

        /// <summary>
        /// 订阅消息处理
        /// </summary>
        /// <param name="token">生命周期</param>
        /// <returns></returns>
        private async Task SubMessageHandleAsync(CancellationToken token)
        {
            await Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        SubscriberSocket? subscriberSocket = subscriber;
                        if (subscriberSocket != null)
                        {
                            if (subscriberSocket.TryReceiveFrameString(TimeSpan.FromMilliseconds(basics.TimeOut), out string? Topic, out bool moreFrames))
                            {
                                if (Topic.Equals(basics.Topic))
                                {
                                    //得到内容Json
                                    string con = subscriberSocket.ReceiveFrameString();

                                    //得到数据包
                                    PerformanceTestingData.Pack[]? packs = con.ToJsonEntity<PerformanceTestingData.Pack[]>();

                                    //判断包是不是空的
                                    if (packs == null || packs.Length <= 0)
                                    {
                                        continue;
                                    }

                                    //遍历包
                                    foreach (var item in packs)
                                    {
                                        ioc.TryGetValue(item.AddressName, out AddressDetails? addressDetails);
                                        if (addressDetails == null)
                                        {
                                            await LogHelper.ErrorAsync($"未找到 {item.AddressName} 的缓存数据 ( 底层驱动输出 )", token: token);
                                        }
                                        else
                                        {
                                            AddressValue? addressValue = AddressHandler.ExecuteDispose(addressDetails, item.Value, "成功");
                                            //往队列里面添加
                                            await DataQueue.Writer.WriteAsync(addressValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await OnInfoEventHandlerAsync(this, new EventInfoResult(false, $"消费异常:{ex.Message}"));
                    }
                }
            }, token);
        }


        /// <inheritdoc/>
        public override async Task<OperateResult> OnAsync(CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if ((await GetStatusAsync(token)).GetDetails(out string? message))
                {
                    return await EndOperateAsync(false, message, token: token);
                }
                if (subscriber == null)
                {
                    subscriber = new SubscriberSocket();
                }
                if (DataQueue == null)
                {
                    DataQueue = Channel.CreateBounded<AddressValue>(channelOptions);
                    g_token = new CancellationTokenSource();
                    _ = SubMessageHandleAsync(g_token.Token).ConfigureAwait(false);
                    _ = TaskHandleAsync(g_token.Token).ConfigureAwait(false);
                }
                subscriber.Connect(basics.Uri);
                subscriber?.Subscribe(basics.Topic);
                return await EndOperateAsync(true, token: token);
            }
            catch (Exception ex)
            {
                await OffAsync(true, token: token);
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> OffAsync(bool hardClose = false, CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if (!hardClose)
                {
                    if (!(await GetStatusAsync(token)).GetDetails(out string? message))
                    {
                        return await EndOperateAsync(false, message, token: token);
                    }
                }

                await g_token?.CancelAsync();
                g_token?.Dispose();
                g_token = null;

                if (subscribeOperate != null)
                {
                    OperateResult operateResult = await subscribeOperate.OffAsync(hardClose, token);
                    if (!operateResult.Status)
                    {
                        return await EndOperateAsync(false, operateResult.Message, token: token);
                    }

                    subscribeOperate = null;
                }

                VAM?.DisposeAsync();

                //清空队列
                if (DataQueue != null)
                {
                    // 替换 Channel 实例来清空数据
                    while (DataQueue.Reader.TryRead(out _)) { }

                    DataQueue = null;
                }
                subscriber?.Close();
                subscriber?.Dispose();
                subscriber = null;
                //清空
                ioc?.Clear();
                values?.Clear();
                return await EndOperateAsync(true, token: token);
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> ReadAsync(Address address, CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                //节点数据
                ConcurrentDictionary<string, AddressValue> param = new ConcurrentDictionary<string, AddressValue>();
                //循环添加项集合
                foreach (var item in address.AddressArray)
                {
                    if (!item.IsEnable)
                        continue;
                    //详细信息
                    string Msg = await "失败".GetLanguageValueAsync(token: token);
                    //是不是虚拟地址
                    bool IsVA = false;
                    //初始化虚拟地址
                    VAM.InitVirtualAddress(item, out IsVA);
                    //值
                    object? Value = null;
                    //地址
                    string addressName = item.AddressName;
                    if (IsVA)
                    {
                        Value = VAM.Read(item);
                        //数据处理
                        AddressValue? addressValue = Snet.Core.handler.AddressHandler.ExecuteDispose(item, Value, Msg);
                        //数据添加
                        param.AddOrUpdate(addressName, addressValue, (k, v) => addressValue);
                    }
                    else
                    {
                        //更新容器
                        ioc.AddOrUpdate(addressName, item, (k, v) => item);

                        if (values.TryGetValue(addressName, out AddressValue? value) && value != null)
                        {
                            param.AddOrUpdate(addressName, value, (k, v) => value);
                        }
                    }
                }

                if (param.Count > 0)
                {
                    //返回读取的数据
                    return await EndOperateAsync(true, resultData: param, token: token);
                }
                else
                {
                    return await EndOperateAsync(false, await "读取失败".GetLanguageValueAsync(token: token), token: token);
                }
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> WriteAsync(ConcurrentDictionary<string, (object value, EncodingType? encodingType)> values, CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                return await EndOperateAsync(false, "不支持此功能", token: token);
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> SubscribeAsync(Address address, CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if (!(await GetStatusAsync(token)).GetDetails(out string? message))
                {
                    return await EndOperateAsync(false, message, token: token);
                }

                if (!address.CheckAddress())
                {
                    return await EndOperateAsync(false, await "存在无效点位数据，操作失败".GetLanguageValueAsync(token: token), token: token);
                }

                if (subscribeOperate == null)
                {
                    subscribeOperate = await SubscribeOperate.InstanceAsync(new SubscribeData.Basics() { Address = address, ChangeOut = basics.ChangeOut, FunctionAsync = ReadAsync, AllOut = basics.AllOut, HandleInterval = basics.HandleInterval, SN = basics.SN, TaskNumber = basics.TaskNumber });
                    subscribeOperate.OnDataEvent += OnDataEventHandler;
                    subscribeOperate.OnInfoEvent += OnInfoEventHandler;
                    return await EndOperateAsync(await subscribeOperate.OnAsync(token), token: token);
                }
                else
                {
                    return await EndOperateAsync(await subscribeOperate.SubscribeAsync(address, token), token: token);
                }
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> UnSubscribeAsync(Address address, CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if (!(await GetStatusAsync(token)).GetDetails(out string? message))
                {
                    return await EndOperateAsync(false, message, token: token);
                }

                if (subscribeOperate != null)
                {
                    return await EndOperateAsync(await subscribeOperate.UnSubscribeAsync(address, token), token: token);
                }
                else
                {
                    return await EndOperateAsync(false, await "当前尚未订阅".GetLanguageValueAsync(token: token), token: token);
                }
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> GetStatusAsync(CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if (subscriber == null)
                {
                    return await EndOperateAsync(false, await "未连接".GetLanguageValueAsync(token: token), consoleOutput: false, token: token);
                }
                return await EndOperateAsync(true, await "已连接".GetLanguageValueAsync(token: token), consoleOutput: false, token: token);
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }
        /// <inheritdoc/>
        public override async Task<OperateResult> GetBaseObjectAsync(CancellationToken token = default)
        {
            await BegOperateAsync(token);
            try
            {
                if (!(await GetStatusAsync(token)).GetDetails(out string? message))
                {
                    return await EndOperateAsync(false, message, token: token);
                }
                return await EndOperateAsync(true, resultData: subscriber, token: token);
            }
            catch (Exception ex)
            {
                return await EndOperateAsync(false, ex.Message, exception: ex, token: token);
            }
        }

    }
}

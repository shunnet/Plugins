using CommunityToolkit.Mvvm.Input;
using NetMQ;
using NetMQ.Sockets;
using Snet.PerformanceTesting;
using Snet.Utility;
using Snet.Windows.Controls.handler;
using Snet.Windows.Core.mvvm;
using System.Windows.Controls;

namespace Snet.Windows.PerformanceTesting
{
    public class MainWindowViewModel : BindNotify
    {

        public MainWindowViewModel()
        {
            // 界面消息处理
            uiMessage.OnInfoEventAsync += async (object? sender, Model.data.EventInfoResult e) => Info = e.Message;
            uiMessage.StartAsync().ConfigureAwait(false);
        }

        UiMessageHandler uiMessage = UiMessageHandler.Instance("Info");

        /// <summary>
        /// 数据类型
        /// </summary>
        public int DataType
        {
            get => dataType;
            set => SetProperty(ref dataType, value);
        }
        private int dataType = 20;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }
        private string address = "tcp://127.0.0.1:8866";

        /// <summary>
        /// 主题
        /// </summary>
        public string Topic
        {
            get => topic;
            set => SetProperty(ref topic, value);
        }
        private string topic = "PerformanceTesting";

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix
        {
            get => prefix;
            set => SetProperty(ref prefix, value);
        }
        private string prefix = "PT";

        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        private int count = 10000;


        /// <summary>
        /// 次数
        /// </summary>
        public int Frequency
        {
            get => frequency;
            set => SetProperty(ref frequency, value);
        }
        private int frequency = 100;


        /// <summary>
        /// 信息
        /// </summary>
        public string Info
        {
            get => GetProperty(() => Info);
            set => SetProperty(() => Info, value);
        }

        /// <summary>
        /// 信息框事件
        /// </summary>
        public IAsyncRelayCommand InfoTextChanged { get => new AsyncRelayCommand<TextChangedEventArgs>(InfoTextChangedAsync); }
        /// <summary>
        /// 信息框事件
        /// 让滚动条一直处在最下方
        /// </summary>
        public Task InfoTextChangedAsync(TextChangedEventArgs? e)
        {
            TextBox textBox = e.Source.GetSource<TextBox>();
            textBox.SelectionStart = textBox.Text.Length;
            textBox.SelectionLength = 0;
            textBox.ScrollToEnd();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 数据清空
        /// </summary>
        public IAsyncRelayCommand Clear => new AsyncRelayCommand(ClearAsync);
        public async Task ClearAsync()
        {
            await uiMessage.ClearAsync();
        }
        Random random = new Random();
        async Task send(CancellationToken token, int frequency = 1)
        {
            try
            {
                await Task.Run(async () =>
                {
                    int milliseconds = 0;
                    for (int f = 1; f <= frequency; f++)
                    {
                        if (frequency == 1)
                        {
                            await uiMessage.ShowAsync("正在发送中...");
                        }
                        else
                        {
                            await uiMessage.ShowAsync($"正在发送第 {f} 次");
                        }
                        List<PerformanceTestingData.Pack> packs = new List<PerformanceTestingData.Pack>();
                        for (int i = 0; i < Count; i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                await uiMessage.ShowAsync("已停止发送");
                                return;
                            }
                            Model.@enum.DataType dataType = (Model.@enum.DataType)DataType + 1;
                            switch (dataType)
                            {
                                case Model.@enum.DataType.None:
                                    break;
                                case Model.@enum.DataType.Bool:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = random.NextDouble() > 0.5 });
                                    break;
                                case Model.@enum.DataType.Double:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = random.NextDouble() });
                                    break;
                                case Model.@enum.DataType.Float:
                                case Model.@enum.DataType.Single:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (float)random.NextDouble() });
                                    break;
                                case Model.@enum.DataType.Short:
                                case Model.@enum.DataType.Int16:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (short)random.Next(short.MinValue, short.MaxValue) });
                                    break;
                                case Model.@enum.DataType.Ushort:
                                case Model.@enum.DataType.UInt16:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (ushort)random.Next(ushort.MinValue, ushort.MaxValue) });
                                    break;
                                case Model.@enum.DataType.Int:
                                case Model.@enum.DataType.Int32:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = random.Next() });
                                    break;
                                case Model.@enum.DataType.Uint:
                                case Model.@enum.DataType.UInt32:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (uint)random.Next() * 2 });
                                    break;
                                case Model.@enum.DataType.Long:
                                case Model.@enum.DataType.Int64:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (long)random.Next() << 32 | (long)random.Next() & 0xFFFFFFFF });
                                    break;
                                case Model.@enum.DataType.Ulong:
                                case Model.@enum.DataType.UInt64:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = (ulong)random.Next() << 32 | (ulong)random.Next() & 0xFFFFFFFF });
                                    break;
                                case Model.@enum.DataType.String:
                                case Model.@enum.DataType.Char:
                                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                                    string stringValue = new string(Enumerable.Repeat(chars, random.Next(1, chars.Length)).Select(s => s[random.Next(s.Length)]).ToArray());
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = stringValue });
                                    break;
                                case Model.@enum.DataType.ByteArray:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new byte[] { (byte)random.Next(0, 256) } });
                                    break;
                                case Model.@enum.DataType.BoolArray:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new bool[] { random.NextDouble() > 0.5 } });
                                    break;
                                case Model.@enum.DataType.DoubleArray:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new double[] { random.NextDouble() } });
                                    break;
                                case Model.@enum.DataType.FloatArray:
                                case Model.@enum.DataType.SingleArray:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new float[] { (float)random.NextDouble() } });
                                    break;
                                case Model.@enum.DataType.ShortArray:
                                case Model.@enum.DataType.Int16Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new short[] { (short)random.Next(short.MinValue, short.MaxValue) } });
                                    break;
                                case Model.@enum.DataType.UshortArray:
                                case Model.@enum.DataType.UInt16Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new ushort[] { (ushort)random.Next(ushort.MinValue, ushort.MaxValue) } });
                                    break;
                                case Model.@enum.DataType.IntArray:
                                case Model.@enum.DataType.Int32Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new int[] { random.Next() } });
                                    break;
                                case Model.@enum.DataType.UintArray:
                                case Model.@enum.DataType.UInt32Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new uint[] { (uint)random.Next() * 2 } });
                                    break;
                                case Model.@enum.DataType.LongArray:
                                case Model.@enum.DataType.Int64Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new long[] { (long)random.Next() << 32 | (long)random.Next() & 0xFFFFFFFF } });
                                    break;
                                case Model.@enum.DataType.UlongArray:
                                case Model.@enum.DataType.UInt64Array:
                                    packs.Add(new PerformanceTestingData.Pack { AddressName = $"{Prefix}{i + 1}", Value = new ulong[] { (ulong)random.Next() << 32 | (ulong)random.Next() & 0xFFFFFFFF } });
                                    break;
                                default:
                                    break;
                            }
                        }
                        TimeHandler.Instance(Topic).StartRecord();
                        publisher.SendMoreFrame(Topic).SendFrame(packs.ToJson());
                        if (frequency == 1)
                        {
                            await uiMessage.ShowAsync($"成功发送 {Count} 个数据包，耗时：{TimeHandler.Instance(Topic).StopRecord().milliseconds}");
                        }
                        else
                        {
                            int ms = TimeHandler.Instance(Topic).StopRecord().milliseconds;
                            milliseconds += ms;
                            await uiMessage.ShowAsync($"成功发送第 {f} 次，{Count} 个数据包，耗时：{ms}");
                        }
                    }
                    if (frequency != 1)
                    {
                        await uiMessage.ShowAsync($"总发送 {frequency} 次，总耗时：{milliseconds}");
                    }
                }, token);
            }
            catch (TaskCanceledException)
            {
                await uiMessage.ShowAsync("任务取消");
            }
            catch (OperationCanceledException)
            {
                await uiMessage.ShowAsync("任务取消");
            }
            catch (Exception ex)
            {
                await uiMessage.ShowAsync($"任务异常：{ex.Message}");
            }
        }


        /// <summary>
        /// 发布端
        /// </summary>
        private PublisherSocket publisher;

        private CancellationTokenSource token_one;
        private CancellationTokenSource token_two;

        /// <summary>
        /// 启动
        /// </summary>
        public IAsyncRelayCommand Start => new AsyncRelayCommand(StartAsync);
        public async Task StartAsync()
        {
            if (publisher == null)
            {
                try
                {
                    publisher = new PublisherSocket();
                    publisher.Bind(Address);
                    await uiMessage.ShowAsync("启动成功");
                }
                catch (Exception ex)
                {
                    await uiMessage.ShowAsync($"启动失败，{ex.Message}");
                    publisher = null;
                }

            }
            else
            {
                await uiMessage.ShowAsync("启动失败，已启动");
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public IAsyncRelayCommand Stop => new AsyncRelayCommand(StopAsync);
        public async Task StopAsync()
        {
            if (publisher == null)
            {
                await uiMessage.ShowAsync("停止失败，未启动");
                return;
            }
            token_one?.Cancel();
            token_one = null;
            token_two?.Cancel();
            token_two = null;
            publisher?.Close();
            publisher?.Dispose();
            publisher = null;
            await uiMessage.ShowAsync("停止成功");
        }

        /// <summary>
        /// 发送
        /// </summary>
        public IAsyncRelayCommand Send => new AsyncRelayCommand(SendAsync);
        public async Task SendAsync()
        {
            if (publisher == null)
            {
                await uiMessage.ShowAsync("发送失败，未启动");
                return;
            }
            token_one ??= new CancellationTokenSource();
            await send(token_one.Token);
        }

        /// <summary>
        /// 停止发送
        /// </summary>
        public IAsyncRelayCommand StopSend => new AsyncRelayCommand(StopSendAsync);
        public async Task StopSendAsync()
        {
            if (publisher == null)
            {
                await uiMessage.ShowAsync("停止发送失败，未启动");
                return;
            }

            token_one?.Cancel();
            token_one = null;
        }

        /// <summary>
        /// 循环发送
        /// </summary>
        public IAsyncRelayCommand LoopSend => new AsyncRelayCommand(LoopSendAsync);
        public async Task LoopSendAsync()
        {
            if (publisher == null)
            {
                await uiMessage.ShowAsync("循环发送失败，未启动");
                return;
            }

            token_two ??= new CancellationTokenSource();
            await send(token_two.Token, Frequency);
        }
        /// <summary>
        /// 停止循环发送
        /// </summary>
        public IAsyncRelayCommand StopLoopSend => new AsyncRelayCommand(StopLoopSendAsync);
        public async Task StopLoopSendAsync()
        {
            if (publisher == null)
            {
                await uiMessage.ShowAsync("停止循环发送失败，未启动");
                return;
            }

            token_two?.Cancel();
            token_two = null;
        }
    }
}

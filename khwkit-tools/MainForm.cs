using CrazySharp.Base;
using CrazySharp.Std;
using khwkit.Beans;
using khwkit.Core;
using khwkit.Enums;
using khwkit_tools.Beans;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;
using NLog;
using NLog.Config;
using NLog.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;
using khwkit.Beans.TicketPrinter;
using khwkit.Beans.QRCodeReader;
using khwkit.Beans.RoomCard;
using khwkit.Beans.IdCard;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using Newtonsoft.Json;

namespace khwkit_tools
{
    public partial class MainForm : BaseForm
    {
        private const string small_rabbit_baseurl = "https://small-rabbit.pujie88.com/api/v1";
        private static Logger logger = LogManager.GetCurrentClassLogger();//日志类
        private static Logger formLogger = null;
        private const string PRINTER_CONTENT = "[{\"type\":1,\"title\":null,\"content\":\"智慧酒店\"},{\"type\":2,\"title\":null,\"content\":\"---- 入住单 ----\"},{\"type\":3,\"title\":\"房\\t\\t号\",\"content\":\"4002\"},{\"type\":3,\"title\":\"入住人\",\"content\":\"**\"},{\"type\":3,\"title\":\"同 住 码\",\"content\":\"暂无\"},{\"type\":3,\"title\":\"入住时间\",\"content\":\"2020-03-14 00:00:00\"},{\"type\":3,\"title\":\"离店时间\",\"content\":\"2020-03-18 14:00:00\"},{\"type\":3,\"title\":\"早\\t\\t餐\",\"content\":\"暂无\"},{\"type\":3,\"title\":\"备\\t\\t注\",\"content\":\"请于离店日期2020-03-14 12:00退房,否则将收取额外费用\"},{\"type\":6,\"title\":null,\"content\":null},{\"type\":3,\"title\":\"房费\",\"content\":\"0\"},{\"type\":3,\"title\":\"押金\",\"content\":\"0\"},{\"type\":3,\"title\":\"付款\",\"content\":\"0 \"},{\"type\":6,\"title\":null,\"content\":null},{\"type\":3,\"title\":\"WIFI名称\",\"content\":\"HOTEL8888\"},{\"type\":3,\"title\":\"密\\t\\t码\",\"content\":\"88888888\"},{\"type\":3,\"title\":\"前台电话\",\"content\":\"400-1818-802\"},{\"type\":3,\"title\":\"打印时间\",\"content\":\"2020-03-17 16:57:24\"},{\"type\":6,\"title\":null,\"content\":null},{\"type\":5,\"title\":null,\"content\":\"谢谢光临!期待您再次使用智能终端入住!\"}]";
        public MainForm()
        {
            InitializeComponent();
        }
        protected override void OnCreate()
        {
            this.Text = $"自助机外设服务配置工具-V{ Application.ProductVersion}";
            Defaults.UseDefaultJsonSetting();
            //初始化formLogger
            formLogger = initLogger(this, this.rtbInfoOut);
            formLogger.Info("初始化成功");
            txHwkitIp.Text = "127.0.0.1";
            // FetchSummaryAsync($"http://{txHwkitIp.Text}:5000/api");
            txLoalIp.Text = Utils.LocalIp();
            //#if DEBUG
            //            txHwkitIp.Text = "192.168.20.54";
            //#endif
            if (PRINTER_CONTENT.TryDeserializeJsonStr<List<TicketStyledItem>>( out List<TicketStyledItem> content))
            {
                tpPrinterRtxtPrintContent.Text = content.ToJsonString(Newtonsoft.Json.Formatting.Indented);
            }
       
        }
        private void txTvId_TextChanged(object sender, EventArgs e)
        {
            string tmp = txTvId.Text?.Replace(" ", "");
            txTvId.Text = tmp;
        }
        private void btnFetchSummary_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl,out _))
            {
                return;
            }
            FetchSummaryAsync(baseurl);
        }

        private void btnSetupTv_Click(object sender, EventArgs e)
        {
            string tvSetUpFile = "TeamViewer_Setup.exe";
            if (!File.Exists(tvSetUpFile))
            {
                Utils.Error("程序根目录未找到 'TeamViewer_Setup.exe' ,请手动安装");
                return;
            }
            //install service
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = tvSetUpFile;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            startInfo.Verb = "RunAs";
            process.Start();
            // process.WaitForExit();
        }

        private async void btnReportTvId_Click(object sender, EventArgs e)
        {
            string sn = txSN.Text?.Trim() ?? "";
            if (sn.IsEmpty())
            {
                Utils.Error("请先获取SN");
                txTvId.Focus();
                return;
            }
            if (sn.Length != 12)
            {
                Utils.Error("SN格式不正确");
                txTvId.Focus();
                return;
            }
            string tvId = txTvId.Text?.Trim() ?? "";
            if (tvId.IsEmpty())
            {
                Utils.Error("请填写Teamviewer Id");
                txTvId.Focus();
                return;
            }
            tvId = tvId.Replace(" ", "");
            if (tvId.Length != 9 && tvId.Length != 10)
            {
                Utils.Error("Teamviewer Id 格式不正确");
                txTvId.Focus();
                return;
            }
            var pb = this.ShowProgress("【上传远程信息】......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{small_rabbit_baseurl}/ssm/remotectl/{sn}",
                new UpdateSsmRemoteCtlInfoReq()
                {
                    RemoteType = "Teamviewer",
                    Detail = new JsonObject() { { "tvId", tvId } }
                });
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            Utils.Info("上传成功");
        }
        private bool OutRespLog<T>(string tag, BasicResp<T> resp, bool showDialog = false)
        {
            if (resp.Ok)
            {
                formLogger.Info($"{tag} 结果 : {resp.ToJsonString()}");
                return true;
            }
            formLogger.Error($"{tag} 失败 : {resp.ToJsonString()}");
            if (showDialog)
            {
                Utils.Error($"{tag} 失败 : {resp.Error}");
            }
            return false;
        }
        private void tabCtl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtl.SelectedTab == tpRoomCard)
            {
                var today = DateTime.Today;
                var tomorow = today.AddDays(1).AddHours(14);
                tpRoomCardTimeStartToW.Value = today;
                tpRoomCardTimeEndToW.Value = tomorow;
            }
            if (tabCtl.SelectedTab == tpPSB)
            {
                var today = DateTime.Today;
                var tomorow = today.AddDays(1).AddHours(14);
                tpPSBTimeInTime.Value = today;
                tpPSBTimeOutTime.Value = tomorow;
                tpPSBTimeStayOut.Value = tomorow;
            }
        }
        private Logger initLogger(Form targetForm, RichTextBox targetRichText)
        {
            // Step 1. Create configuration object 
            LoggingConfiguration logConfig = LogManager.Configuration;  // new LoggingConfiguration();
                                                                        // LoggingConfiguration logConfig = new LoggingConfiguration();
                                                                        // Step 2. Create targets and add them to the configuration 
            var minLoggerLevel = LogLevel.Info;
#if DEBUG
            minLoggerLevel = LogLevel.Trace;
#endif
            if (logConfig == null)
            {
                logConfig = new LoggingConfiguration();
            }
            RichTextBoxTarget rtbTarget = new RichTextBoxTarget();
            logConfig.AddTarget("richTextBox", rtbTarget);
            rtbTarget.ControlName = targetRichText.Name;
            rtbTarget.FormName = targetForm.Name;
            rtbTarget.Layout = "${longdate} | ${message}";
            rtbTarget.AutoScroll = true;
            rtbTarget.UseDefaultRowColoringRules = true;
            rtbTarget.RowColoringRules.Insert(0,
                new RichTextBoxRowColoringRule("level== LogLevel.Error", "RED", "ControlLight"));
            // Step 4. Define rules
            LoggingRule rule = new LoggingRule("FormLogger", minLoggerLevel, LogLevel.Fatal, rtbTarget);
            logConfig.LoggingRules.Add(rule);
            // Step 5. Activate the configuration
            LogManager.Configuration = logConfig;
            Logger logger = LogManager.GetLogger("FormLogger");
            return logger;
        }

        private bool TryGetHwKitBaseUrl(out string httpBaseUrl,out string wsBaseUrl)
        {
            httpBaseUrl = "";
            wsBaseUrl = "";
            var ip = txHwkitIp.Text?.Trim() ?? "";
            var portStr = txHwkitPort.Text?.Trim() ?? "";
            if (ip.IsEmpty())
            {
                Utils.Error("请填写外设服务IP地址");
                txHwkitIp.Focus();
                return false;
            }
            if (!IPAddress.TryParse(ip, out _))
            {
                Utils.Error("外设服务IP地址不是正确的IP地址格式");
                txHwkitIp.Focus();
                return false;
            }
            if (portStr.IsEmpty())
            {
                Utils.Error("请填写外设服务端口");
                txHwkitPort.Focus();
                return false;
            }
            if(!int.TryParse(portStr,out int port)|| port <=0 || port>65535)
            {
                Utils.Error("外设服务端口格式不正确(1-65535)");
                txHwkitPort.Focus();
                return false;
            }

            httpBaseUrl = $"http://{ip}:{port}/api";
            wsBaseUrl = $"ws://{ip}:{port+1}/api";
            return true;
        }
        private async void FetchSummaryAsync(string baseUrl)
        {
            var pb = this.ShowProgress("正在获取信息 ......");
            formLogger.Info("【获取系统信息】...");
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitSystemSummary>>($"{baseUrl}/system/summary");
            pb.Done();
            if (!OutRespLog("【获取系统信息】", resp, true))
            {
                return;
            }
            txSN.Text = resp.Data.SN;
            txSoftVersion.Text = resp.Data.SwVersion;
            txHwVersion.Text = $"{resp.Data.HwVersion}({resp.Data.HwVersionDesc})";
        }
        public class ItemEx
        {
            public object UserData { get; set; }
            public object Tag { get; set; }
            public string Text { get; set; }
            public ItemEx(string text, object tag, object userData = null)
            {
                this.Text = text;
                this.Tag = tag;
                this.UserData = userData;
            }
            public override string ToString()
            {
                return this.Text;
            }
        }
        private async void ServiceStateCheck(KitServices service, Label stateLb)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info($"【状态检测-{service.FriendlyName()}】......");
            // var pb = this.ShowProgress($"正在检测'{service.FriendlyName()}'状态信息 ......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitHeathState>>($"{baseUrl}/{service.ApiPathStr()}/check");
            if (!OutRespLog($"【状态检测-{service.FriendlyName()}】", resp))
            {
                return;
            }
            // pb.Done();
            stateLb.ForeColor = Color.Black;
            stateLb.Text = $"{resp.Data.State}({resp.Data.StateDesc})";
            if (resp.Data.State == "WARN")
            {
                stateLb.ForeColor = Color.Orange;
            }
            else if (resp.Data.State == "ERROR")
            {
                stateLb.ForeColor = Color.Red;
            }
        }

        private void LoadBrandToCombox(KitServices service, List<KitServiceProviderItem> providerItems, KitServiceProviderItem currentProvider, ComboBox brand)
        {
            brand.Items.Clear();
            Dictionary<string, List<KitServiceProviderItem>> providerMap = new Dictionary<string, List<KitServiceProviderItem>>();
            foreach (var provider in providerItems)
            {
                if (!providerMap.ContainsKey(provider.GroupName))
                {
                    providerMap[provider.GroupName] = new List<KitServiceProviderItem>();
                }
                providerMap[provider.GroupName].Add(provider);
            }
            //品牌
            ItemEx selectedBrand = null;
            foreach (var kv in providerMap)
            {
                var item = new ItemEx(kv.Key, kv.Value, currentProvider);
                if (kv.Value.Any(it => it.ProviderId == currentProvider?.ProviderId))
                {
                    selectedBrand = item;
                }
                brand.Items.Add(item);
            }
            if (selectedBrand != null)
            {
                brand.SelectedItem = selectedBrand;
            }
            else if (brand.Items.Count > 0)
            {
                brand.SelectedIndex = 0;
            }
        }
        private void LoadModelToCombox(KitServices service, List<KitServiceProviderItem> providerItems, KitServiceProviderItem currentProvider, ComboBox model)
        {
            model.Items.Clear();
            ItemEx selectedModel = null;
            foreach (var provider in providerItems)
            {
                var item = new ItemEx(provider.Name, provider);
                if (provider.ProviderId == currentProvider?.ProviderId)
                {
                    selectedModel = item;
                }
                model.Items.Add(item);
            }
            if (selectedModel != null)
            {
                model.SelectedItem = selectedModel;
            }
            else if (model.Items.Count > 0)
            {
                model.SelectedIndex = 0;
            }
        }
        private void LoadPropsToControl(KitServices service, KitServiceProviderItem provider, Control parent)
        {
            parent.Tag = null;
            parent.Controls.Clear();
            parent.Tag = provider;
            if (provider.Props?.Count > 0)
            {
                int paddingLeft = 10;
                int paddingTop = 10;
                int margin = 50;
                for (int i = 0; i < provider.Props.Count; ++i)
                {
                    //title
                    var prop = provider.Props[i];
                    Label title = new Label();
                    title.AutoSize = true;
                    title.Location = new Point(paddingLeft, paddingTop + margin * i);
                    title.Name = $"tp{service.FriendlyNameEn()}Lb_{ prop.KeyName}";
                    title.Text = $"{prop.KeyName} ( {prop.ValueDesc} )";
                    parent.Controls.Add(title);
                    //value
                    if (prop.ValueType == "boolean")
                    {
                        CheckBox value = new CheckBox();
                        value.Location = new Point(paddingLeft, paddingTop + margin * i + 15);
                        value.Size = new Size(22, 22);
                        value.Name = $"tp{service.FriendlyNameEn()}{prop.ValueType}_{ prop.KeyName}";
                        value.Checked = (bool)prop.CurrentValue;
                        parent.Controls.Add(value);

                    }
                    else if (prop.ValueType == "number")
                    {
                        NumericUpDown value = new NumericUpDown();
                        value.Location = new Point(paddingLeft, paddingTop + margin * i + 15);
                        value.Size = new Size(200, 22);
                        value.Maximum = long.MaxValue;
                        value.Name = $"tp{service.FriendlyNameEn()}{prop.ValueType}_{ prop.KeyName}";
                        if (int.TryParse($"{prop.CurrentValue}", out int v))
                        {
                            value.Value = v;
                        }
                        parent.Controls.Add(value);
                    }
                    else
                    {
                        TextBox value = new TextBox();
                        value.Location = new Point(paddingLeft, paddingTop + margin * i + 15);
                        value.Size = new Size(200, 22);
                        value.Name = $"tp{service.FriendlyNameEn()}{prop.ValueType}_{ prop.KeyName}";
                        value.Text = $"{prop.CurrentValue}";
                        parent.Controls.Add(value);
                    }
                    //required 
                    Label required = new Label();
                    required.AutoSize = true;
                    required.Location = new Point(paddingLeft + 205, paddingTop + margin * i + 19);
                    required.Name = $"tp{service.FriendlyNameEn()}LbRequired_{ prop.KeyName}";
                    required.Text = $"{(prop.Required ? "必须" : "可选")}";
                    if (prop.Required)
                    {
                        required.ForeColor = Color.Red;
                    }
                    parent.Controls.Add(required);
                }
            }
        }

        private bool TryGetPropsFromControl(KitServices service, Control parent, out KitServiceProviderItem provider)
        {
            provider = parent.Tag as KitServiceProviderItem;
            if (provider == null)
            {
                return false;
            }
            if (provider.Props?.Count > 0)
            {
                foreach (var prop in provider.Props)
                {
                    var ctrlName = $"tp{service.FriendlyNameEn()}{prop.ValueType}_{ prop.KeyName}";
                    var ctrl = parent.Controls.Find(ctrlName, true).FirstOrDefault();
                    if (ctrl != null)
                    {
                        if (ctrl is CheckBox check)
                        {
                            prop.CurrentValue = check.Checked;
                        }
                        else if (ctrl is NumericUpDown num)
                        {
                            prop.CurrentValue = (long)num.Value;
                        }
                        else if (ctrl is TextBox txt)
                        {
                            prop.CurrentValue = txt.Text?.Trim() ?? "";
                        }
                        if (prop.Required && (prop.CurrentValue == null || $"{prop.CurrentValue}".IsEmpty()))
                        {
                            Utils.Error($"{prop.KeyName} 为必填参数");
                            ctrl.Focus();
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private async void FetchServiceConfig(KitServices service, ComboBox brand, ComboBox model, Control props)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            brand.Items.Clear();
            model.Items.Clear();
            formLogger.Info($"【获取配置-{service.FriendlyName()}】......");
            //var pb = this.ShowProgress($"正在获取'{service.FriendlyName()}'配置信息 ......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitServiceItem>>($"{baseUrl}/{service.ApiPathStr()}/config");
            //pb.Done();
            if (!OutRespLog($"【获取配置-{service.FriendlyName()}】", resp))
            {
                return;
            }
            if (resp?.Data?.ServiceProviders == null)
            {
                return;
            }
            LoadBrandToCombox(service, resp.Data.ServiceProviders, resp.Data.CurrentProvider, brand);
        }

        private async void SaveServiceConfig(KitServices service, ComboBox brand, ComboBox model, Control propsControl)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            if (!TryGetPropsFromControl(service, propsControl, out KitServiceProviderItem provider))
            {
                return;
            }
            //save 
            JsonObject props = new JsonObject();
            if (provider.Props.Count > 0)
            {
                foreach (var prop in provider.Props)
                {
                    props.Add(prop.KeyName, prop.CurrentValue);
                }
            }
            JsonObject req = new JsonObject()
            {
                {
                "currentProviderId",provider.ProviderId
                },
                {
                    "props",props
                }
            };
            formLogger.Info($"【保存配置-{service.FriendlyName()}】( {req.ToJsonString()} )......");
            var resp = await HttpUtils.SendHttpPut<BasicResp<KitServiceItem>>($"{baseUrl}/{service.ApiPathStr()}/config", req);
            //pb.Done();
            if (!OutRespLog($"【保存配置-{service.FriendlyName()}】", resp))
            {
                return;
            }
            Utils.Info($"【保存配置-{service.FriendlyName()}】成功");
        }
        private void CbBrand_SelectedIndexChanged(KitServices service, ComboBox brand, ComboBox model, Control props)
        {
            logger.Debug($"CbBrand_SelectedIndexChanged : {brand.Name} - {model.Name} - {props.Name}");
            model.Items.Clear();
            props.Controls.Clear();
            if (brand.SelectedItem == null)
            {
                return;
            }
            ItemEx selectedBrand = brand.SelectedItem as ItemEx;
            if (selectedBrand == null)
            {
                return;
            }
            var currentProvider = selectedBrand.UserData as KitServiceProviderItem;
            var providers = (List<KitServiceProviderItem>)selectedBrand.Tag;
            LoadModelToCombox(service, providers, currentProvider, model);
        }

        private void CbModel_SelectedIndexChanged(KitServices service, ComboBox brand, ComboBox model, Control props)
        {
            logger.Debug($"CbModel_SelectedIndexChanged : {brand.Name} - {model.Name} - {props.Name}");
            props.Controls.Clear();
            if (model.SelectedItem == null)
            {
                return;
            }
            ItemEx selectedModel = model.SelectedItem as ItemEx;
            if (selectedModel == null)
            {
                return;
            }
            var provider = (KitServiceProviderItem)selectedModel.Tag;
            LoadPropsToControl(service, provider, props);
        }

        #region 系统

        private void tpSystemCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemPanelProps);
        }

        private void tpSystemCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemPanelProps);
        }

        private void tpSystemGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemPanelProps);

        }

        private void tpSystemSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemPanelProps);
        }
        private void tpSystemCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.System, tpSystemLbState);
        }

        private async void tpSystemLedBlink_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            string ledNoStr = tpSystemTxLedNo.Text?.Trim() ?? "";
            if (ledNoStr.IsEmpty())
            {
                Utils.Error("请填写要控制的LED序号");
                tpSystemTxLedNo.Focus();
                return;
            }
            if (!int.TryParse(ledNoStr, out int ledNo) || ledNo < 0 || ledNo > 7)
            {
                Utils.Error("请正确填写要控制的LED序号");
                tpSystemTxLedNo.Focus();
                return;
            }
            string ledHzStr = tpSystemTxLedHz.Text?.Trim() ?? "";
            if (ledHzStr.IsEmpty())
            {
                Utils.Error("请填写闪烁频率");
                tpSystemTxLedHz.Focus();
                return;
            }
            if (!int.TryParse(ledHzStr, out int ledHz) || ledHz < 0 || ledNo > 15)
            {
                Utils.Error("请正确填写闪烁频率");
                tpSystemTxLedHz.Focus();
                return;
            }
            //var pb = this.ShowProgress("正在发送控制命令......");
            formLogger.Info($"【LED闪烁】LED :{ledNo} ,  频率: {ledHz}HZ ......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "ledNo",ledNo},
                    { "frequency",ledHz}
                });
            // pb.Done();
            OutRespLog($"【LED闪烁】LED :{ledNo} ,  频率: {ledHz}HZ", resp);
        }

        private async void tpSystemOffLed_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string ledNoStr = tpSystemTxLedNo.Text?.Trim() ?? "";
            if (ledNoStr.IsEmpty())
            {
                Utils.Error("请填写要控制的LED序号");
                tpSystemTxLedNo.Focus();
                return;
            }
            if (!int.TryParse(ledNoStr, out int ledNo) || ledNo < 0 || ledNo > 7)
            {
                Utils.Error("请正确填写要控制的LED序号");
                tpSystemTxLedNo.Focus();
                return;
            }
            formLogger.Info($"【关闭LED】LED :{ledNo}  ......");
            //var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.System.ApiPathStr()}/led/off",
                new JsonObject()
                {
                    { "ledNo",ledNo},
                });
            //pb.Done();
            OutRespLog($"【关闭LED】LED :{ledNo}", resp);
        }
        private bool TryParseHexToInt(string hex,out int v)
        {
            v = -1;
            try
            {
                v = Convert.ToInt32(hex, 16);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private async void SystemDevPowerCtl(int action)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            string devToCtlStr = tpSystemTxDevToPowerCtl.Text?.Trim() ?? "";
            if (devToCtlStr.IsEmpty())
            {
                Utils.Error("请填写要控制的设备");
                tpSystemTxDevToPowerCtl.Focus();
                return;
            }
           
            if (!TryParseHexToInt(devToCtlStr, out int devToCtl) || devToCtl < 0 || devToCtl > 0x7f)
            {
                Utils.Error("请正确填写要控制的设备");
                tpSystemTxDevToPowerCtl.Focus();
                return;
            }
            formLogger.Info($"【电源控制】action : {action} , devices : 0x{devToCtl:X2}  ......");
            //var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.System.ApiPathStr()}/power",
                new JsonObject()
                {
                    { "action",action },
                    { "devices",devToCtl},
                });
            //pb.Done();
            OutRespLog($"【电源控制】action : {action} , devices : 0x{devToCtl:X2}", resp);
        }
        private void tpSystelBtnPowerOff_Click(object sender, EventArgs e)
        {
            SystemDevPowerCtl(1);
        }

        private void tpSystelBtnPowerOn_Click(object sender, EventArgs e)
        {
            SystemDevPowerCtl(2);
        }
        private void tpSystelBtnRePower_Click(object sender, EventArgs e)
        {
            SystemDevPowerCtl(3);
        }
        #endregion

        #region 身份证采集
        private void tpIDReaderCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpIDReaderPanelProps);
        }

        private void tpIDReaderCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpIDReaderPanelProps);
        }

        private void tpIDReaderBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpIDReaderPanelProps);
        }

        private void tpIDReaderSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpIDReaderPanelProps);
        }
        private void tpIDReaderCheckState_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.IDReader, tpIDReaderLbState);
        }
        private async void tpIDReaderReadIDCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            string timeoutStr = tpIDReaderTbTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写超时时间");
                tpIDReaderTbTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写超时时间");
                tpIDReaderTbTimeout.Focus();
                return;
            }
            formLogger.Info("【读取身份证】...");
            // var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.IDReader.ApiPathStr()}/read?timeoutSec={timeout}");
            // pb.Done();
            OutRespLog("【读取身份证】", resp);
        }
        private async void tpIDReaderCancelRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl,out _))
            {
                return;
            }
            formLogger.Info("【取消读取身份证】...");
            //var pb = this.ShowProgress("正在发送取消读取命令......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.IDReader.ApiPathStr()}/cancel_read");
            //pb.Done();
            OutRespLog("【取消读取身份证】", resp);
        }

        #endregion

        #region 收发卡机
        private void tpCardBoxCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.CardBox, tpCardBoxCbBrand, tpCardBoxCbModel, tpCardBoxPanelProps);
        }

        private void tpCardBoxCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.CardBox, tpCardBoxCbBrand, tpCardBoxCbModel, tpCardBoxPanelProps);
        }

        private void tpCardBoxBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.CardBox, tpCardBoxCbBrand, tpCardBoxCbModel, tpCardBoxPanelProps);
        }

        private void tpCardBoxBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.CardBox, tpCardBoxCbBrand, tpCardBoxCbModel, tpCardBoxPanelProps);
        }

        private void tpCardBoxBtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.CardBox, tpCardBoxLbState);
        }

        private async void tpCardBoxBtnSendToWrite_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【发卡到写卡位置】...");
            //var pb = this.ShowProgress("发卡到写卡位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/send_to_write", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到写卡位置】", resp);
        }

        private async void tpCardBoxBtnSendToTake_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【发卡到取卡位置】...");
            //var pb = this.ShowProgress("发卡到取卡位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/send_to_take", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到取卡位置】", resp);

        }

        private async void tpCardBoxBtnSendToMouth_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【发卡到卡口位置】...");
            //var pb = this.ShowProgress("发卡到卡口位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/send_to_mouth", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到卡口位置】", resp);
        }

        private async void tpCardBoxBtnBackToRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpCardBoxBackTxTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待放卡超时时间");
                tpCardBoxBackTxTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待放卡超时时间");
                tpCardBoxBackTxTimeout.Focus();
                return;
            }
            formLogger.Info("【收卡到读卡位置】...");
            //var pb = this.ShowProgress("收卡到读卡位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/back_to_read?timeoutSec={timeout}", new JsonObject());
            //pb.Done();
            OutRespLog("【收卡到读卡位置】", resp);
        }

        private async void tpCardBoxBtnBackToBox_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpCardBoxBackTxTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待放卡超时时间");
                tpCardBoxBackTxTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待放卡超时时间");
                tpCardBoxBackTxTimeout.Focus();
                return;
            }
            formLogger.Info("【收卡到卡箱】...");
            //var pb = this.ShowProgress("收卡到卡箱......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/back_to_box?timeoutSec={timeout}", new JsonObject());
            //pb.Done();
            OutRespLog("【收卡到卡箱】", resp);
        }
        private async void tpCardBoxBtnCancelBack_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【取消收卡】...");
            //var pb = this.ShowProgress("取消收卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/cancel_back");
            //pb.Done();
            OutRespLog("【取消收卡】", resp);
        }

        private async void tpCardBoxBtnCheckCardAtTake_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【检测卡片是否取走】...");
            //var pb = this.ShowProgress("检测卡片是否取走......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/is_card_taken");
            //pb.Done();
            OutRespLog("【检测卡片是否取走】", resp);
        }

        private async void tpCardBoxBtnCheckCardAtRW_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【检测读写卡位置是否有卡】...");
            //var pb = this.ShowProgress("检测读写卡位置是否有卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.CardBox.ApiPathStr()}/is_card_at_read_write");
            //pb.Done();
            OutRespLog("【检测读写卡位置是否有卡】", resp);
        }

        #endregion

        #region 小票打印机
        private void tpPrinterCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.Printer, tpPrinterCbBrand, tpPrinterCbModel, tpPrinterPanelProps);
        }

        private void tpPrinterCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.Printer, tpPrinterCbBrand, tpPrinterCbModel, tpPrinterPanelProps);
        }

        private void tpPrinterBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.Printer, tpPrinterCbBrand, tpPrinterCbModel, tpPrinterPanelProps);
        }

        private void tpPrinterBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.Printer, tpPrinterCbBrand, tpPrinterCbModel, tpPrinterPanelProps);
        }

        private void tpPrinterBtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.Printer, tpPrinterLbState);
        }
        private async void tpPrinterBtnPrint_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string content = tpPrinterRtxtPrintContent.Text?.Trim() ?? "";
            if (content.IsEmpty())
            {
                Utils.Error("请填写打印内用");
                tpPrinterRtxtPrintContent.Focus();
                return;
            }
            if (!content.TryDeserializeJsonStr(out List<TicketStyledItem> contentJson))
            {
                Utils.Error("打印内用格式不正确");
                tpCardBoxBackTxTimeout.Focus();
                return;
            }
            formLogger.Info("【打印小票】...");
            //var pb = this.ShowProgress("打印小票......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.Printer.ApiPathStr()}/print_ticket", contentJson);
            //pb.Done();
            OutRespLog("【打印小票】", resp);
        }
        #endregion

        #region 二维码扫描器
        private void tpQRScannerCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.QRScanner, tpQRScannerCbBrand, tpQRScannerCbModel, tpQRScannerPanelProps);
        }

        private void tpQRScannerCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.QRScanner, tpQRScannerCbBrand, tpQRScannerCbModel, tpQRScannerPanelProps);
        }

        private void tpQRScannerBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.QRScanner, tpQRScannerCbBrand, tpQRScannerCbModel, tpQRScannerPanelProps);
        }

        private void tpQRScannerBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.QRScanner, tpQRScannerCbBrand, tpQRScannerCbModel, tpQRScannerPanelProps);
        }

        private void tpQRScannerBtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.QRScanner, tpQRScannerLbState);
        }
        private async void tpQRScannerBtnScan_Click(object sender, EventArgs e)
        {
            tpQRScannerQrContent.Text = "";
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpQRScannerTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写超时时间");
                tpQRScannerTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写超时时间");
                tpQRScannerTimeout.Focus();
                return;
            }
            formLogger.Info("【扫描二维码】...");
            // var pb = this.ShowProgress("扫描二维码......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<QRCode>>($"{baseUrl}/{KitServices.QRScanner.ApiPathStr()}/scan?timeoutSec={timeout}");
            // pb.Done();
            OutRespLog("【扫描二维码】", resp);
            tpQRScannerQrContent.Text = resp?.Data?.QRCodeContent; 
        }

        private async void tpQRScannerBtnCancel_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【取消扫描二维码】...");
            //var pb = this.ShowProgress("取消扫描二维码......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.QRScanner.ApiPathStr()}/cancel_scan");
            //pb.Done();
            OutRespLog("【取消扫描二维码】", resp);
        }
        #endregion

        #region 房卡读写

        private void tpRoomCardCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.RoomCard, tpRoomCardCbBrand, tpRoomCardCbModel, tpRoomCardPanelProps);
        }

        private void tpRoomCardCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.RoomCard, tpRoomCardCbBrand, tpRoomCardCbModel, tpRoomCardPanelProps);
        }

        private void tpRoomCardBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.RoomCard, tpRoomCardCbBrand, tpRoomCardCbModel, tpRoomCardPanelProps);
        }

        private void tpRoomCardBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.RoomCard, tpRoomCardCbBrand, tpRoomCardCbModel, tpRoomCardPanelProps);
        }
        private void tpRoomCardbtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.RoomCard, tpRoomCardLbState);
        }

        private async void tpRoomBtnCardWriteCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpRoomCardTxRoomToW.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpRoomCardTxRoomToW.Focus();
                return;
            }
            var start = tpRoomCardTimeStartToW.Value;
            var end = tpRoomCardTimeEndToW.Value;
            var req = new JsonObject() {
                { "room",room},
                { "start",start.Unix()},
                { "end",end.Unix()},
            };
            formLogger.Info("【写卡】...");
            // var pb = this.ShowProgress("写卡......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/write", req);
            // pb.Done();
            OutRespLog("【写卡】", resp);
        }

        private async void tpRoomBtnWriteAndSend_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpRoomCardTxRoomToW.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpRoomCardTxRoomToW.Focus();
                return;
            }
            var start = tpRoomCardTimeStartToW.Value;
            var end = tpRoomCardTimeEndToW.Value;
            var req = new JsonObject() {
                { "room",room},
                { "start",start.Unix()},
                { "end",end.Unix()},
            };
            formLogger.Info("【写卡及发卡】...");
            // var pb = this.ShowProgress("写卡及发卡......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/write_and_send", req);
            // pb.Done();
            OutRespLog("【写卡及发卡】", resp);
        }

        private async void tpRoomBtnCopy_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpRoomCardTxRoomToW.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpRoomCardTxRoomToW.Focus();
                return;
            }
            var start = tpRoomCardTimeStartToW.Value;
            var end = tpRoomCardTimeEndToW.Value;
            var req = new JsonObject() {
                { "room",room},
                { "start",start.Unix()},
                { "end",end.Unix()},
            };
            formLogger.Info("【复制写卡】...");
            // var pb = this.ShowProgress("复制写卡......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/copy", req);
            // pb.Done();
            OutRespLog("【复制写卡】", resp);
        }

        private async void tpRoomBtnCopyAndSend_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpRoomCardTxRoomToW.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpRoomCardTxRoomToW.Focus();
                return;
            }
            var start = tpRoomCardTimeStartToW.Value;
            var end = tpRoomCardTimeEndToW.Value;
            var req = new JsonObject() {
                { "room",room},
                { "start",start.Unix()},
                { "end",end.Unix()},
            };
            formLogger.Info("【复制写卡及发卡】...");
            // var pb = this.ShowProgress("复制写卡及发卡......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/copy_and_send", req);
            // pb.Done();
            OutRespLog("【复制写卡及发卡】", resp);
        }

        private async void tpRoomBtnRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【读卡】...");
            // var pb = this.ShowProgress("读卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<RoomCardData>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/read");
            // pb.Done();
            if(!OutRespLog("【读卡】", resp)) { 
                return; 
            }
            tpRoomCardReadRoomNo.Text = resp?.Data?.Room;
            if (resp?.Data?.Start > 0)
            {
                tpRoomCardTimeStart.Text = resp?.Data?.Start.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (resp?.Data?.End > 0)
            {
                tpRoomCardTimeEnd.Text = resp?.Data?.End.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private async void tpRoomBtnBackAndRead_Click(object sender, EventArgs e)
        {
            tpRoomCardReadRoomNo.Text = "";
            tpRoomCardTimeStart.Text = "";
            tpRoomCardTimeEnd.Text = "";
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpRoomCardTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待放卡超时时间");
                tpRoomCardTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待放卡超时时间");
                tpRoomCardTimeout.Focus();
                return;
            }
            formLogger.Info("【收卡及读卡】...");
            // var pb = this.ShowProgress("收卡及读卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<RoomCardData>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/back_and_read");
            // pb.Done();
            if (!OutRespLog("【收卡及读卡】", resp))
            {
                return;
            }
            tpRoomCardReadRoomNo.Text = resp?.Data?.Room;
            if (resp?.Data?.Start > 0)
            {
                tpRoomCardTimeStart.Text = resp?.Data?.Start.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (resp?.Data?.End > 0)
            {
                tpRoomCardTimeEnd.Text = resp?.Data?.End.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        private  async void tpRoomBtnCancelBackAndRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【取消收卡及读卡】...");
            //var pb = this.ShowProgress("取消收卡及读卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/cancel_back_and_read");
            //pb.Done();
            OutRespLog("【取消收卡及读卡】", resp);
        }

        private async void tpRoomBtnClearCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【注销卡】...");
            // var pb = this.ShowProgress("注销卡......");
            var resp = await HttpUtils.SendHttpDelete<BasicResp<object>>($"{baseUrl}/{KitServices.RoomCard.ApiPathStr()}/clear");
            // pb.Done();
            if (!OutRespLog("【注销卡】", resp))
            {
                return;
            }
        }
        #endregion

        #region PSB
        private void tpPSBCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.PSB, tpPSBCbBrand, tpPSBCbModel, tpPSBPanelProps);
        }

        private void tpPSBCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.PSB, tpPSBCbBrand, tpPSBCbModel, tpPSBPanelProps);
        }

        private void tpPSBBtnGeConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.PSB, tpPSBCbBrand, tpPSBCbModel, tpPSBPanelProps);
        }

        private void tpPSBBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.PSB, tpPSBCbBrand, tpPSBCbModel, tpPSBPanelProps);
        }

        private void tpPSBBtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.PSB, tpPSBLbState);
        }

        private async void tpPSBBtnInReadID_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【读取身份证】...");
            // var pb = this.ShowProgress("读取身份证......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<IdCardInfo>>($"{baseUrl}/{KitServices.IDReader.ApiPathStr()}/read?timeoutSec=10");
            // pb.Done();
            OutRespLog("【读取身份证】", resp);
            if (resp?.Data == null) { return; }
            var info = resp.Data;
            tpPSBTxInName.Text = info.Name;
            tpPSBTxInIDNo.Text = info.Number;
            tpPSBTxInSex.Text = info.Sex;
            tpPSBTxInNation.Text = info.Nation;
            tpPSBTxInBirthday.Text = info.Birthday;
            tpPSBTxInAddress.Text = info.Address;
            tpPSBTxInOrg.Text = info.IssuingAuthority;
            tpPSBTxInValidBegin.Text = info.ValidBegin;
            tpPSBTxInValidEnd.Text = info.ValidEnd;
            tpPSBTxInPhoto.Text = info.ImageBase64;
        }

        private async void tpPSBBtnIn_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpPSBTxInRoom.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpPSBTxInRoom.Focus();
                return;
            }
            string name = tpPSBTxInName.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写姓名");
                tpPSBTxInName.Focus();
                return;
            }
            string idNo = tpPSBTxInIDNo.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写身份证号");
                tpPSBTxInIDNo.Focus();
                return;
            }
            string sex = tpPSBTxInSex.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写性别");
                tpPSBTxInSex.Focus();
                return;
            }
            string nation = tpPSBTxInNation.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写民族");
                tpPSBTxInNation.Focus();
                return;
            }
            string birthday = tpPSBTxInBirthday.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写出生日期");
                tpPSBTxInBirthday.Focus();
                return;
            }
            string address = tpPSBTxInAddress.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写地址");
                tpPSBTxInAddress.Focus();
                return;
            }
            string grantDept = tpPSBTxInOrg.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写签发机关");
                tpPSBTxInOrg.Focus();
                return;
            }
            string validBegin = tpPSBTxInValidBegin.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写证件有效期");
                tpPSBTxInValidBegin.Focus();
                return;
            }
            string validEnd = tpPSBTxInValidEnd.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写证件有效期");
                tpPSBTxInValidEnd.Focus();
                return;
            }
            string photo = tpPSBTxInPhoto.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写身份证图片的Base64");
                tpPSBTxInPhoto.Focus();
                return;
            }
            string curPhoto = tpPSBTxInCurrentPhoto.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写现场照片的Base64");
                tpPSBTxInCurrentPhoto.Focus();
                return;
            }
            var inTime = tpPSBTimeInTime.Value.Unix();
            var outTime = tpPSBTimeOutTime.Value.Unix();
            //
            var req = new JsonObject() {
                { "roomNo",room},
                { "number",idNo},
                { "name",name},
                { "sex",sex},
                { "nation",nation},
                { "birthday",birthday},
                { "address",address},
                { "grantDept",grantDept},
                { "validBegin",validBegin},
                { "validEnd",validEnd},
                { "photo",photo},
                { "curPhoto",curPhoto},
                { "inTime",inTime},
                { "outTime",outTime},
            };
            formLogger.Info("【入住上传】...");
            // var pb = this.ShowProgress("读取身份证......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<IdCardInfo>>($"{baseUrl}/{KitServices.PSB.ApiPathStr()}in", req);
            // pb.Done();
            OutRespLog("【入住上传】", resp);

        }


        private async void tpPSBBtnOut_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpPSBTxOutRoom.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpPSBTxOutRoom.Focus();
                return;
            }
            string idNo = tpPSBTxOutIDNo.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写身份证号");
                tpPSBTxOutIDNo.Focus();
                return;
            }
            formLogger.Info("【离店上传】...");
            // var pb = this.ShowProgress("离店上传......");
            var resp = await HttpUtils.SendHttpDelete<BasicResp<object>>($"{baseUrl}/{KitServices.PSB.ApiPathStr()}/out?roomNo={room}&number={idNo}");
            // pb.Done();
            OutRespLog("【离店上传】", resp);
        }

        private async void tpPSBBtnSwap_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpPSBTxSwapRoom.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写原房间号");
                tpPSBTxSwapRoom.Focus();
                return;
            }
            string newRoom = tpPSBTxSwapNewRoom.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写新房间号");
                tpPSBTxSwapNewRoom.Focus();
                return;
            }
            string idNo = tpPSBTxSwapIDNo.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写身份证号");
                tpPSBTxSwapIDNo.Focus();
                return;
            }
            var req = new JsonObject() {
                { "oldRoomNo",room},
                { "newRoomNo",newRoom},
                { "number",idNo},
            };
            formLogger.Info("【换房上传】...");
            // var pb = this.ShowProgress("换房上传......");
            var resp = await HttpUtils.SendHttpPut<BasicResp<object>>($"{baseUrl}/{KitServices.PSB.ApiPathStr()}/swap",req);
            // pb.Done();
            OutRespLog("【换房上传】", resp);
        }

        private async void tpPSBBtnStay_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string room = tpPSBTxStayRoom.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写房间号");
                tpPSBTxStayRoom.Focus();
                return;
            }
            string idNo = tpPSBTxStayIDNo.Text?.Trim() ?? "";
            if (room.IsEmpty())
            {
                Utils.Error("请填写身份证号");
                tpPSBTxStayIDNo.Focus();
                return;
            }
            var outTime = tpPSBTimeStayOut.Value.Unix();
            var req = new JsonObject() {
                { "roomNo",room},
                { "outTime",outTime},
                { "number",idNo},
            };
            formLogger.Info("【续住上传】...");
            // var pb = this.ShowProgress("续住上传......");
            var resp = await HttpUtils.SendHttpPut<BasicResp<object>>($"{baseUrl}/{KitServices.PSB.ApiPathStr()}/stay", req);
            // pb.Done();
            OutRespLog("【续住上传】", resp);
        }

        #endregion

        #region 刷卡支付

        private void tpPayPiCbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbBrand_SelectedIndexChanged(KitServices.PayPi, tpPayPiCbBrand, tpPayPiCbModel, tpPayPiPanelProps);
        }

        private void tpPayPiCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.PayPi, tpPayPiCbBrand, tpPayPiCbModel, tpPayPiPanelProps);
        }

        private void tpPayPiBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.PayPi, tpPayPiCbBrand, tpPayPiCbModel, tpPayPiPanelProps);
        }

        private void tpPayPiBtnSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.PayPi, tpPayPiCbBrand, tpPayPiCbModel, tpPayPiPanelProps);
        }

        private void tpPayPiBtnCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.PayPi, tpPayPiLbState);
        }
        private CancelToken wsCancelToken = new CancelToken();
        private async void tpPayPiBtnInit_Click(object sender, EventArgs e)
        {
            OutPayPiInfo("", true);
            if (!TryGetHwKitBaseUrl(out string baseUrl, out string wsBaseUrl))
            {
                return;
            }
            string hoelId = tpPayPiTxtHotelId.Text?.Trim() ?? "";
            if (hoelId.IsEmpty())
            {
                Utils.Error("请填写酒店ID");
                tpPayPiTxtHotelId.Focus();
                return;
            }
            string tradeNo = tpPayPiTxtTradeNo.Text?.Trim() ?? "";
            if (tradeNo.IsEmpty())
            {
                Utils.Error("请填写交易流水号");
                tpPayPiTxtTradeNo.Focus();
                return;
            }
            formLogger.Info("【刷卡交易-初始化】...");
            var req = new JsonObject() {
                { "projectId",hoelId},
                { "tradeNo",tradeNo},
            };
            wsCancelToken.Reset();
            StartWsEventHandler(wsBaseUrl, wsCancelToken);
            // var pb = this.ShowProgress("刷卡交易-初始化......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/bank_card_init", req);
            // pb.Done();
            OutRespLog("【刷卡交易-初始化】", resp);
        }

        private async void tpPayPiBtnEnterCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpPayPiEnterCardTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待插卡超时时间");
                tpPayPiEnterCardTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待插卡超时时间");
                tpRoomCardTimeout.Focus();
                return;
            }
            formLogger.Info("【启用进卡】...");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/enter_card?timeoutSec={timeout}",new JsonObject());
            OutRespLog("【启用进卡】", resp);
        }

        private async void tpPayPiBtnReadCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpPayPiEnterCardTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待插卡超时时间");
                tpPayPiEnterCardTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待插卡超时时间");
                tpRoomCardTimeout.Focus();
                return;
            }
            formLogger.Info("【读银行卡】...");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/read_card");
            if(OutRespLog("【读银行卡】", resp))
            {
                OutPayPiInfo($">>>>>\n银行卡信息:{resp.Data.ToJsonString(Formatting.Indented)}");
            }
        }

        private async void tpPayPiBtnEnterAndReadCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            string timeoutStr = tpPayPiEnterCardTimeout.Text?.Trim() ?? "";
            if (timeoutStr.IsEmpty())
            {
                Utils.Error("请填写等待插卡超时时间");
                tpPayPiEnterCardTimeout.Focus();
                return;
            }
            if (!int.TryParse(timeoutStr, out int timeout) || timeout < 0 || timeout > 120)
            {
                Utils.Error("请正确填写等待插卡超时时间");
                tpRoomCardTimeout.Focus();
                return;
            }
            formLogger.Info("【启用进卡及读银行卡】...");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/enter_and_read_card?timeoutSec={timeout}");
            if (OutRespLog("【启用进卡及读银行卡】", resp))
            {
                OutPayPiInfo($">>>>>\n银行卡信息:{resp.Data.ToJsonString(Formatting.Indented)}");
            }
        }
        private void StartWsEventHandler(string wsBaseUrl, CancelToken cancelToken)
        {
            CancellationToken cancellation = new CancellationToken();
            //链接websocket 接收按键信息
            Task.Run(async () => {
                var url = $"{wsBaseUrl}/ws/event_callback";
                var cli = new ClientWebSocket();
                formLogger.Info($"websocket connect  to {url} ...");
                await cli.ConnectAsync(new Uri(url), cancellation);
                formLogger.Info("websocket connect success");
                do
                {
                    if (cancelToken.IsCanceled())
                    {
                        break;
                    }
                    var rcvBytes = new byte[4096];
                    var rcvBuffer = new ArraySegment<byte>(rcvBytes);
                    WebSocketReceiveResult rcvResult = await cli.ReceiveAsync(rcvBuffer, cancellation);
                    byte[] msgBytes = rcvBuffer.Skip(rcvBuffer.Offset).Take(rcvResult.Count).ToArray();
                    string msg = Encoding.UTF8.GetString(msgBytes);
                    if (msg.TryDeserializeJsonStr<KitEventArg>(out KitEventArg arg))
                    {
                        formLogger.Info($"[Event-{arg.EventStr}] from {arg.Source}, msg = {arg.Message} , >> {arg.Data?.ToJsonString()} ");
                        if (arg.Source == "PayPi")
                        {
                            OutPayPiInfo($"[Event-{arg.EventStr}]  msg = {arg.Message} , >> {arg.Data?.ToJsonString()} ");
                        }
                    }
                } while (true);
            });
        }
        private async void tpPayPiBtnStartPin_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【启动密码键盘】...");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/start_pin",new JsonObject());
            if(!OutRespLog("【启动密码键盘】", resp))
            {
                return;
            }
            OutPayPiInfo($">>>>>\n密码键盘启动成功，开始监听键盘输入");
        }
        private void OutPayPiInfo(string msg,bool clear =false)
        {
            this.Invoke((Action) delegate{
                if (clear)
                {
                    tpPayPiRtxtLog.Clear();
                }
                tpPayPiRtxtLog.AppendText($"\n{msg}");
            });
        }

        private async void tpPayPiBtnTradeTrans_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【刷卡交易处理】...");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/trade_trans", new JsonObject());
            if (!OutRespLog("【刷卡交易处理】", resp))
            {
                return;
            }
        }
        private async void tpPayPiBtnEjectCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【弹卡】...");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/eject_card", new JsonObject());
            if (!OutRespLog("【弹卡】", resp))
            {
                return;
            }
        }

        private async void tpPayPiBtnSwallowCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl, out _))
            {
                return;
            }
            formLogger.Info("【吞卡】...");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseUrl}/{KitServices.PayPi.ApiPathStr()}/swallow_card", new JsonObject());
            if (!OutRespLog("【吞卡】", resp))
            {
                return;
            }
        }
        #endregion
    }
   
}

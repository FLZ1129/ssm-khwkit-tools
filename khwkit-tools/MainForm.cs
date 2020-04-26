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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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

        private bool TryGetHwKitBaseUrl(out string url)
        {
            url = "";
            var ip = txHwkitIp.Text?.Trim() ?? "";
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
            url = $"http://{ip}:5000/api";
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
            txHwVersion.Text = resp.Data.HwVersion;
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
            if (!TryGetHwKitBaseUrl(out string baseUrl))
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
            if (!TryGetHwKitBaseUrl(out string baseUrl))
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
            if (!TryGetHwKitBaseUrl(out string baseUrl))
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
            CbBrand_SelectedIndexChanged(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemGbProps);
        }

        private void tpSystemCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemGbProps);
        }

        private void tpSystemGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemGbProps);

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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "ledNo",ledNo},
                });
            //pb.Done();
            OutRespLog($"【关闭LED】LED :{ledNo}", resp);
        }
        private async void SystemDevPowerCtl(int action)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            if (!int.TryParse(devToCtlStr, out int devToCtl) || devToCtl < 0 || devToCtl > 0x7f)
            {
                Utils.Error("请正确填写要控制的设备");
                tpSystemTxDevToPowerCtl.Focus();
                return;
            }
            formLogger.Info($"【电源控制】action : {action} , devices : {devToCtl}  ......");
            //var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "action ",action },
                    { "devices ",devToCtl},
                });
            //pb.Done();
            OutRespLog("【电源控制】action : {action} , devices : {devToCtl}", resp);
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
            CbBrand_SelectedIndexChanged(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpRoomCardPanelProps);
        }

        private void tpIDReaderCbModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            CbModel_SelectedIndexChanged(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpRoomCardPanelProps);
        }

        private void tpIDReaderBtnGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpRoomCardPanelProps);
        }

        private void tpIDReaderSaveConfig_Click(object sender, EventArgs e)
        {
            SaveServiceConfig(KitServices.IDReader, tpIDReaderCbBrand, tpIDReaderCbModel, tpRoomCardPanelProps);
        }
        private void tpIDReaderCheckState_Click(object sender, EventArgs e)
        {
            ServiceStateCheck(KitServices.IDReader, tpIDReaderLbState);
        }
        private async void tpIDReaderReadIDCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseurl}/{KitServices.IDReader.ApiPathStr()}/read?timeoutSec={timeout}");
            // pb.Done();
            OutRespLog("【读取身份证】", resp);
        }
        private async void tpIDReaderCancelRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【取消读取身份证】...");
            //var pb = this.ShowProgress("正在发送取消读取命令......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseurl}/{KitServices.IDReader.ApiPathStr()}/cancel_read");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【发卡到写卡位置】...");
            //var pb = this.ShowProgress("发卡到写卡位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/send_to_write", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到写卡位置】", resp);
        }

        private async void tpCardBoxBtnSendToTake_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【发卡到取卡位置】...");
            //var pb = this.ShowProgress("发卡到取卡位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/send_to_take", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到取卡位置】", resp);

        }

        private async void tpCardBoxBtnSendToMouth_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【发卡到卡口位置】...");
            //var pb = this.ShowProgress("发卡到卡口位置......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/send_to_mouth", new JsonObject());
            //pb.Done();
            OutRespLog("【发卡到卡口位置】", resp);
        }

        private async void tpCardBoxBtnBackToRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/back_to_read?timeoutSec={timeout}", new JsonObject());
            //pb.Done();
            OutRespLog("【收卡到读卡位置】", resp);
        }

        private async void tpCardBoxBtnBackToBox_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/back_to_box?timeoutSec={timeout}", new JsonObject());
            //pb.Done();
            OutRespLog("【收卡到卡箱】", resp);
        }

        private async void tpCardBoxBtnCheckCardAtTake_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【检测卡片是否取走】...");
            //var pb = this.ShowProgress("检测卡片是否取走......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/is_card_taken");
            //pb.Done();
            OutRespLog("【检测卡片是否取走】", resp);
        }

        private async void tpCardBoxBtnCheckCardAtRW_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【检测读写卡位置是否有卡】...");
            //var pb = this.ShowProgress("检测读写卡位置是否有卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseurl}/{KitServices.CardBox.ApiPathStr()}/is_card_at_read_write");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.Printer.ApiPathStr()}/print_ticket", contentJson);
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpGet<BasicResp<QRCode>>($"{baseurl}/{KitServices.QRScanner.ApiPathStr()}/scan?timeoutSec={timeout}");
            // pb.Done();
            OutRespLog("【扫描二维码】", resp);
            tpQRScannerQrContent.Text = resp?.Data?.QRCodeContent; 
        }

        private async void tpQRScannerBtnCancel_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【取消扫描二维码】...");
            //var pb = this.ShowProgress("取消扫描二维码......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<object>>($"{baseurl}/{KitServices.QRScanner.ApiPathStr()}/cancel_scan");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/write", req);
            // pb.Done();
            OutRespLog("【写卡】", resp);
        }

        private async void tpRoomBtnWriteAndSend_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/write_and_send", req);
            // pb.Done();
            OutRespLog("【写卡及发卡】", resp);
        }

        private async void tpRoomBtnCopy_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/copy", req);
            // pb.Done();
            OutRespLog("【复制写卡】", resp);
        }

        private async void tpRoomBtnCopyAndSend_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<QRCode>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/copy_and_send", req);
            // pb.Done();
            OutRespLog("【复制写卡及发卡】", resp);
        }

        private async void tpRoomBtnRead_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【读卡】...");
            // var pb = this.ShowProgress("读卡......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<RoomCardData>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/read");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpGet<BasicResp<RoomCardData>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/back_and_read");
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

        private async void tpRoomBtnClearCard_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【注销卡】...");
            // var pb = this.ShowProgress("注销卡......");
            var resp = await HttpUtils.SendHttpDelete<BasicResp<object>>($"{baseurl}/{KitServices.RoomCard.ApiPathStr()}/clear");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
            {
                return;
            }
            formLogger.Info("【读取身份证】...");
            // var pb = this.ShowProgress("读取身份证......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<IdCardInfo>>($"{baseurl}/{KitServices.IDReader.ApiPathStr()}/read?timeoutSec=10");
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
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPost<BasicResp<IdCardInfo>>($"{baseurl}/{KitServices.PSB.ApiPathStr()}in", req);
            // pb.Done();
            OutRespLog("【入住上传】", resp);

        }


        private async void tpPSBBtnOut_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpDelete<BasicResp<object>>($"{baseurl}/{KitServices.PSB.ApiPathStr()}/out?roomNo={room}&number={idNo}");
            // pb.Done();
            OutRespLog("【离店上传】", resp);
        }

        private async void tpPSBBtnSwap_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPut<BasicResp<object>>($"{baseurl}/{KitServices.PSB.ApiPathStr()}/swap",req);
            // pb.Done();
            OutRespLog("【换房上传】", resp);
        }

        private async void tpPSBBtnStay_Click(object sender, EventArgs e)
        {
            if (!TryGetHwKitBaseUrl(out string baseurl))
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
            var resp = await HttpUtils.SendHttpPut<BasicResp<object>>($"{baseurl}/{KitServices.PSB.ApiPathStr()}/stay", req);
            // pb.Done();
            OutRespLog("【续住上传】", resp);
        }

        #endregion

    }
   
}

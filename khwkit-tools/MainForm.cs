using CrazySharp.Base;
using CrazySharp.Std;
using khwkit.Beans;
using khwkit.Core;
using khwkit.Enums;
using khwkit_tools.Beans;
using System.Windows.Forms;
using System.Collections;
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

namespace khwkit_tools
{
    public partial class MainForm : BaseForm
    {
        private const string small_rabbit_baseurl = "https://small-rabbit.pujie88.com/api/v1";
        private static Logger logger = LogManager.GetCurrentClassLogger();//日志类
        private static Logger formLogger = null;
        public MainForm()
        {
            InitializeComponent();
        }
        protected override void OnCreate()
        {
            Defaults.UseDefaultJsonSetting();
            //初始化formLogger
            formLogger = initLogger(this, this.rtbInfoOut);
            formLogger.Info("初始化成功");
            txHwkitIp.Text = "127.0.0.1";
            // FetchSummaryAsync($"http://{txHwkitIp.Text}:5000/api");
            txLoalIp.Text = Utils.LocalIp();
        }
        private void txTvId_TextChanged(object sender, EventArgs e)
        {
            string tmp = txTvId.Text?.Replace(" ", "");
            txTvId.Text = tmp;
        }
        private void btnFetchSummary_Click(object sender, EventArgs e)
        {
            if(!TryGetHwKitBaseUrl(out string baseurl))
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
            var pb = this.ShowProgress("正在上报远程信息......");
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

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

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
            if(!IPAddress.TryParse(ip,out _))
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
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitSystemSummary>>($"{baseUrl}/system/summary");
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            txSN.Text = resp.Data.SN;
            txSoftVersion.Text = resp.Data.SwVersion;
            txHwVersion.Text = resp.Data.HwVersion;
        }
        public class ItemEx
        {
            public object Tag;
            public string Text;
            public ItemEx(string text, object tag)
            {
                this.Text = text;
                this.Tag = tag;
            }
            public override string ToString()
            {
                return this.Text;
            }
        }
        private async void ServiceStateCheck( KitServices service, Label stateLb)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl))
            {
                return;
            }
            var pb = this.ShowProgress($"正在检测'{service.FriendlyName()}'状态信息 ......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitHeathState>>($"{baseUrl}/{service.ApiPathStr()}/check");
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            formLogger.Info(resp.ToJsonString());
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
        private async void FetchServiceConfig(KitServices service, ComboBox brand, ComboBox model, GroupBox props)
        {
            if (!TryGetHwKitBaseUrl(out string baseUrl))
            {
                return;
            }
            brand.Items.Clear();
            model.Items.Clear();
            var pb = this.ShowProgress($"正在获取'{service.FriendlyName()}'配置信息 ......");
            var resp = await HttpUtils.SendHttpGet<BasicResp<KitServiceItem>>($"{baseUrl}/{service.ApiPathStr()}/config");
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            Dictionary<string, List<KitServiceProviderItem>> providerMap = new Dictionary<string, List<KitServiceProviderItem>>();
            foreach (var provider in resp.Data.ServiceProviders)
            {
                if (!providerMap.ContainsKey(provider.GroupName))
                {
                    providerMap[provider.GroupName] = new List<KitServiceProviderItem>();
                }
                providerMap[provider.GroupName].Add(provider);
            }
            var currentProvider = resp.Data.CurrentProvider;
            //品牌
            ItemEx selectedBrand = null;
            foreach (var kv in providerMap)
            {
                var item = new ItemEx(kv.Key, kv.Value);
                if (kv.Value.Any(it => it.ProviderId == currentProvider?.ProviderId))
                {
                    selectedBrand = item;
                }
                brand.Items.Add(item);
            }
            brand.SelectedItem = selectedBrand;
            //型号
            if (selectedBrand != null)
            {
                ItemEx selectedModel = null;
                var providers = (List<KitServiceProviderItem>)selectedBrand.Tag;
                foreach (var provider in providers)
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
                    var provider = (KitServiceProviderItem)selectedModel.Tag;
                    if (provider.Props?.Count > 0)
                    {
                        for (int i = 0; i < provider.Props.Count; ++i)
                        {
                            var prop = provider.Props[i];
                            Label title = new Label();
                            title.Text = prop.KeyName;
                        }
                    }
                }
            }
        }
       
        private async void SaveServiceConfig(KitServices service, ComboBox brand, ComboBox model, GroupBox props){
            if (!TryGetHwKitBaseUrl(out string baseUrl))
            {
                return;
            }

        }
        private void CbBrand_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox brand = sender as ComboBox;
            if (brand.SelectedItem == null)
            {
                return;
            }
            ItemEx selectedBrand = brand.SelectedItem as ItemEx;


        }

        private void CbModel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #region 系统

        private void tpSystemGetConfig_Click(object sender, EventArgs e)
        {
            FetchServiceConfig(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemProps);
        
        }

        private void tpSystemSaveConfig_Click(object sender, EventArgs e)
        {
            //TODO 
            SaveServiceConfig(KitServices.System, tpSystemCbBrand, tpSystemCbModel, tpSystemProps);
        }
        private void tpSystemCheck_Click(object sender, EventArgs e)
        {
            ServiceStateCheck( KitServices.System,tpSystemLbState);
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
            if(!int.TryParse(ledNoStr,out int ledNo)|| ledNo < 0 || ledNo > 7)
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

            var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "ledNo",ledNo},
                    { "frequency",ledHz}
                });
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            formLogger.Info(resp.ToJsonString());
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
            var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "ledNo",ledNo},
                });
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            formLogger.Info(resp.ToJsonString());
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
            var pb = this.ShowProgress("正在发送控制命令......");
            var resp = await HttpUtils.SendHttpPost<BasicResp<object>>($"{baseurl}/{KitServices.System.ApiPathStr()}/led/blink",
                new JsonObject()
                {
                    { "action ",action },
                    { "devices ",devToCtl},
                });
            pb.Done();
            if (!resp.Ok)
            {
                Utils.Error(resp.Error);
                return;
            }
            formLogger.Info(resp.ToJsonString());
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

        #endregion
    }
}

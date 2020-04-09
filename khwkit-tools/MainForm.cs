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

namespace khwkit_tools
{
    public partial class MainForm : BaseForm
    {
        private string khwkitBaseUrl = $"http://127.0.0.1:5000/api";
        private const string small_rabbit_baseurl = "https://small-rabbit.pujie88.com/api/v1";
        public MainForm()
        {
            InitializeComponent();
        }
        protected override void OnCreate()
        {
            txHwkitIp.Text = "127.0.0.1";
            // FetchSummaryAsync($"http://{txHwkitIp.Text}:5000/api");
            txLoalIp.Text = Utils.LocalIp();
        }
        private void btnFetchSummary_Click(object sender, System.EventArgs e)
        {
            FetchSummaryAsync($"http://{txHwkitIp.Text}:5000/api");
        }

        private void btnSetupTv_Click(object sender, System.EventArgs e)
        {

        }

        private async void btnReportTvId_Click(object sender, System.EventArgs e)
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

        private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
        {

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
        public class itemEx
        {
            public object Tag;
            public string Text;
            public itemEx(string text, object tag)
            {
                this.Tag = tag;
                this.Text = text;
            }
            public override string ToString()
            {
                return this.Text;
            }
        }
        private async void FetchServiceConfig(string baseUrl, KitServices service, ComboBox brand, ComboBox model, GroupBox props)
        {
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
            itemEx selectedBrand = null;
            foreach (var kv in providerMap)
            {
                var item = new itemEx(kv.Key, kv.Value);
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
                itemEx selectedModel = null;
                var providers = (List<KitServiceProviderItem>)selectedBrand.Tag;
                foreach (var provider in providers)
                {
                    var item = new itemEx(provider.Name, provider);
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
    }
}

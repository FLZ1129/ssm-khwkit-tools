using CrazySharp.Base.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HotelRcuPowerCheck
{
    [Serializable]
    [DataGridBindAble]
    public class DgViewModel
    {
        [DgvCol("楼栋", true, DataGridViewAutoSizeColumnMode.DisplayedCells)]
        public string Building { get; set; }

        [DgvCol("楼层", true, DataGridViewAutoSizeColumnMode.DisplayedCells)]
        public string Floor { get; set; }

        [DgvCol("房号", true)]
        public string RoomName { get; set; }

        [DgvCol("主机SN")]
        public string RcuSn { get; set; }

        [DgvCol("在线状态")]
        public string IsOnline { get; set; }

        [DgvCol("取电状态")]
        public string PowerState { get; set; }

    }

    public class BaseHttpResponse
    {
        public int HttpStatusCode { get; set; } = 200;
        public string HttpResponse { get; set; }
        protected bool IsSuccessStatusCode => HttpStatusCode >= 200 && HttpStatusCode <= 209;
    }

    public class BasicReq
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }
    }

    public class BasicResp : BaseHttpResponse
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("resultId")]
        public int ResultId { get; set; } = -1;

        [JsonProperty("resultMsg")]
        public string ResultMsg { get; set; }

        public bool Ok => IsSuccessStatusCode && (ResultId == 1);

        [JsonIgnore]
        public string Message
        {
            get
            {
                if (!IsSuccessStatusCode)
                {
                    return HttpResponse;
                }
                return ResultMsg;
            }
        }
    }


    public class FetchCurrentProjectReq : BasicReq
    {
        public FetchCurrentProjectReq()
        {
            Cmd = "fetchCurrentProject";
        }
    }
    public class FetchCurrentProjectResp : BasicResp
    {
        [JsonProperty("projectAuthToken")]
        public string ProjectAuthToken { get; set; }

        [JsonProperty("projectName")]
        public string ProjectName { get; set; }
    }

    public class FetchRoomAuthTokensReq : BasicReq
    {
        [JsonProperty("useHierarchy")]
        public int useHierarchy { get; set; } = 0;

        [JsonProperty("projectAuthKey")]
        public string projectAuthKey { get; set; }

        public FetchRoomAuthTokensReq()
        {
            Cmd = "fetchRoomAuthTokens";
        }
    }

    public class FetchRoomAuthTokensResp : BasicResp
    {
        [JsonProperty("projectAuthKey")]
        public string ProjectAuthKey { get; set; }
        [JsonProperty("projectName")]
        public string ProjectName { get; set; }
        [JsonProperty("authRoomCount")]
        public int AuthRoomCount { get; set; }
        [JsonProperty("authRoomList")]
        public List<RoomAuthTokenItem> AuthRoomList { get; set; } = new List<RoomAuthTokenItem>();

    }
    public class RoomAuthTokenItem
    {
        [JsonProperty("roomNo")]
        public string RoomNo { get; set; }
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }
        [JsonProperty("floor")]
        public string Floor { get; set; }
        [JsonProperty("house")]
        public string House { get; set; }
    }

    public class FecthDeviceReq : BasicReq
    {
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }
        public FecthDeviceReq()
        {
            Cmd = "fetchDevices";
        }
    }

    public class FetchDeviceResp : BasicResp
    {
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }
        [JsonProperty("devInfos")]
        public List<DeviceInfoItem> DevInfos { get; set; } = new List<DeviceInfoItem>();

    }

    public class DeviceInfoItem
    {
        [JsonProperty("areaId")]
        public string AreaId { get; set; }

        [JsonProperty("devActionCode")]
        public int DevActionCode { get; set; }

        [JsonProperty("devId")]
        public string DevId { get; set; }

        [JsonProperty("devName")]
        public string DevName { get; set; }

        [JsonProperty("devSecretKey")]
        public string DevSecretKey { get; set; }

        [JsonProperty("devType")]
        public int DevType { get; set; }

        [JsonProperty("offLine")]
        public int OffLine { get; set; }=-1;



    }
}

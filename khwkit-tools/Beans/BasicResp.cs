using CrazySharp.Std;
using Newtonsoft.Json;

namespace khwkit_tools.Beans
{
    public class BasicResp<T>: BaseHttpResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; } = -1;
        [JsonProperty("message")]
        public string Message { get; set; }
        public T Data { get; set; }
        [JsonIgnore]
        public bool Ok => IsSuccessStatusCode && (Code == 0);

        [JsonIgnore]
        public string Error
        {
            get
            {
                if (!IsSuccessStatusCode)
                {
                    return HttpResponse;
                }
                return Message;
            }
        }
    }
}

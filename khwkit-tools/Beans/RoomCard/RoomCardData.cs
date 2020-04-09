using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace khwkit.Beans.RoomCard
{
    public class RoomCardData
    {
        [JsonProperty("room")]
        public string Room { get; set; }
        [JsonProperty("start")]
        public long Start { get; set; }
        [JsonProperty("end")]
        public long End { get; set; }
    }
}

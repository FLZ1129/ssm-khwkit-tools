using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace khwkit.Core
{
    //[SwaggerSchemaFilter(typeof(KitServiceProviderSchemaFilter))]
    public class KitServiceProviderItem
    {
        [JsonProperty("providerId")]
        public string ProviderId { get; set; }
        [JsonProperty("groupName")]
        public string GroupName { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("desc")]
        public string Desc { get; set; }
        [JsonProperty("props")]
        public List<PropertyItem> Props { get; set; } = new List<PropertyItem>();
        [JsonProperty("isNull")]
        public bool IsNull { get; set; }
    }
}

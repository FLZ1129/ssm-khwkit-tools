using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CrazySharp.Std
{
    public static class JsonUtils
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static string ToJsonString(this string obj)
        {
            return obj;
        }

        public static T ParseJson<T>(this string obj)
        {
            //需要在这里统一取出转义字符
            return JsonConvert.DeserializeObject<T>(obj);
        }

        public static bool TryDeserializeJsonStr<T>(this string jsonStr, out T data) {
            data = default(T);
            if (string.IsNullOrEmpty(jsonStr))
            {
                return false;
            }
            try
            {
                data = JsonConvert.DeserializeObject<T>(jsonStr);
                return data!=null;
            } catch (Exception e)
            {
                logger.Warn($"TryDeserializeJsonStr error: {e.Message}");
                return false;
            }
        }

        public static bool TryDeserializeJsonObject<T>(this JsonObject jObj, out T data) {
            return TryDeserializeJsonStr(jObj.ToJsonString(), out data);
        }
    }

    [Serializable]
    public class JsonObject : Dictionary<string, object>
    {
        #region make lint happy

        public JsonObject() {
        }

        protected JsonObject(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        #endregion make lint happy

        public static JsonObject New() {
            return new JsonObject();
        }
    }

    [Serializable]
    public class JsonArray : List<object>
    {
        public static JsonArray New() {
            return new JsonArray();
        }
    }
}
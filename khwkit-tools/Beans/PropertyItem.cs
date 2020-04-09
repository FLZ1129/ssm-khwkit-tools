using khwkit.Errors;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace khwkit.Core
{

    public class PropertyItem
    {

        [JsonProperty("keyName")]
        public string KeyName { get; set; }
        [JsonProperty("valueType")]
        public string ValueType { get; set; }
        [JsonProperty("valueDesc")]
        public string ValueDesc { get; set; }
        [JsonProperty("defaultValue")]
        public object DefaultValue { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("currentValue")]
        public object CurrentValue { get; set; }
        [JsonIgnore]
        private PropertyInfo RawProperty;

        public PropertyItem(PropertyInfo p) {
            RawProperty = p;
        }

        public object GetValue(object obj)
        {
            return RawProperty?.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (RawProperty == null || value==null) { return; }
            object valueToSet = value;
            string valueStr = valueToSet.ToString();
            var t = RawProperty.PropertyType;
            if (t == typeof(object))
            {

            }
            else if (t == typeof(bool))
            {
                valueToSet = bool.Parse(valueStr);
            }
            else if (t == typeof(byte))
            {
                valueToSet = byte.Parse(valueStr);
            }
            else if (t == typeof(short))
            {
                valueToSet = short.Parse(valueStr);
            }
            else if (t == typeof(ushort))
            {
                valueToSet = ushort.Parse(valueStr);
            }
            else if (t == typeof(uint))
            {
                valueToSet = uint.Parse(valueStr);
            }
            else if (t == typeof(int))
            {
                valueToSet = int.Parse(valueStr);
            }
            else if (t == typeof(long))
            {
                valueToSet = long.Parse(valueStr);
            }
            else if (t == typeof(ulong))
            {
                valueToSet = ulong.Parse(valueStr);
            }
            else if (t == typeof(float))
            {
                valueToSet = float.Parse(valueStr);
            }
            else if (t == typeof(double))
            {
                valueToSet = double.Parse(valueStr);
            }
            else if (t == typeof(string))
            {
                valueToSet = valueStr;
            }
            else
            {
                throw new Exception($"config property type '{t?.FullName??""}' not support yet");
            }
            RawProperty.SetValue(obj, valueToSet);
        }
    }
}

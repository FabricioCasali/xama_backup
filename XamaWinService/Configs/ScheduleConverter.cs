using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using XamaCore.Configs;

using XamaWinService.Helpers;

namespace XamaWinService.Configs
{
    public class ScheduleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ConfigSchedule));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var val = jo.Value<string>("type");
            var types = TypeHelper.GetTypesExtending<ConfigSchedule>();
            var converter = types.First(x => x.Name.Equals(val, StringComparison.InvariantCultureIgnoreCase));
            var o = jo.ToObject(converter);
            return o;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

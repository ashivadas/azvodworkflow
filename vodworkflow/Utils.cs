using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace vodworkflow
{
    public static class Utils
    {
        public static string SerializeObject(object @object)
        {
            return JsonConvert.SerializeObject(@object, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }
    }
}

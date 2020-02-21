using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace IdentityServer.Api.Tests
{
    public static class Extensions
    {
        private static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static HttpContent ToHttpContent(this object obj)
        {
            return new StringContent(obj.ToJson(), Encoding.UTF8, "application/json");
        }

        public static T ToObject<T>(this HttpResponseMessage response)
        {
            var json = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using BroadridgeTimeApp.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace BroadridgeTimeApp.Services
{
    public class WorldTimeApiClient
    {
        private const string BaseUrl = "http://worldtimeapi.org/api/timezone/";
        private readonly HttpClient httpClient;
        public string Timezone { get; set; }

        public WorldTimeApiClient()
        {
            httpClient = new HttpClient();
        }

        public WorldTimeApiClient(string timezone) : this()
        {
            this.Timezone = timezone;
        }

        public async Task<List<string>> GetTimezonesAsync()
        {
            string json = await httpClient.GetStringAsync(BaseUrl);
            List<string> timezones = JsonConvert.DeserializeObject<List<string>>(json);
            return timezones;
        }

        public async Task<DateTimeData> GetDateTimeDataAsync()
        {
            string url = BaseUrl + Timezone;

            HttpResponseMessage response = await httpClient.GetAsync(url);
            
            if(!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == HttpStatusCode.NotFound)
                    throw new InvalidTimeZoneException(Timezone + " is not a proper timezone.");
            }

            string json = await response.Content.ReadAsStringAsync();

            var settings = new JsonSerializerSettings
            {
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
            
            Dictionary<string, string> data = 
                JsonConvert.DeserializeObject<Dictionary<string, string>>(json, settings);

            int utcOffset = Convert.ToInt32(data["raw_offset"]) * 1000;
            if (Convert.ToBoolean(data["dst"]))
                utcOffset += Convert.ToInt32(data["dst_offset"]) * 1000;

            return new DateTimeData { UtcOffset = utcOffset, Timezone = this.Timezone }; ;
        }
    }
}

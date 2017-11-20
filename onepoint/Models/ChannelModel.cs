using Newtonsoft.Json;

namespace onepoint.Models
{
    public class ChannelModel
    {
        [JsonProperty("channel")]
        public string channel { get; set; }

        [JsonProperty("epg_name")]
        public string epg_name { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("genre")]
        public string genre { get; set; }

        [JsonProperty("radio")]
        public bool radio { get; set; }

        [JsonProperty("sources")]
        public string source { get; set; }

        [JsonProperty("lb")]
        public bool lb { get; set; }

        [JsonProperty("program")]
        public Program program { get; set; }

        public class Program
        {
            [JsonProperty("startts")]
            public string startts { get; set; }

            [JsonProperty("stopts")]
            public string stopts { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("desc")]
            public string desc { get; set; }
        }

        public EpgModel epg { get; set; }
    }
}

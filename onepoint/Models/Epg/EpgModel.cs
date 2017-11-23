using Newtonsoft.Json;
using System.Collections.Generic;

namespace onepoint.Models
{
    public class EpgModel
    {
        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("num")]
        public string num { get; set; }

        [JsonProperty("fullname")]
        public string fullname { get; set; }

        [JsonProperty("placeholder")]
        public string placeholder { get; set; }

        [JsonProperty("genre")]
        public string genre { get; set; }

        [JsonProperty("programme")]
        public List<Programme> programme { get; set; }

        public class Programme
        {
            [JsonProperty("start")]
            public string start { get; set; }

            [JsonProperty("stop")]
            public string stop { get; set; }

            [JsonProperty("startts")]
            public string startts { get; set; }

            [JsonProperty("stopts")]
            public string stopts { get; set; }

            [JsonProperty("title")]
            public string title { get; set; }

            [JsonProperty("desc")]
            public string desc { get; set; }
        }
    }
}

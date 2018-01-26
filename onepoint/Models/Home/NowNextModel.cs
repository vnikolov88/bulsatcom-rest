using System;

namespace onepoint.Models.Home
{
    public class NowNextModel
    {
        public string channel { get; set; }
        public string channel_logo_path { get; set; }
        public string channel_poster_url { get; set; }
        public string channel_poster_path { get; set; }

        public string titleNow { get; set; }
        public string descNow { get; set; }
        public string startNow { get; set; }
        public string stopNow { get; set; }

        public string titleNext { get; set; }
        public string descNext { get; set; }
        public string startNext { get; set; }
        public string stopNext { get; set; }


        public NowNextModel()
        {

        }

        public int progress()
        {
            DateTime start, stop;
            start = DateTime.ParseExact(startNow, "HH:mm", null);
            stop = DateTime.ParseExact(stopNow, "HH:mm", null);

            double total = (stop - start).TotalSeconds;
            double pass = (DateTime.Now - start).TotalSeconds;

            return (int)Math.Round((pass / total) * 100);
        }
    }
}

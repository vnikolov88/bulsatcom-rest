using onepoint.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace onepoint.Models.Home
{
    public class HomeModel
    {
        private string[] NOW_NEXT_CHANNEL_NAMES = { "FOX", "bTV Comedy" };

        public List<string> channels;
        public List<string> channels_epg;
        public List<string> radios;
        public List<NowNextModel> channels_now_next;

        public HomeModel()
        {
            channels = new List<string>();
            channels_epg = new List<string>();
            radios = new List<string>();
            channels_now_next = new List<NowNextModel>();
        }

        public void getLive()
        {
            XDocument doc = new XmlEpgHelper().loadXml(false);
            if (doc != null)
            {
                IEnumerable<XElement> e_tv = doc.Descendants("tv");
                IEnumerable<XElement> e_channel = e_tv.Elements("channel");
                foreach (XElement e_c in e_channel)
                {
                    if (e_c.Element("display-name") != null)
                    {
                        // channel
                        if (e_c.Element("radio") == null)
                        {
                            channels.Add(e_c.Element("display-name").Value);

                            // epg
                            IEnumerable<XElement> e_programme = from x in e_tv.Elements("programme") where x.Attribute("channel").Value.Equals(e_c.Attribute("id").Value) select x;
                            if (e_programme.Any())
                            {
                                channels_epg.Add(e_c.Element("display-name").Value);
                            }
                        }
                        // radio
                        else
                        {
                            radios.Add(e_c.Element("display-name").Value);
                        }
                    }
                }
            }
        }

        public void getNowNext()
        {
            XDocument doc = new XmlEpgHelper().loadXml(false);
            if (doc != null)
            {
                IEnumerable<XElement> e_root = doc.Descendants("tv");
                IEnumerable<XElement> e_channels = e_root.Elements("channel");
                foreach (XElement e_ch in e_channels)
                {
                    // get channel or radio
                    if (e_ch.Element("display-name") != null)
                    {
                        // channel
                        if (e_ch.Element("radio") == null)
                        {
                            channels.Add(e_ch.Element("display-name").Value);
                        }
                        // radio
                        else
                        {
                            radios.Add(e_ch.Element("display-name").Value);
                        }
                    }

                    // get epg
                    IEnumerable<XElement> e_channels_epg = from x in e_root.Elements("programme") where x.Attribute("channel").Value.Equals(e_ch.Attribute("id").Value) select x;
                    if (e_channels_epg.Any())
                    {
                        channels_epg.Add(e_ch.Element("display-name").Value);

                        // get now and next
                        if (NOW_NEXT_CHANNEL_NAMES.Contains(e_ch.Element("display-name").Value))
                        {
                            NowNextModel m = new NowNextModel();
                            m.channel = e_ch.Element("display-name").Value;

                            // now
                            IEnumerable<XElement> e_now = from x in e_channels_epg
                                                          where DateTime.ParseExact(x.Attribute("start").Value.Substring(0, 14), "yyyyMMddHHmmss", null) <= DateTime.Now
                                                          && DateTime.ParseExact(x.Attribute("stop").Value.Substring(0, 14), "yyyyMMddHHmmss", null) >= DateTime.Now
                                                          select x;
                            if (e_now.Any())
                            {
                                m.titleNow = e_now.First().Element("title").Value;
                                m.descNow = e_now.First().Element("desc").Value;
                                m.startNow = e_now.First().Attribute("start").Value.Substring(8, 2) + ":" + e_now.First().Attribute("start").Value.Substring(10, 2);
                                m.stopNow = e_now.First().Attribute("stop").Value.Substring(8, 2) + ":" + e_now.First().Attribute("stop").Value.Substring(10, 2);
                            }

                            // next
                            IEnumerable<XElement> e_next = from x in e_channels_epg where x.Attribute("start").Value == e_now.First().Attribute("stop").Value select x;
                            if (e_next.Any())
                            {
                                m.titleNext = e_next.First().Element("title").Value;
                                m.descNext = e_next.First().Element("desc").Value;
                                m.startNext = e_next.First().Attribute("start").Value.Substring(8, 2) + ":" + e_next.First().Attribute("start").Value.Substring(10, 2);
                                m.stopNext = e_next.First().Attribute("stop").Value.Substring(8, 2) + ":" + e_next.First().Attribute("stop").Value.Substring(10, 2);
                            }

                            if (m.titleNow.Length > 0)
                            {
                                channels_now_next.Add(m);
                            }
                        }
                    }
                }
            }

            /*
            XDocument doc = new XmlEpgHelper().loadXml();
            if (doc != null)
            {
                IEnumerable<XElement> e_root = doc.Descendants("tv");
                IEnumerable<XElement> e_channels = e_root.Elements("channel");
                foreach (XElement e_ch in e_channels)
                {
                    // get channel or radio
                    if (e_ch.Element("display-name") != null)
                    {
                        // channel
                        if (e_ch.Element("radio") == null)
                        {
                            channels.Add(e_ch.Element("display-name").Value);
                        }
                        // radio
                        else
                        {
                            radios.Add(e_ch.Element("display-name").Value);
                        }
                    }

                    // get epg
                    IEnumerable<XElement> e_channels_epg = from x in e_root.Elements("programme") where x.Attribute("channel").Value.Equals(e_ch.Attribute("id").Value) select x;
                    if (e_channels_epg.Any())
                    {
                        channels_epg.Add(e_ch.Element("display-name").Value);

                        // get now and next
                        if (NOW_NEXT_CHANNEL_NAMES.Contains(e_ch.Element("display-name").Value))
                        {
                            NowNextModel m = new NowNextModel();
                            m.channel = e_ch.Element("display-name").Value;

                            // now
                            IEnumerable<XElement> e_now = from x in e_channels_epg
                                                          where DateTime.ParseExact(x.Attribute("start").Value.Substring(0, 14), "yyyyMMddHHmmss", null) <= DateTime.Now
                                                          && DateTime.ParseExact(x.Attribute("stop").Value.Substring(0, 14), "yyyyMMddHHmmss", null) >= DateTime.Now
                                                          select x;
                            if (e_now.Any())
                            {
                                m.titleNow = e_now.First().Element("title").Value;
                                m.descNow = e_now.First().Element("desc").Value;
                                m.startNow = e_now.First().Attribute("start").Value.Substring(8, 2) + ":" + e_now.First().Attribute("start").Value.Substring(10, 2);
                                m.stopNow = e_now.First().Attribute("stop").Value.Substring(8, 2) + ":" + e_now.First().Attribute("stop").Value.Substring(10, 2);
                            }

                            // next
                            IEnumerable<XElement> e_next = from x in e_channels_epg where x.Attribute("start").Value == e_now.First().Attribute("stop").Value select x;
                            if (e_next.Any())
                            {
                                m.titleNext = e_next.First().Element("title").Value;
                                m.descNext = e_next.First().Element("desc").Value;
                                m.startNext = e_next.First().Attribute("start").Value.Substring(8, 2) + ":" + e_next.First().Attribute("start").Value.Substring(10, 2);
                                m.stopNext = e_next.First().Attribute("stop").Value.Substring(8, 2) + ":" + e_next.First().Attribute("stop").Value.Substring(10, 2);
                            }

                            if (m.titleNow.Length > 0)
                            {
                                channels_now_next.Add(m);
                            }
                        }
                    }
                }
            }
             */
        }
    }
}

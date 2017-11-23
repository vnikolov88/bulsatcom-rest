using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace onepoint.Models.Epg
{
    public class XmlHelper
    {
        public static XElement generateXml(List<ChannelModel> list)
        {
            // root element
            XElement tvElement = new XElement("tv", new XAttribute("date", (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds));

            // channels
            foreach (ChannelModel c in list)
            {
                XElement channelElement = new XElement("channel", new XAttribute("id", c.epg_name));
                channelElement.Add(new XElement("display-name", c.title));

                // add channel
                tvElement.Add(channelElement);

                // program
                foreach (EpgModel.Programme p in c.epg.programme)
                {
                    XElement programmeElement = new XElement("programme", new XAttribute("channel", c.epg_name), new XAttribute("start", p.start), new XAttribute("stop", p.stop));
                    programmeElement.Add(new XElement("title", p.title));
                    programmeElement.Add(new XElement("desc", p.desc));

                    // add program
                    tvElement.Add(programmeElement);
                }
            }

            return tvElement;
        }
    }
}

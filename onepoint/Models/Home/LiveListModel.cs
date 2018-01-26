using onepoint.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace onepoint.Models.Home
{
    public class LiveListModel
    {
        public List<string> channels { get; set; }
        public List<string> radions { get; set; }
        public List<string> epgs { get; set; }

        public LiveListModel()
        {
            loadLive();
        }

        private void loadLive()
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
                                channels.Add(e_c.Element("display-name").Value);
                            }
                        }
                        // radio
                        else
                        {
                            radions.Add(e_c.Element("display-name").Value);
                        }
                    }
                }
            }
        }
    }
}

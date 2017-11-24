using onepoint.Models.Channel;
using onepoint.Models.Epg;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;

namespace onepoint.Helpers
{
    public class XmlEpgHelper
    {
        private string filePath = @".\Resources\data";
        private string fileName = "epg";
        private string extXml = ".xml";
        private string extGz = ".gz";

        public XDocument doc { get; set; }


        public bool createXml(List<ChannelModel> list)
        {
            if (list.Count > 0)
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);

                // root element
                XElement tvElement = new XElement("tv", new XAttribute("date", t.TotalSeconds));

                // channels
                foreach (ChannelModel c in list)
                {
                    XElement channelElement = new XElement("channel", new XAttribute("id", c.epg_name));
                    channelElement.Add(new XElement("display-name", c.title));

                    // add channel
                    tvElement.Add(channelElement);

                    // program
                    if (c.epg != null && c.epg.programme != null && c.epg.programme.Count > 0)
                    {
                        foreach (EpgModel.Programme p in c.epg.programme)
                        {
                            XElement programmeElement = new XElement("programme", new XAttribute("channel", c.epg_name), new XAttribute("start", p.start), new XAttribute("stop", p.stop));
                            programmeElement.Add(new XElement("title", p.title));
                            programmeElement.Add(new XElement("desc", p.desc));

                            // add program
                            tvElement.Add(programmeElement);
                        }
                    }
                }

                // create
                doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), tvElement);
                // save
                saveXml();
                saveZip();
                return true;
            }

            return false;
        }


        /// <summary>
        /// 
        /// XML
        /// 
        /// </summary>
        /// <returns></returns>


        public bool saveXml()
        {
            if (doc != null)
            {
                doc.Save(filePath + Path.DirectorySeparatorChar + fileName + extXml);
                return true;
            }

            return false;
        }



        public bool loadXml()
        {
            if (File.Exists(filePath + Path.DirectorySeparatorChar + fileName + extXml))
            {
                XDocument doc = XDocument.Load(filePath + Path.DirectorySeparatorChar + fileName + extXml);
                if (doc != null)
                {
                    // check if file is to old
                    foreach (XNode node in doc.DescendantNodes())
                    {
                        if (node is XElement)
                        {
                            XElement element = (XElement)node;
                            if (element.Name.LocalName.Equals("tv"))
                            {
                                XAttribute att = element.Attribute("date");
                                string date = (string)att;

                                if (date != null && date.Length > 0)
                                {
                                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                                    double secondsSinceEpoch = t.TotalSeconds;

                                    double secondsSinceEpochFileCreate = double.Parse(date, System.Globalization.CultureInfo.InvariantCulture);

                                    // file is not older then 5 days, so we can use it
                                    // as we have data for 7 days inside
                                    if (secondsSinceEpoch - secondsSinceEpochFileCreate < 60 * 60 * 24 * 5)
                                    {
                                        this.doc = doc;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// GZip
        /// 
        /// </summary>
        /// <returns></returns>


        public bool saveZip()
        {
            if (File.Exists(filePath + Path.DirectorySeparatorChar + fileName + extXml) == true)
            {
                FileStream file = File.OpenRead(filePath + Path.DirectorySeparatorChar + fileName + extXml);
                FileStream fileGz = File.Create(filePath + Path.DirectorySeparatorChar + fileName + extXml + extGz);

                byte[] buffer = new byte[file.Length];
                file.Read(buffer, 0, buffer.Length);

                using (GZipStream output = new GZipStream(fileGz, CompressionMode.Compress))
                {
                    output.Write(buffer, 0, buffer.Length);
                }

                // Close the files.
                file.Close();
                fileGz.Close();

                return true;
            }

            return false;
        }


        public FileStream getZip()
        {
            if (File.Exists(filePath + Path.DirectorySeparatorChar + fileName + extXml + extGz) == true)
            {
                return File.OpenRead(filePath + Path.DirectorySeparatorChar + fileName + extXml + extGz);
            }

            return null;
        }
    }
}

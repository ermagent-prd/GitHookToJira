using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TCALauncher.HistoryProcess
{
    internal class HTMLAggregationInfoExport : IAggregationInfoExport
    {
        #region Private fields

        private readonly string fileName;

        #endregion

        #region Constructor

        public HTMLAggregationInfoExport(string fileName)
        {
            this.fileName = fileName;
        }

        #endregion

        public void Export(IEnumerable<AggregationInfo> aggregationInfos)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement xmlRoot = xmlDoc.CreateElement("html");
            xmlDoc.AppendChild(xmlRoot);

            XmlElement xmlHead = xmlDoc.CreateElement("head");
            xmlRoot.AppendChild(xmlHead);

            XmlElement xmlbody = xmlDoc.CreateElement("body");

            foreach (AggregationInfo aggregationInfo in aggregationInfos)
                xmlbody.AppendChild(BuildElement(xmlDoc, aggregationInfo));

            xmlRoot.AppendChild(xmlbody);

            xmlDoc.Save(new StreamWriter(fileName, false));
        }

        private XmlElement BuildElement(XmlDocument xmlDoc, AggregationInfo aggregationInfo)
        {
            XmlElement xmlDiv = xmlDoc.CreateElement("div");

            XmlElement xmlParagraph1 = xmlDoc.CreateElement("h3");
            xmlParagraph1.InnerText = $"{aggregationInfo.Caption} {aggregationInfo.Value}/{aggregationInfo.Total}";
            xmlDiv.AppendChild(xmlParagraph1);
            XmlElement xmlParagraph2 = xmlDoc.CreateElement("p");
            xmlParagraph2.InnerText = $"{string.Join(", ",aggregationInfo.IDs)}";
            xmlDiv.AppendChild(xmlParagraph2);

            return xmlDiv;
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TCALauncher.HistoryProcess
{
    internal class HTMLHistoryExportBuilder : IHistoryExportBuilder
    {
        #region Private fields

        private XmlDocument xmlDoc;
        private string fileName;
        private List<XmlElement> elements;

        #endregion

        #region Constructor

        public HTMLHistoryExportBuilder(string fileName)
        {
            xmlDoc = new XmlDocument();
            this.fileName = fileName;
            elements = new List<XmlElement>();

        }

        #endregion


        #region Interface implementation

        public void BuildHistory(IProcessHistory history)
        {
            XmlElement hElement = BuildTitle(history.ProcessId);

            XmlElement listElement = xmlDoc.CreateElement("ol");

            foreach (ProcessPhase phase in history.Phases)
            {
                XmlElement bulletElement = xmlDoc.CreateElement("li");
                bulletElement.AppendChild(BuildLine(phase));
                listElement.AppendChild(bulletElement);
            }

            hElement.AppendChild(listElement);

            elements.Add(hElement);
        }
        
        public void Build()
        {
            XmlElement xmlRoot = xmlDoc.CreateElement("html");
            xmlDoc.AppendChild(xmlRoot);

            XmlElement xmlHead = xmlDoc.CreateElement("head");
            xmlRoot.AppendChild(xmlHead);

            XmlElement xmlbody = xmlDoc.CreateElement("body");
            foreach (XmlElement element in elements)
                xmlbody.AppendChild(element);

            xmlRoot.AppendChild(xmlbody);

            xmlDoc.Save(new StreamWriter(fileName, false));
        }

        #endregion

        #region Private methods

        private XmlElement BuildTitle(string title)
        {
            XmlElement xmlDiv = xmlDoc.CreateElement("div");
            XmlElement xmlHeader = xmlDoc.CreateElement("h3");
            xmlHeader.InnerText = title;
            xmlDiv.AppendChild(xmlHeader);

            return xmlDiv;
        }

        private XmlElement BuildLine(IProcessPhase processPhase)
        {
            XmlElement xmlDiv = xmlDoc.CreateElement("div");
            XmlElement xmlParagraph = xmlDoc.CreateElement("p");

            if (!processPhase.PhasePassed)
            {
                XmlAttribute styleAttribute = xmlDoc.CreateAttribute("style");
                styleAttribute.Value = "color:red";
                xmlParagraph.Attributes.Append(styleAttribute);
            }

            xmlParagraph.InnerText = processPhase.ToString();
            xmlDiv.AppendChild(xmlParagraph);

            return xmlDiv;
        }

        #endregion
    }
}

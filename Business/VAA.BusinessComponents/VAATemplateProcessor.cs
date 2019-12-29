using System;
using System.Collections.Generic;
using System.Xml;
using VAA.BusinessComponents.Interfaces;

namespace VAA.BusinessComponents
{
    /// <summary>
    /// Chili template processor - variable show/hide
    /// </summary>
    public class VAATemplateProcessor : ITemplateProcessor
    {

        public void ProcessDocument(XmlDocument document, Dictionary<string, string> variables)
        {

        }

        private void SetLayerVisibility(XmlDocument xmlDoc, string layerName, bool visible)
        {
            try
            {
                var xmlLayerNode = xmlDoc.SelectSingleNode("//layers/item[@name='" + layerName + "']");
                if (xmlLayerNode == null) throw new Exception("Unable to find layer " + layerName);
                if (xmlLayerNode.Attributes != null)
                {
                    var visAttr = xmlLayerNode.Attributes["visible"];
                    if (visAttr == null)
                    {
                        visAttr = xmlDoc.CreateAttribute("visible");
                        xmlLayerNode.Attributes.Append(visAttr);
                    }
                    visAttr.InnerText = visible.ToString().ToLower();
                }
            }
            catch (Exception ex)
            {
                //ignore this as layer might not be available
            }
        }

    }

}

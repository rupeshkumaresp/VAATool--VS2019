using System.Collections.Generic;
using System.Xml;
using VAA.DataAccess.Model;

namespace VAA.BusinessComponents.Interfaces
{
    interface ITemplateProcessor
    {
        void ProcessDocument( XmlDocument document, Dictionary<string, string> variables);
    }
}
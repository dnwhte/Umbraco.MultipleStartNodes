using MultipleStartNodes.Models;
using MultipleStartNodes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.Logging;

namespace MultipleStartNodes.PackageActions
{
    class CreateDatabase : IPackageAction
    {
        public string Alias()
        {
            return "MultipleStartNodes_CreateDatabase";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                Resources.DatabaseSchemaHelper.CreateTable<UserStartNodes>(false);

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(CreateDatabase), message, ex);
            }

            return false;
            
        }

        public XmlNode SampleXml()
        {
            var xml = string.Concat("<Action runat=\"install\" undo=\"true\" alias=\"", this.Alias(), "\" />");
            return helper.parseStringToXmlNode(xml);
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {            
            try
            {
                Resources.DatabaseSchemaHelper.DropTable<UserStartNodes>();

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(CreateDatabase), message, ex);
            }

            return false;
        }
    }
}

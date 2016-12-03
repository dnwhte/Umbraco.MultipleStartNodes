using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace MultipleStartNodes.PackageActions
{
    public class AddConfigSection : IPackageAction
    {
        public string Alias()
        {
            return "MultipleStartNodes_AddConfigSection";
        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            try
            {
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections["MultipleStartNodes"] == null)
                {
                    webConfig.Sections.Add("MultipleStartNodes", new MultipleStartNodesConfig());

                    var configPath = string.Concat("Config", Path.DirectorySeparatorChar, "MultipleStartNodes.config");
                    var xmlPath = IOHelper.MapPath(string.Concat("~/", configPath));

                    string xml;

                    using (var reader = new StreamReader(xmlPath))
                    {
                        xml = reader.ReadToEnd();
                    }                    
                    
                    webConfig.Sections["MultipleStartNodes"].SectionInformation.ConfigSource = configPath;
                    webConfig.Sections["MultipleStartNodes"].SectionInformation.RequirePermission = false;
                    webConfig.Save(ConfigurationSaveMode.Minimal);

                    // TODO - this is a quick fix for M-149
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        writer.Write(xml);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at install ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(AddConfigSection), message, ex);
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
                var webConfig = WebConfigurationManager.OpenWebConfiguration("~/");
                if (webConfig.Sections["MultipleStartNodes"] != null)
                {
                    webConfig.Sections.Remove("MultipleStartNodes");

                    webConfig.Save(ConfigurationSaveMode.Modified);
                }

                return true;
            }
            catch (Exception ex)
            {
                var message = string.Concat("Error at undo ", this.Alias(), " package action: ", ex);
                LogHelper.Error(typeof(AddConfigSection), message, ex);
            }

            return false;
        }
    }
}

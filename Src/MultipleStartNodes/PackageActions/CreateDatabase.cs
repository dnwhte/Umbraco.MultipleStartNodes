using MultipleStartNodes.Helpers;
using MultipleStartNodes.Models;
using System;
using System.Xml;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

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
                ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
                DatabaseSchemaHelper databaseSchemaHelper = new DatabaseSchemaHelper(appContext.DatabaseContext.Database, ContextHelpers.EnsureApplicationContext().ProfilingLogger.Logger, appContext.DatabaseContext.SqlSyntax);

                if (!databaseSchemaHelper.TableExist("userStartNodes"))
                {
                    databaseSchemaHelper.CreateTable<UserStartNodes>(false);
                }

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
                ApplicationContext appContext = ContextHelpers.EnsureApplicationContext();
                DatabaseSchemaHelper databaseSchemaHelper = new DatabaseSchemaHelper(appContext.DatabaseContext.Database, ContextHelpers.EnsureApplicationContext().ProfilingLogger.Logger, appContext.DatabaseContext.SqlSyntax);
                
                databaseSchemaHelper.DropTable<UserStartNodes>();

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

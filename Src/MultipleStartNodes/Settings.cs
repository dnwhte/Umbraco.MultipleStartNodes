using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultipleStartNodes
{
    public static class Settings
    {
        static MultipleStartNodesConfig multipleStartNodesConfig = MultipleStartNodesConfig.GetConfig();        

        public static bool LimitPickersToStartNodes
        {
            get
            {               
                return multipleStartNodesConfig.LimitPickersToStartNodes.Value;
            }
        }

        public static string[] RemoveActionsForStartNodes
        {
            get
            {
                return multipleStartNodesConfig.RemoveActionsForStartNodes.Value.Split(',');
            }
        }
    }


    public class MultipleStartNodesConfig : ConfigurationSection
    {
        public static MultipleStartNodesConfig GetConfig()
        {
            return ConfigurationManager.GetSection("MultipleStartNodes") as MultipleStartNodesConfig;
        }

        [ConfigurationProperty("LimitPickersToStartNodes")]
        public LimitPickersToStartNodes LimitPickersToStartNodes
        {
            get
            {
                return (LimitPickersToStartNodes)this["LimitPickersToStartNodes"];
            }
            set
            { 
                this["LimitPickersToStartNodes"] = value; 
            }
        }

        [ConfigurationProperty("RemoveActionsForStartNodes")]
        public RemoveActionsForStartNodes RemoveActionsForStartNodes
        {
            get
            {
                return (RemoveActionsForStartNodes)this["RemoveActionsForStartNodes"];
            }
            set
            {
                this["RemoveActionsForStartNodes"] = value;
            }
        }
    }


    public class LimitPickersToStartNodes : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = false, IsRequired = true)]
        public bool Value
        {
            get
            {
                return bool.Parse(this["value"].ToString());
            }
        }
    }

    public class RemoveActionsForStartNodes : ConfigurationElement
    {
        [ConfigurationProperty("value", DefaultValue = "", IsRequired = false)]
        public string Value
        {
            get
            {
                return this["value"].ToString();
            }
        }
    }

}
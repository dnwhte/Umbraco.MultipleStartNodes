using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;


namespace MultipleStartNodes.Controllers
{
    [PluginController("MultipleStartNodes")]
    public class SettingsApiController : UmbracoAuthorizedJsonController
    {
        [HttpGet]
        public bool LimitPickersToStartNodes()
        {
            return Settings.LimitPickersToStartNodes;
        }
    }
}

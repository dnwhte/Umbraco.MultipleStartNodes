using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Umbraco.Web.Security;

namespace MultipleStartNodes.Helpers
{
    public class ContextHelpers
    {
        public static ApplicationContext EnsureApplicationContext()
        {
            if (ApplicationContext.Current != null)
            {
                return ApplicationContext.Current;
            }

            return ApplicationContext.EnsureContext(
                ApplicationContext.Current, 
                false
            );
        }

        public static UmbracoContext EnsureUmbracoContext()
        {
            if (UmbracoContext.Current != null)
            {
                return UmbracoContext.Current;
            }

            var dummyHttpContext = new HttpContextWrapper(new HttpContext(new SimpleWorkerRequest("dummy.aspx", "", new StringWriter())));

            return UmbracoContext.EnsureContext(
                dummyHttpContext,
                EnsureApplicationContext(),
                new WebSecurity(dummyHttpContext, ApplicationContext.Current),
                UmbracoConfig.For.UmbracoSettings(),
                UrlProviderResolver.Current.Providers,
                false);
        }
    }
}

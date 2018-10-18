using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Web;
using X.Web.Apis;
using X.Web.Com;

namespace X.App.Apis
{
    public class xapi : Api
    {
        protected Config cfg = Config.LoadConfig();

        protected DBDataContext db = new DBDataContext(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString) { DeferredLoadingEnabled = true };
    }
}

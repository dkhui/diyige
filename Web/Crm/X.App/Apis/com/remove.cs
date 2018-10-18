using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using X.Web.Com;

namespace X.App.Apis.com
{
    public class remove : xapi
    {
        [ParmsAttr(name = "文件名", req = true)]
        public string file { get; set; }
        protected override XResp Execute()
        {
            file = Context.Server.MapPath(file.Replace("http://" + cfg.domain, ""));
            try
            {
                if (File.Exists(file)) File.Delete(file);
            }
            catch { }
            return new XResp();
        }
    }
}

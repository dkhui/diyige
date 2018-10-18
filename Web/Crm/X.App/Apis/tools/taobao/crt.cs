using System;
using System.Diagnostics;
using X.Web.Com;

namespace X.App.Apis.tools.taobao
{
    public class crt : xapi
    {
        public string no { get; set; }
        public string tp { get; set; }
        protected override XResp Execute()
        {
            var n = Guid.NewGuid().ToString();

            var par = new ProcessStartInfo(Context.Server.MapPath("/bin/phantomjs.exe"));
            par.Arguments = Context.Server.MapPath("/bin/ph.js") + " http://" + Context.Request.Url.Authority + "/tools/taobao-" + no + "-" + tp + "-1.html " + Context.Server.MapPath("/temp/" + n + ".png");
            par.CreateNoWindow = true;
            par.UseShellExecute = true;
            par.WindowStyle = ProcessWindowStyle.Hidden;
            var pr = new Process();
            pr.StartInfo = par;
            pr.Start();
            pr.WaitForExit();

            return new XResp() { msg = n };
        }
    }
}

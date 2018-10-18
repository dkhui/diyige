using X.Web.Com;

namespace X.App.Views.com
{
    public class err : xview
    {
        public string code { get; set; }
        protected override string GetParmNames => "code";
        protected override void InitView()
        {
            base.InitView();
            if (code == "404")
            {
                Context.Response.StatusCode = 404;
                dict.Add("rsp", new XResp() { code = code, msg = "找不到页面" });
            }
        }
    }
}

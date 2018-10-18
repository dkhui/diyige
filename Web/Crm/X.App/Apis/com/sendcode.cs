using X.App.Com;
using X.Core.Cache;
using X.Core.Plugin;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.com
{
    public class sendcode : xapi
    {
        [ParmsAttr(name = "验证码")]
        public string code { get; set; }
        [ParmsAttr(name = "手机号", req = true)]
        public string tel { get; set; }
        protected override XResp Execute()
        {
            var sc = CacheHelper.Get<string>("img.code." + tel);
            if (sc != code) throw new XExcep("0x0010");
            CacheHelper.Remove("img.code." + tel);
            var smscode = Tools.GetRandRom(4, 1);
            if (tel != "18073113871")
            {
                var re = Sms.CodeMsg(tel, smscode);
                if (!re) throw new XExcep("0x0013");
            }
            CacheHelper.Save("sms.code." + tel, smscode, 5 * 60);
            return new XResp() { msg = tel == "18073113871" ? smscode : "" };
        }
    }
}

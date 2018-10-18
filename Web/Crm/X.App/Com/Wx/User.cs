using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Plugin;
using X.Core.Utility;

namespace X.App.Com.Wx
{
    /// <summary>
    /// 用户相关
    /// </summary>
    public class User
    {
        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <param name="opid"></param>
        /// <param name="tk"></param>
        /// <param name="isweb"></param>
        /// <returns></returns>
        public uinfo GetUserInfo(string opid, string tk, bool isweb)
        {
            var json = "";
            if (isweb) json = Tools.GetHttpData("https://api.weixin.qq.com/sns/userinfo?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
            else json = Tools.GetHttpData("https://api.weixin.qq.com/cgi-bin/user/info?access_token=" + tk + "&openid=" + opid + "&lang=zh_CN");
            var ui = Serialize.FromJson<uinfo>(json);
            if (!string.IsNullOrEmpty(ui.errcode)) Loger.Error(json);
            return ui;
        }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <param name="opid"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        public uinfo GetUserInfo(string opid, string tk)
        {
            return GetUserInfo(opid, tk, false);
        }
        /// <summary>
        /// 批量或取用户信息
        /// </summary>
        /// <param name="pus"></param>
        /// <param name="tk"></param>
        /// <returns></returns>
        public users GetMutiUserInfo(List<object> pus, string tk)
        {
            var url = "https://api.weixin.qq.com/cgi-bin/user/info/batchget?access_token=" + tk;
            var json = Tools.PostHttpData(url, Serialize.ToJson(new { user_list = pus }));
            Loger.Info("getmutiuserinfo->" + Serialize.ToJson(new { user_list = pus }));
            var us = Serialize.FromJson<users>(json);
            if (!string.IsNullOrEmpty(us.errcode)) return null;
            return us;
        }

        public class users : Basic.mbase
        {
            public List<uinfo> user_info_list { get; set; }
        }

        public class uinfo : Basic.mbase
        {
            /// <summary>
            /// 用户是否订阅该公众号标识，值为0时，代表此用户没有关注该公众号，拉取不到其余信息。
            /// </summary>
            public int subscribe { get; set; }
            /// <summary>
            /// 用户的标识，对当前公众号唯一
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户的昵称
            /// </summary>
            public string nickname { get; set; }
            /// <summary>
            /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
            /// </summary>
            public int sex { get; set; }
            /// <summary>
            /// 用户所在城市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 用户所在国家
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 用户所在省份
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 用户的语言，简体中文为zh_CN
            /// </summary>
            public string language { get; set; }
            /// <summary>
            /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像），用户没有头像时该项为空。若用户更换头像，原有头像URL将失效。
            /// </summary>
            public string headimgurl { get; set; }
            /// <summary>
            /// 用户关注时间，为时间戳。如果用户曾多次关注，则取最后关注时间
            /// </summary>
            public string subscribe_time { get; set; }
            /// <summary>
            /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。详见：获取用户个人信息（UnionID机制）
            /// </summary>
            public string unionid { get; set; }
            /// <summary>
            /// 公众号运营者对粉丝的备注，公众号运营者可在微信公众平台用户管理界面对粉丝添加备注
            /// </summary>
            public string remark { get; set; }
            /// <summary>
            /// 用户所在的分组ID
            /// </summary>
            public int groupid { get; set; }
        }
    }
}

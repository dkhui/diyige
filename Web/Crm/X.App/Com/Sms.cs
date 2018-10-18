using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using X.Core.Plugin;
using X.Core.Utility;
using X.Web;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Dysmsapi.Model.V20170525;
using Aliyun.Acs.Core.Http;

namespace X.App.Com
{
    public class Sms
    {
        static Config.smscfg cfg = Config.LoadConfig().sms;

        public static bool CodeMsg(string to, string code)
        {
            return Send(new Dictionary<string, string>()
            {
                { "to",to},
                { "code",code},
                { "tpcode","sendcode"}
            });
        }

        public static bool SendPackage(string to, string did)
        {
            return Send(new Dictionary<string, string>()
            {
                { "to",to},
                { "did",did},
                { "tpcode","drawreply"}
            });
        }
        public static bool DrawReply(string to, string did)
        {
            return Send(new Dictionary<string, string>()
            {
                { "to",to},
                { "did",did},
                { "tpcode","drawreply"}
            });
        }
        public static bool PaySucc(string to, string oid)
        {
            return Send(new Dictionary<string, string>()
            {
                { "to",to},
                { "oid",oid},
                { "tpcode","paysucc"}
            });
        }

        protected static bool Send(Dictionary<string, string> ps)
        {
            var tpl = cfg.tpls.FirstOrDefault(o => o.code == ps["tpcode"]);
            if (tpl == null) { throw new XExcep("找不到发送模板：" + ps["tpcode"]); }
            if (cfg.way == 1)
            {
                var pf = DefaultProfile.GetProfile("cn-hangzhou", cfg.ali.appid, cfg.ali.secret);

                DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "Dysmsapi", cfg.ali.endpoint);
                var ac = new DefaultAcsClient(pf);
                var req = new SendSmsRequest();
                try
                {
                    //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                    req.PhoneNumbers = ps["to"];
                    //必填:短信签名-可在短信控制台中找到
                    req.SignName = cfg.ali.sign;
                    //必填:短信模板-可在短信控制台中找到
                    req.TemplateCode = tpl.alicode;
                    //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                    var tp = new StringBuilder("{");
                    foreach (var c in ps.Where(o => o.Key != "to" && o.Key != "tpcode")) tp.AppendFormat("\"{0}\":\"{1}\"", c.Key, c.Value);
                    tp.Append("}");
                    req.TemplateParam = tp.ToString();

                    var rsp = ac.GetAcsResponse(req);
                    if (rsp.Message != "OK") { Loger.Error("短信发送失败，错误信息：" + rsp.Message); return false; }
                    return true;
                }
                catch (Exception e)
                {
                    Loger.Error("短信发送失败，错误信息：" + e.Message);
                    throw new XExcep(e.Message);
                }

                //var pf = DefaultProfile.GetProfile("cn-hangzhou", cfg.ali.appid, cfg.ali.secret);
                //DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "Dysmsapi", cfg.ali.endpoint);
                //var ac = new DefaultAcsClient(pf);
                //var req = new SendSmsRequest();
                //req.AcceptFormat = FormatType.JSON;
                //req.Method = MethodType.POST; //(MethodType.POST);
                //try
                //{
                //    //必填:待发送手机号。支持以逗号分隔的形式进行批量调用，批量上限为20个手机号码,批量调用相对于单条调用及时性稍有延迟,验证码类型的短信推荐使用单条调用的方式
                //    req.PhoneNumbers = ps["to"];
                //    //必填:短信签名-可在短信控制台中找到
                //    req.SignName = cfg.ali.sign;
                //    //必填:短信模板-可在短信控制台中找到
                //    req.TemplateCode = tpl.alicode;
                //    //可选:模板中的变量替换JSON串,如模板内容为"亲爱的${name},您的验证码为${code}"时,此处的值为
                //    var tp = new StringBuilder("{");
                //    foreach (var c in ps.Where(o => o.Key != "to" && o.Key != "tpcode")) tp.AppendFormat("\"{0}\":\"{1}\"", c.Key, c.Value);
                //    tp.Append("}");
                //    req.TemplateParam = tp.ToString();
                //    //可选:outId为提供给业务方扩展字段,最终在短信回执消息中将此值带回给调用者
                //    //req.OutId = "21212121211";
                //    //请求失败这里会抛ClientException异常
                //    var rsp = ac.GetAcsResponse(req);
                //    return rsp.Message == "OK";
                //}
                //catch (Exception ex)
                //{
                //    Loger.Error("短信发送失败，错误信息：" + ex.Message);
                //    throw new XExcep(ex.Message);
                //}
                #region 消息服务方式
                //var req = new PublishMessageRequest() { MessageBody = "fuck you" };
                //req.MessageAttributes = new MessageAttributes()
                //{
                //    SmsAttributes = new SmsAttributes()
                //    {
                //        Receiver = ps["to"],
                //        TemplateCode = tpl.alicode,
                //        FreeSignName = cfg.ali.sign,
                //        SmsParams = ps.Where(o => o.Key != "to" && o.Key != "tpcode").ToDictionary(k => k.Key, v => v.Value)
                //    }
                //};
                //try
                //{
                //    var client = new MNSClient(cfg.ali.appid, cfg.ali.secret, cfg.ali.endpoint);
                //    var tp = client.GetNativeTopic(cfg.ali.topic);
                //    var resp = tp.PublishMessage(req);
                //    return !string.IsNullOrEmpty(resp.MessageId);
                //}
                //catch (Exception ex)
                //{
                //    Loger.Error("短信发送失败，错误信息：" + ex.Message);
                //    throw new XExcep(ex.Message);
                //}
                #endregion
            }
            else if (cfg.way == 2)
            {
                var cot = tpl.content;
                foreach (var p in ps.Keys) { if (p == "to") continue; cot = cot.Replace("{$" + p + "}", ps[p]); }
                var mb = new
                {
                    account = cfg.clan.account,
                    password = cfg.clan.password,
                    msg = HttpUtility.UrlEncode(cot + cfg.clan.sign),
                    phone = ps["to"]
                };
                try
                {
                    var json = Tools.PostHttpData(cfg.clan.gateway, Serialize.ToJson(mb));
                    var rt = Serialize.JsonToDict(json);
                    if (rt["code"] != null) Loger.Error("短信发送失败，错误信息：" + json);
                    return rt["code"] == "0";
                }
                catch (Exception ex)
                {
                    Loger.Error("短信发送失败，错误信息：" + ex.Message);
                    return false;
                }
            }
            else throw new Exception("不支持的短信平台");
        }
    }
}

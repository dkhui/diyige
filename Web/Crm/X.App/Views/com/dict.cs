using System;
using System.Collections.Generic;
using System.Linq;
using X.App.Com;
using X.Web.Views;

namespace X.App.Views.com
{
    public class dict : xview
    {
        [ParmsAttr(name = "代号", req = true)]
        public string code { get; set; }
        public string upv { get; set; }
        public int bylet { get; set; }

        protected override void InitDict()
        {
            base.InitDict();
            if (dict.GetInt("bylet") == 1)
            {
                dict.Add("list", "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList());
            }
            else
            {
                var ds = GetDataList(code, string.IsNullOrEmpty(upv) ? "00" : upv);
                if (!ds.ContainsKey("0")) ds.Add("0", "请选择");
                var list = ds.OrderBy(o => o.Key).Select(o => new { name = o.Value, value = o.Key }).ToList();
                dict.Add("dict", list);
            }
        }

        protected override string GetParmNames
        {
            get
            {
                ///代号-上级值-按字母排
                return "code-upv-bylet";
            }
        }

        
    }
}

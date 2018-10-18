using System;
using System.Collections.Generic;
using System.Linq;
using X.Core.Utility;
using X.Web.Views;

namespace X.App.Views.com
{
    public class view : xview
    {
        /// <summary>
        /// 代号
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 搜索
        /// </summary>
        public string key { get; set; }

        protected override void InitDict()
        {
            base.InitDict();
            dict.Add("dict", GetViewList(code, key));
        }

        protected override string GetParmNames
        {
            get { return "code-key"; }
        }

        public override string GetTplFile()
        {
            return "com/dict";
        }
    }
}

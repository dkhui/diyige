using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using X.App.Com;
using X.Core.Cache;
using X.Web.Views;

namespace X.App.Views
{
    public class xview : View 
    {
        /// <summary>
        /// 系统配置文件
        /// </summary>
        protected Config cfg = null;
        protected DBDataContext db = null;

        protected override void InitView()
        {
            base.InitView();
            cfg = Config.LoadConfig();
            dict.Add("cfg", cfg);
            db = new DBDataContext(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString) { DeferredLoadingEnabled = true };
        }

        protected override Dictionary<string, string> GetDataList(string cd, string up)
        {
            var list = CacheHelper.Get<Dictionary<string, string>>(cd);
            if (list == null || list.Count == 0)
            {
                if (cd.StartsWith("view."))
                {
                    switch (cd)
                    {
                        case "view.app.goods":
                            list = db.gt_app_goods.Where(o => o.app_id == long.Parse(up)).ToDictionary(k => k.app_goods_id + "", v => v.name);
                            break;
                        case "view.market":
                            list = db.gt_market.ToDictionary(k => k.market_id + "", v => v.name);
                            break;
                        case "view.app":
                            list = db.gt_app.ToDictionary(k => k.app_id + "", v => v.name);
                            break;
                    }
                    CacheHelper.Save(cd, list);
                }
                else
                {
                    list = db.GetDictList(cd, up)?.ToDictionary(k => k.value, v => v.name);
                }
            }
            if (list == null) list = new Dictionary<string, string>();
            return list;
        }
    }
}

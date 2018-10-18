using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.proj
{
    public class updt : xmg
    {
        [ParmsAttr(name = "项目ID", min = 1)]
        public int id { get; set; }
        public int frame { get; set; }
        public int style { get; set; }
        public int layct { get; set; }
        public decimal layht { get; set; }
        public decimal open { get; set; }
        public decimal depth { get; set; }
        public decimal zarea { get; set; }
        public decimal jarea { get; set; }
        public string cost { get; set; }
        public string remark { get; set; }
        public string demand { get; set; }
        public string opt { get; set; }
        public decimal @long { get; set; }
        public decimal width { get; set; }
        public string rec_man { get; set; }
        //public string rec_shen { get; set; }
        //public string rec_shi { get; set; }
        //public string rec_xian { get; set; }
        public string rec_addr { get; set; }
        //public string bud_shen { get; set; }
        //public string bud_shi { get; set; }
        //public string bud_xian { get; set; }
        public string bud_addr { get; set; }
        public string laydesc { get; set; }
        public string imgs { get; set; }
        public string attr { get; set; }
        public DateTime ctime { get; set; }
        protected override string PowerCode => "E010207";
        protected override XResp Execute()
        {
            var p = db.x_project.FirstOrDefault(o => o.project_id == id);
            if (p == null) throw new XExcep("0x0039");

            if (p.status > 1) throw new XExcep("T项目当前状态不修改需求信息！！！");

            var od = db.x_order.FirstOrDefault(o => o.pid == id);
            if (od == null) throw new XExcep("0x0040");

            if (p.status == 0) { p.status = 1; p.ap_id = mg.mgr_id; p.ap_man = mg.name; }

            p.frame = frame;
            p.style = style;
            p.layct = layct;
            p.layht = layht;
            p.laydesc = laydesc;
            p.demand = demand;
            p.open = open;
            p.depth = depth;
            p.@long = @long;
            p.width = width;
            p.zarea = zarea;
            p.jarea = jarea;
            p.cost = cost;
            p.remark = remark;
            p.opt = opt;
            p.refer = imgs;
            p.attr = attr;

            if (ctime != null) p.ctime = ctime;

            p.bud_addr = /*bud_shen + "|" + bud_shi + "|" + bud_xian + "|" +*/ bud_addr;
            p.rec_addr = /*rec_shen + "|" + rec_shi + "|" + rec_xian + "|" +*/ rec_addr;

            var tp = "开间：" + p.open + "米，进深：" + p.depth + "米，建面：" + p.jarea + "平，占面：" + p.zarea + "平，" + p.layct + "层，层高：" + p.layht + "米，预算：" + p.cost + "万，风格：" + db.GetDictName("draw.style", p.style) + "，框架：" + db.GetDictName("draw.frame", p.frame) + "，朝向：" + p.opt + "，要求：" + db.GetDictName("draw.attr", attr);

            var lg = new x_project_log()
            {
                cot = string.IsNullOrEmpty(od.topic) ? "填写了项目需求：" + tp : "修改了项目需求，原需求：" + od.topic + "，新需求：" + tp,
                ctime = DateTime.Now,
                mgr_id = mg.mgr_id,
                type = 1
            };

            p.x_project_log.Add(lg);

            if (!string.IsNullOrEmpty(imgs)) od.cover = imgs.Split(',')[0];
            od.topic = tp;
            if (!string.IsNullOrEmpty(rec_man)) od.rec_man = rec_man;
            od.rec_addr = rec_addr;
            od.user_remark = remark;

            db.SubmitChanges();

            return new XResp();
        }
    }
}

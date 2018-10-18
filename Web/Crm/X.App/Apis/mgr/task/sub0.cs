using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.App.Com;
using X.Core.Utility;
using X.Web;
using X.Web.Com;

namespace X.App.Apis.pc.task
{
    public class sub0 : sub
    {
        protected override void Submit()
        {
            switch (t.type)
            {
                case 3:
                    dr.pbuild = cot;
                    break;
                case 4:
                    dr.pstruct = cot;
                    break;
                case 5:
                    dr.pdrain = cot;
                    break;
                case 6:
                    dr.peleric = cot;
                    break;
            }
        }
    }
}

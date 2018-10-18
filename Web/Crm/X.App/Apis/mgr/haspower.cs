using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace X.App.Apis.pc
{
    public class haspower : xmg
    {
        public string code { get; set; }
        protected override string PowerCode => code;
    }
}

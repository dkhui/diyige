using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using X.Core.Utility;

namespace X.App.Com.Wx
{

    /// <summary>
    /// 微信异常
    /// </summary>
    public class WxExcep : Exception
    {
        /// <summary>
        /// Initializes a new instance of the WxExcep class.
        /// </summary>
        public WxExcep(string msg)
            : base(msg)
        {
            if (msg[0] == '{') error = Serialize.FromJson<Err>(msg);
            else error = new Err(msg);
        }

        public Err error { get; set; }

        public class Err
        {
            public string errcode { get; set; }
            public string errmsg { get; set; }
            /// <summary>
            /// Initializes a new instance of the Err class.
            /// </summary>
            public Err(string msg)
            {
                errmsg = msg;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Error
    {
        public string StatusCode;
        public string SourceAddress;
        public string SourceURL;
        public int PageDepth;

        public Error(string Code, string Address, string URL, int Depth)
        {
            StatusCode = Code;
            SourceAddress = Address;
            SourceURL = URL;
            PageDepth = Depth;

        }
    }
}

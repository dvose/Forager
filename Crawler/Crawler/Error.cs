using System;
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

        public Error(string Code, string Source) {
            StatusCode = Code;
            SourceAddress = Source;
        }
    }
}

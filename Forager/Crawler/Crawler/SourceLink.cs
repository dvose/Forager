 using System;
 using System.Collections.Generic;
 using System.Linq;
 using System.Text;
 using System.Threading.Tasks;
 
 namespace Crawler
 {
     class SourceLink
     {
         public string SourceAddress;
         public string SourceURL;
         public int PageDepth;
 
         public SourceLink(string Address, string URL, int Depth)
         {
             SourceURL = URL;
             SourceAddress = Address;
             PageDepth = Depth;
 
         }
     }
 }
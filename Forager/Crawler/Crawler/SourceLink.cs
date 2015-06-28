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
         public override bool Equals(object obj)
         {
             if (obj is SourceLink) 
             {
                 return this.SourceAddress.Equals((obj as SourceLink).SourceAddress);
             }
             return base.Equals(obj);
         }

         public override int GetHashCode()
         {
             return this.SourceAddress.GetHashCode();
         }
     }
 }
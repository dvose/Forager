using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        public static void Main(string[] args)
        {
            /*PriorityQueue p;
            p = new PriorityQueue();

            Queue q;
            q = new Queue();

            WebCrawler webcrawler; 
            webcrawler = new WebCrawler(ref q);
            webcrawler.GetLinksFromPage("http://www.spsu.edu/admission", ref webcrawler);
            webcrawler.CheckLinks(ref webcrawler);

            Console.WriteLine("Back to main.");

            Console.ReadLine();*/

            CrawlerPool threads = new CrawlerPool(1000);
            Thread printerThread = new Thread(WebCrawler.printStats);
            Thread threadCounter = new Thread(CrawlerPool.AliveThreadsCount);

            WebCrawler.linkQueue.Enqueue("http://www.spsu.edu");
            printerThread.Start();
            threadCounter.Start();
            threads.StartPool();
        }
    }

    /*class PriorityQueue
    {
        public PriorityQueue()
        {
            //
        }
    }
    */

   

}
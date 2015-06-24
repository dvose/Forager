using Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Crawler
{
    public class CrawlerControl
    {
        static CrawlerPool threads;
        static Thread printerThread;
        static Thread threadCounter;

        public static void Start(int threadCount)
        {
            threads = new CrawlerPool(threadCount);
            printerThread = new Thread(WebCrawler.printStats);
            threadCounter = new Thread(CrawlerPool.AliveThreadsCount);

            WebCrawler.linkQueue.Enqueue("http://www.spsu.edu");
            printerThread.Start();
            threadCounter.Start();
            threads.StartPool();
        }

        public static void Stop()
        {
            threads.StopPool();
            printerThread.Abort();
            threadCounter.Abort();
        }
    }
}
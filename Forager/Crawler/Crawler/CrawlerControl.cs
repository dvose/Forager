using Crawler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Forager.Models;

namespace Crawler
{
    public class CrawlerControl
    {
        static CrawlerPool threads;
        static Thread printerThread;
        static Thread threadCounter;
        public static Boolean inProgress = false;

        public static int currentReportId;

        public static void Start(int threadCount)
        {
            if (!inProgress)
            {
                CrawlerControl.inProgress = true;
                CrawlerControl.Reset();
                threads = new CrawlerPool(threadCount);
                printerThread = new Thread(WebCrawler.printStats);
                threadCounter = new Thread(CrawlerPool.AliveThreadsCount);

                WebCrawler.linkQueue.Enqueue("http://www.spsu.edu");
                printerThread.Start();
                threadCounter.Start();
                threads.StartPool();
                using (ReportEntitiesContext db = new ReportEntitiesContext())
                {
                    ReportModel newReport = new ReportModel();
                    newReport.TimeStampStart = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    db.Reports.Add(newReport);
                    db.SaveChanges();
                    currentReportId = newReport.Id;
                }
            }
            else {
                System.Diagnostics.Debug.WriteLine("-----------------------------\nWebCrawler in progress\n-------------------------------");
            }
            
        }

        public static void Stop()
        {
            WebCrawler.shouldStop = true;
        }

        public static void Reset() 
        {
            WebCrawler.linkQueue = new Queue();
            WebCrawler.linksChecked = 0;
            WebCrawler.errors = new List<Error>();
            WebCrawler.checkedQueue = new Queue();
            WebCrawler.shouldStop = false;
        }
    }
}
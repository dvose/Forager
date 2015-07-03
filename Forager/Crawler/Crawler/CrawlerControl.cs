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
        public static Boolean isStopping = false;
        public static Boolean isPaused = false;
        public static string startTime = "";

        public static int currentReportId;

        public static void Start(int threadCount)
        {
            if (!inProgress)
            {
                CrawlerControl.inProgress = true;
                CrawlerControl.isStopping = false;
                CrawlerControl.isPaused = false;

                CrawlerControl.Reset();
                threads = new CrawlerPool(threadCount);
                printerThread = new Thread(WebCrawler.printStats);
                threadCounter = new Thread(CrawlerPool.AliveThreadsCount);

                SourceLink sl = new SourceLink("http://www.spsu.edu", null, 0);
                WebCrawler.linkQueue.Enqueue(sl);
                WebCrawler.rse.Set();
                printerThread.Start();
                threadCounter.Start();
                threads.StartPool();
                startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                using (ReportEntitiesContext db = new ReportEntitiesContext())
                {
                    ReportModel newReport = new ReportModel();
                    newReport.TimeStampStart = startTime;
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
            isStopping = true;
        }

        public static void Reset() 
        {
            WebCrawler.linkQueue = new Queue();
            WebCrawler.linksChecked = 0;
            WebCrawler.errors = new List<ErrorModel>();
            WebCrawler.checkedLinks = new Dictionary<string,string>();
            WebCrawler.currentLink = new SourceLink();
            WebCrawler.errorsFound = 0;
            WebCrawler.liveUpdate = "Web Crawler is ready to start";
            WebCrawler.shouldStop = false;
            WebCrawler.rse.Set();
        }

        public static void Pause()
        {
            WebCrawler.rse.Reset();
            isPaused = true;
        }

        public static void Resume()
        {
            WebCrawler.rse.Set();
            isPaused = false;
        }
    }
}
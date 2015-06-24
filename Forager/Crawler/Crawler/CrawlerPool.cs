using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class CrawlerPool
    {
        public static Thread[] threads;
        
        public CrawlerPool(int count)
        {
            threads = new Thread[count];
            for (int i = 0; i < count; i++)
            {
                threads[i] = new Thread(WebCrawler.CheckLinks);
                threads[i].Name = i.ToString();
                
            }
        }

        public void StartPool()
        {
            foreach (Thread thread in threads)
            {
                thread.Start();
            }
        }

        public void WaitForComplete()
        {
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        public static void AliveThreadsCount() 
        {
            for (int i = 0; i < 100000; i++)
            {
                Thread.Sleep(1000);
                int count = 0;
                foreach (Thread thread in threads)
                {
                    if (thread.IsAlive)
                    {
                        count++;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Alive Threads: " + count);
            }
        }


        public void StopPool()
        {
            foreach (Thread thread in threads)
            {
                thread.Abort();
            }
        }
    }
}

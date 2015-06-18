﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            PriorityQueue p;
            p = new PriorityQueue();

            Queue q;
            q = new Queue();

            Webcrawler webcrawler; 
            webcrawler = new Webcrawler(ref q);
            webcrawler.GetLinksFromPage("http://www.spsu.edu", ref webcrawler);

            Console.ReadLine();
        }
    }

    class PriorityQueue
    {
        public PriorityQueue()
        {
            //
        }
    }

    class Webcrawler
    {
        //public PriorityQueue q;

        public Queue q;

        public Webcrawler(ref Queue pq)
        {
            q = pq;
        }

        public void GetLinksFromPage(string sourceUrl, ref Webcrawler w)
        {
            string sourceHtml = w.GetWebText(sourceUrl);

            Console.WriteLine("\nGathering Links from Webpage.");

            MatchCollection AnchorTags = Regex.Matches(sourceHtml.ToLower(), @"(<a.*?>.*?</a>)", RegexOptions.Singleline);

            foreach (Match AnchorTag in AnchorTags)
            {
                string value = AnchorTag.Groups[1].Value;

                Match HrefAttribute = Regex.Match(value, @"href=\""(.*?)\""",
                    RegexOptions.Singleline);
                if (HrefAttribute.Success)
                {
                    string HrefValue = HrefAttribute.Groups[1].Value;

                    char[] href;

                    href = HrefValue.ToCharArray();

                    if(href[0].Equals('/'))
                    {
                        HrefValue = sourceUrl + HrefValue;
                    }

                    if (!q.Contains(HrefValue) && !HrefValue.Equals("#"))
                    {
                        q.Enqueue(HrefValue);
                    }
                }

                Match HrefAttribute2 = Regex.Match(value, @"<img.*?src=""(.*?)""",
                   RegexOptions.Singleline);
                if (HrefAttribute2.Success)
                {
                    string HrefValue2 = HrefAttribute2.Groups[1].Value;

                    char[] href2;

                    href2 = HrefValue2.ToCharArray();

                    if (href2[0].Equals('/'))
                    {
                        HrefValue2 = sourceUrl + HrefValue2;
                    }

                    if (!q.Contains(HrefValue2) && !HrefValue2.Equals("#"))
                    {
                        q.Enqueue(HrefValue2);
                    }
                }
            }

            Console.WriteLine("\nFinished Gathering Links.");

            while (q.Count > 0)
            {
                Console.WriteLine(q.Dequeue().ToString());
            }

            /*
            while(sourceHtml.IndexOf("<a href =") != -1)
            {
                parse through html and pull all links and add to queue
            }  */
        }

        public string GetWebText(string url)
        {
            Console.WriteLine("Getting HTML from Webpage \n");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            request.UserAgent = "A .NET Web Crawler";

            WebResponse response = request.GetResponse();

            Stream stream = response.GetResponseStream();

            StreamReader reader = new StreamReader(stream);

            string htmlText = reader.ReadToEnd();

            //Console.WriteLine(htmlText);

            Console.WriteLine("\nFinished gathering HTML");

            return htmlText;
        }
    }

}
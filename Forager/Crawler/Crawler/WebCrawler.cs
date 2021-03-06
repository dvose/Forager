﻿using Forager.Models;
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
    class WebCrawler
    {
            //public PriorityQueue q;

            public static Queue linkQueue = new Queue();
            public static Dictionary<string, string> checkedLinks = new Dictionary<string,string>();
            public static List<ErrorModel> errors = new List<ErrorModel>();
            public static int linksChecked = 0;
            public static int errorsFound = 0;
            public static Boolean shouldStop = false;
            public static ManualResetEvent rse = new ManualResetEvent(false);
            public static SourceLink currentLink =  new SourceLink();
            public static string liveUpdate = "Web Crawler is ready to start";

            /*public WebCrawler(ref Queue pq)
            {
                linkQueue = pq;
            }
             */
            public static void printStats() {
                while(!shouldStop)
                {
                    Thread.Sleep(1000);

                    if (!rse.WaitOne(0))
                    {
                        System.Diagnostics.Debug.WriteLine("Crawler is currently paused...\n");
                        liveUpdate = "\nCrawler is currently paused\n";
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("\nPages Checked " + linksChecked);
                        System.Diagnostics.Debug.WriteLine("Pages Queued: " + linkQueue.Count);
                        System.Diagnostics.Debug.WriteLine("Errors: " + errorsFound);
                        System.Diagnostics.Debug.WriteLine("Current Link: " + currentLink.SourceAddress);

                        string printableLink = currentLink.SourceAddress.Length <= 80 ? currentLink.SourceAddress : currentLink.SourceAddress.Substring(0, 80) + "...";
                        liveUpdate = "Report Start Time: " + CrawlerControl.startTime + "\n" +
                                     "\n" +
                                     "Pages Checked: " + linksChecked + "\n" +
                                     "Errors Found: " + errorsFound + "\n" +
                                     "\n" +
                                     "Current Page: " + currentLink.SourceURL + " | Depth: " + currentLink.PageDepth + "\n" +
                                     "Current Link: " + printableLink  + "\n";
                    }
                }

                liveUpdate = "Currently Stopping Web Crawler \nPlease Wait...";
            }
            public static void CheckLinks()
            {
                    while (!shouldStop)
                    {
                       //if the thread is paused, the thread will wait here until it resumes
                        rse.WaitOne();

                        if (linkQueue.Count == 0)
                        {
                            Thread.Sleep(1500);
                            if (linkQueue.Count == 0)
                                break;
                        }
                        //Console.WriteLine("Thread" + Thread.CurrentThread.Name);
                        SourceLink sl;
                        string url;
                        lock (linkQueue)
                        {
                            if (linkQueue.Count == 0) {
                                continue;
                            }
                            sl = (SourceLink) linkQueue.Dequeue();
                            url = sl.SourceAddress;
                            if(url.Contains("http://www.omniupdate.com") || url.Contains("mailto:") || !url.Contains("http")){
                                continue;
                            }
                            lock (currentLink) 
                            {
                                currentLink = sl;
                            }
                        }
                        if (url != null && !checkedLinks.ContainsKey(url))
                        {
                            lock (checkedLinks)
                            {
                                checkedLinks.Add(url, null);
                                linksChecked++;
                            }
                            if (!url.Contains("@spsu.edu") && !url.Contains("@kennesaw.edu") && !url.Contains("#"))
                            {
                                string htmltext = WebCrawler.GetWebText(sl);
                                WebCrawler.GetLinksFromPage(sl);
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Already checked Link");
                        }
                    //}
                }
            }
            public static void WriteErrors()
            {
                using (ErrorEntitiesContext errorsDb = new ErrorEntitiesContext())
                {
                    foreach (ErrorModel errorf in errors)
                    {
                        errorsDb.Errors.Add(errorf);
                    }
                    errorsDb.SaveChanges();
                }
                errors.Clear();
            }
            public static void GetLinksFromPage(SourceLink sl)
            {
                try
                {
                    string sourceUrl = sl.SourceAddress;
                    //Console.WriteLine(sourceUrl);
                    if (!sourceUrl.Contains("@spsu.edu") && !sourceUrl.Contains(".pdf") && !sourceUrl.Contains(".jpg")
                        && sourceUrl.Contains("spsu.edu") && !sourceUrl.Contains(".gif") && !sourceUrl.Contains(".png")
                        && !sourceUrl.Contains("http://www.omniupdate.com") && !sourceUrl.Contains(".pcf"))
                    {

                        string sourceHtml = WebCrawler.GetWebText(sl);

                        //Console.WriteLine("\nGathering Links from Webpage.");
                        //Console.WriteLine(sourceUrl);

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

                                //Console.WriteLine("Href Value: " + HrefValue);

                                if (!HrefValue.Equals(" ") && !HrefValue.Equals(null) && href.Length > 0)
                                {
                                    HrefValue = WebCrawler.NormalizeLink(HrefValue);
                                    if (href[0].Equals('/'))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue))
                                        {
                                            String URL = sl.SourceAddress;
                                            System.Uri uri = new System.Uri(URL);

                                            checkedLinks.Add(HrefValue, null);
                                            HrefValue = "http://" + uri.Host + HrefValue;

                                        }
                                        else 
                                        {
                                            continue;
                                        }
                                       
                                    }

                                    if (!href[0].Equals('/') && !HrefValue.Contains("http"))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue))
                                        {
                                            String URL1 = sl.SourceAddress;
                                            System.Uri uri1 = new System.Uri(URL1);

                                            checkedLinks.Add(HrefValue, null);
                                            HrefValue = "http://" + uri1.Host + "/" + HrefValue;
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }

                                    if (href[0].Equals('.'))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue))
                                        {
                                            String URL4 = sl.SourceAddress;
                                            System.Uri uri4 = new System.Uri(URL4);

                                            string subHrefValue = HrefValue.Substring(1, HrefValue.Length);

                                            checkedLinks.Add(HrefValue, null);
                                            HrefValue = "http://" + uri4.Host + subHrefValue;
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }

                                    SourceLink sl2 = new SourceLink(HrefValue, sl.SourceAddress, sl.PageDepth + 1);
                                    if (!linkQueue.Contains(sl2) && !HrefValue.Equals("#") && !HrefValue.Equals("./"))
                                    {
                                        if (!checkedLinks.ContainsKey(sl2.SourceAddress))
                                        {
                                            lock(linkQueue)
                                            {
                                                linkQueue.Enqueue(sl2);
                                            }
                                        }
                                    }
                                }
                            }

                            Match HrefAttribute2 = Regex.Match(value, @"<img.*?src=""(.*?)""",
                               RegexOptions.Singleline);
                            if (HrefAttribute2.Success)
                            {

                                string HrefValue2 = HrefAttribute2.Groups[1].Value;

                                char[] href2;

                                href2 = HrefValue2.ToCharArray();

                                //Console.WriteLine("Href Value: " + HrefValue2);

                                if (!HrefValue2.Equals(" ") && !HrefValue2.Equals(null) && href2.Length > 0)
                                {
                                    HrefValue2 = WebCrawler.NormalizeLink(HrefValue2);
                                    if (href2[0].Equals('/'))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue2))
                                        {
                                            String URL2 = sl.SourceAddress;
                                            System.Uri uri2 = new System.Uri(URL2);

                                            checkedLinks.Add(HrefValue2, null);
                                            HrefValue2 = "http://" + uri2.Host + HrefValue2;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }

                                    if (!href2[0].Equals('/') && !HrefValue2.Contains("http"))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue2))
                                        {
                                            String URL3 = sl.SourceAddress;
                                            System.Uri uri3 = new System.Uri(URL3);

                                            checkedLinks.Add(HrefValue2, null);
                                            HrefValue2 = "http://" + uri3.Host + "/" + HrefValue2;
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }

                                    if (href2[0].Equals('.'))
                                    {
                                        if (!checkedLinks.ContainsKey(HrefValue2))
                                        {
                                            String URL5 = sl.SourceAddress;
                                            System.Uri uri5 = new System.Uri(URL5);

                                            string subHrefValue = HrefValue2.Substring(1, HrefValue2.Length);

                                            checkedLinks.Add(HrefValue2, null);
                                            HrefValue2 = "http://" + uri5.Host + subHrefValue;
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }


                                    SourceLink sl2 = new SourceLink(HrefValue2, sl.SourceAddress, sl.PageDepth + 1);
                                    if (!linkQueue.Contains(sl2) && !HrefValue2.Equals("#") && !HrefValue2.Equals("./"))
                                    {
                                        lock (linkQueue)
                                        {
                                            linkQueue.Enqueue(sl2);
                                        }
                                    }
                                }
                            }
                        }

                        //Console.WriteLine("Finished Gathering Links.");
                        /*
                        while (q.Count > 0)
                        {
                          //  Console.WriteLine(q.Dequeue().ToString());
                        }

            
                        while(sourceHtml.IndexOf("<a href =") != -1)
                        {
                            parse through html and pull all links and add to queue
                        }  */
                    }
                    else
                    {
                        //Console.WriteLine("\nCannot gather links outside of SPSU domain.");
                    }
                }
                catch
                {
                    //Console.WriteLine("Miscellaneous exception thrown.");
                }
            }

            private static String NormalizeLink(String url)
            {
                //case: link ends with / example: "http://www.test.com/"
                if (url[url.Length - 1].Equals('/'))
                {
                    url = url.Substring(0, url.Length - 1);
                }

                return url;
            }
            public static string GetWebText(SourceLink sl)
            {
                WebResponse response = null;
                Stream stream = null;
                StreamReader reader = null;
                string url = sl.SourceAddress;
                try
                {

                    //Console.WriteLine("\nGetting HTML from Webpage \n");
                    //Console.WriteLine(url);
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.UserAgent = "A .NET Web Crawler";

                    if (url.Contains(".doc") || url.Contains(".ppt") || url.Contains(".pps") || url.Contains("http://www.omniupdate.com") || url.Contains(".pdf")
                        || url.Contains(".jpg") || url.Contains(".jpeg") || url.Contains(".png") || url.Contains(".gif")
                        || url.Contains(".pcf") || url.Contains(".mp4") || url.Contains(".mp3") || url.Contains(".mov")
                        || url.Contains(".avi") || url.Contains(".zip") || url.Contains(".rdo")) {
                            request.Method = "HEAD";
                    }
                    string htmlText = null;
                    request.Accept = "text/html";
                    using (response = request.GetResponse()) {

                        if (request.Method != "HEAD") {
                            using (stream = response.GetResponseStream()) {
                                using (reader = new StreamReader(stream))
                                {
                                    htmlText = reader.ReadToEnd();
                                }
                            
                            }
                        }
                    }
                
                    //Console.WriteLine(htmlText);
                    //Console.WriteLine("Finished gathering HTML");

                    return htmlText;
                }
                catch (WebException webExcp)
                {
                    //Console.WriteLine("A WebException has been caught.");
                    //Console.WriteLine(webExcp.ToString());
                    if (response != null)
                        response.Dispose();
                    if (stream != null)
                        stream.Dispose();
                    if (reader != null)
                        reader.Dispose();
                    
                    WebExceptionStatus status = webExcp.Status;
                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        //Console.Write("The server returned protocol error ");
                        HttpWebResponse httpResponse = (HttpWebResponse)webExcp.Response;
                        //Console.WriteLine((int)httpResponse.StatusCode + " - "
                           //+ httpResponse.StatusCode);


                        ErrorModel error = new ErrorModel();
                        error.ErrorStatus = "Status Code: " + (int)((HttpWebResponse)webExcp.Response).StatusCode + " - " + ((HttpWebResponse)webExcp.Response).StatusCode.ToString();
                        error.Link = sl.SourceAddress;
                        error.WebPage = sl.SourceURL;
                        error.Depth = sl.PageDepth;
                        error.ErrorTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        error.ReportId = CrawlerControl.currentReportId;

                        lock (errors) 
                        {
                            errors.Add(error);
                            errorsFound++;
                            if (errors.Count > 100) 
                            {
                                WebCrawler.WriteErrors();
                            }
                        }

                        //using (ErrorEntitiesContext errorsDb = new ErrorEntitiesContext())
                        //{

                        //    errorsDb.Errors.Add(error);
                        //    errorsDb.SaveChanges();
                        //}
                        
                    }
                    //Console.WriteLine(errors.Last().StatusCode);
                    return webExcp.ToString();
                }
                catch
                {
                    //Console.WriteLine("Invalid link");
                    return null;
                }
        }
    }
}

using Forager.Models;
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
            public static Queue checkedQueue = new Queue();
            public static List<Error> errors = new List<Error>();
            public static int linksChecked = 0;
            public static Boolean shouldStop = false;

            /*public WebCrawler(ref Queue pq)
            {
                linkQueue = pq;
            }
             */
            public static void printStats() {
                while(!shouldStop)
                {
                    Thread.Sleep(1000);
                    System.Diagnostics.Debug.WriteLine("\nPages Checked " + linksChecked);
                    System.Diagnostics.Debug.WriteLine("Pages Queued: " + linkQueue.Count);
                    if (errors.Count > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Errors Logged: " + errors.Count);
                        //foreach (Error error in errors)
                        //{
                        //    Console.WriteLine(error.StatusCode + " " + error.SourceAddress);
                        //}
                    }
                }
            }
            public static void CheckLinks()
            {
                //Console.WriteLine("Checking Links \n");
                //System.IO.File.WriteAllText(@"C:\Users\Dustin\Documents\HtmlLinks.txt", string.Empty);
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Dustin\Documents\HtmlLinks.txt"))
                //{
                    while (!shouldStop)
                    {
                        if (linkQueue.Count == 0)
                        {
                            Thread.Sleep(1500);
                            if (linkQueue.Count == 0)
                                break;
                        }
                        //Console.WriteLine("Thread" + Thread.CurrentThread.Name);
                        string url;
                        lock (linkQueue)
                        {
                            if (linkQueue.Count == 0) {
                                continue;
                            }
                            url = (string) linkQueue.Dequeue();
                        }
                        if (url != null && !checkedQueue.Contains(url))
                        {
                            lock (checkedQueue)
                            {
                                checkedQueue.Enqueue(url);
                                linksChecked++;
                            }
                            if (!url.Contains("@spsu.edu") && !url.Contains("@kennesaw.edu") && !url.Contains("#"))
                            {
                                string htmltext = WebCrawler.GetWebText(url);
                                WebCrawler.GetLinksFromPage(url);
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Already checked Link");
                        }
                    //}
                }
            }
            public static void GetLinksFromPage(string sourceUrl)
            {
                try
                {
                    //Console.WriteLine(sourceUrl);
                    if (!sourceUrl.Contains("@spsu.edu") && !sourceUrl.Contains(".pdf") && !sourceUrl.Contains(".jpg")
                        && sourceUrl.Contains("spsu.edu") && !sourceUrl.Contains(".gif") && !sourceUrl.Contains(".png")
                        && !sourceUrl.Contains("http://www.omniupdate.com") && !sourceUrl.Contains(".pcf"))
                    {

                        string sourceHtml = WebCrawler.GetWebText(sourceUrl);

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
                                    if (href[0].Equals('/'))
                                    {
                                        HrefValue = "http://www.spsu.edu" + HrefValue;
                                    }

                                    if (!linkQueue.Contains(HrefValue) && !HrefValue.Equals("#") && !HrefValue.Equals("./"))
                                    {
                                        lock(linkQueue)
                                        {
                                            linkQueue.Enqueue(HrefValue);
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

                                    if (href2[0].Equals('/'))
                                    {
                                        HrefValue2 = "http://www.spsu.edu" + HrefValue2;
                                    }


                                    if (!linkQueue.Contains(HrefValue2) && !HrefValue2.Equals("#") && !HrefValue2.Equals("./"))
                                    {
                                        lock (linkQueue)
                                        {
                                            linkQueue.Enqueue(HrefValue2);
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
            public static string GetWebText(string url)
            {
                WebResponse response = null;
                Stream stream = null;
                StreamReader reader = null;
                try
                {

                    //Console.WriteLine("\nGetting HTML from Webpage \n");
                    //Console.WriteLine(url);
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.UserAgent = "A .NET Web Crawler"; 
                    string htmlText;
                    request.Accept = "text/html";
                    using (response = request.GetResponse()) {
                       
                        using (stream = response.GetResponseStream()) {
                            using (reader = new StreamReader(stream))
                            {
                                htmlText = reader.ReadToEnd();
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


                        using (ErrorEntitiesContext errorsDb = new ErrorEntitiesContext())
                        {
                            ErrorModel error = new ErrorModel();
                            error.ErrorStatus = webExcp.ToString();
                            error.Link = url;
                            error.ErrorTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            error.ReportId = CrawlerControl.currentReportId;
                            errorsDb.Errors.Add(error);
                            errorsDb.SaveChanges();
                        }
                        
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

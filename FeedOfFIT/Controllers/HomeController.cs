using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FeedOfFIT.Models;

namespace FeedOfFIT.Controllers
{
    public class HomeController : Controller
    {
        private static List<RssItem> Items = null;
 
        public ActionResult Index()
        {
            ViewBag.RssTitle = "News feed of website FIT - HCMUS";
            NewRss rss = new NewRss();
            Items = rss.getNewsList();
            ViewBag.NewsList = rss.getNewsList();
            return View();
        }

        public ActionResult RssResult()
        {
            NewRss rss = new NewRss();

            RssChanel rssChanel = new RssChanel();
            rssChanel.Title = "RssFeed Web Khoa Công Nghệ Thông Tin - Trường ĐH KHTN";
            rssChanel.Link = "http://www.fit.hcmus.edu.vn/vn/";

            rss.AddRssChanel(rssChanel);

            foreach (RssItem item in Items)
            {
                rss.AddRssItem(item);
            }

            Response.Clear();
            Response.ContentType = "text/xml";
            Response.Write(rss.RssDocument);
            Response.End();
            return View();
        }
    }
}

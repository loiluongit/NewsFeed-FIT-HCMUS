using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml;
using HtmlAgilityPack;

namespace FeedOfFIT.Models
{
    public class NewRss
    {
        private XmlDocument _rss;
        private HtmlNode _rootNode;
        private HtmlNode _singleNode;
        private int day, month, year;
        private string title, link;

        public NewRss()
        {
            _rss = new XmlDocument();
            XmlDeclaration xmlDeclaration = _rss.CreateXmlDeclaration("1.0", "utf-8", null);
            _rss.InsertBefore(xmlDeclaration, _rss.DocumentElement);

            XmlElement rssElement = _rss.CreateElement("rss");
            XmlAttribute rssVersionAttribute = _rss.CreateAttribute("version");

            rssVersionAttribute.InnerText = "2.0";
            rssElement.Attributes.Append(rssVersionAttribute);
            _rss.AppendChild(rssElement);
        }

        private static XmlDocument addRssItem(XmlDocument xmlDocument, RssItem rssItem)
        {
            XmlElement itemElement = xmlDocument.CreateElement("item");
            XmlNode chanelElement = xmlDocument.SelectSingleNode("rss/chanel");
            
            //Add title tag
            XmlElement titleElement = xmlDocument.CreateElement("title");
            titleElement.InnerText = rssItem.Title;
            itemElement.AppendChild(titleElement);
    
            //Add link tag
            XmlElement linkElement = xmlDocument.CreateElement("link");
            linkElement.InnerText = rssItem.Link;
            itemElement.AppendChild(linkElement);

            //Add Description tag (creation time)
            XmlElement dateElement = xmlDocument.CreateElement("date");
            dateElement.InnerText = rssItem.CreateAt.ToShortDateString();
            itemElement.AppendChild(dateElement);

            //Add item elament into chanel element
            chanelElement.AppendChild(itemElement);
            return xmlDocument;
        }

        private static XmlDocument addRssChanel(XmlDocument xmlDocument, RssChanel rssChanel)
        {
            XmlElement chanelElement = xmlDocument.CreateElement("chanel");
            XmlNode rssElement = xmlDocument.SelectSingleNode("rss");
            rssElement.AppendChild(chanelElement);

            //Add title tag
            XmlElement titleElement = xmlDocument.CreateElement("title");
            titleElement.InnerText = rssChanel.Title;
            chanelElement.AppendChild(titleElement);

            //Add link tag
            XmlElement linkElement = xmlDocument.CreateElement("link");
            linkElement.InnerText = rssChanel.Link;
            chanelElement.AppendChild(linkElement);

            return xmlDocument;
        }

        public void AddRssChanel(RssChanel rssChanel)
        {
            _rss = addRssChanel(_rss, rssChanel);
        }

        public void AddRssItem(RssItem item)
        {
            _rss = addRssItem(_rss, item);
        }

        public string RssDocument
        {
            get { return _rss.OuterXml; }
        }

        public XmlDocument RssXmlDocument
        {
            get { return _rss; }
        }

        public List<RssItem> getNewsList()
        {
            HtmlDocument hd;
            try
            {
                HtmlWeb hw = new  HtmlWeb();
                hd = hw.Load("http://www.fit.hcmus.edu.vn/vn/");
            }
            catch (System.ArgumentException)
            {
                hd = new HtmlDocument();
            }

            HtmlNode hn = hd.DocumentNode;
            _rootNode = hn;
            HtmlNodeCollection hnc = _rootNode.SelectNodes("//table[@id='dnn_ctr989_ModuleContent']/tr/td/table");
            string web = "http://www.fit.hcmus.edu.vn/vn/";

            List<RssItem> newsList = new List<RssItem>();

            int i = 1;
            foreach (HtmlNode item in hnc)
            {
                RssItem rssItem = new RssItem();
                
                _singleNode = item.SelectSingleNode(".//td[@class='day_month']");
                day = int.Parse(_singleNode.InnerText);

                _singleNode = item.SelectSingleNode(".//tr[2]/td[@class='day_month']");
                month = int.Parse(_singleNode.InnerText);

                _singleNode = item.SelectSingleNode(".//td[@class='post_year']");
                year = int.Parse(_singleNode.InnerText);

                //CreateAt
                DateTime date = new DateTime(year, month, day);
                rssItem.CreateAt = date;

                //Title
                _singleNode = item.SelectSingleNode(".//td[@class='post_title']/a");
                title = _singleNode.InnerHtml.ToString();
                rssItem.Title = title;

                //Link
                link = _singleNode.Attributes["href"].Value;
                link = web + link;
                rssItem.Link = link;

                newsList.Add(rssItem);
                i++;
            }
            return newsList;
        }
    }
}
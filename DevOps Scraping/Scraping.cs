using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using System.Threading;

namespace DevOps_Scraping
{
    public class Scraping
    {
        public static void Youtube(string path, string searchTerm, bool isCsv, bool isJson)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.youtube.com/");
            driver.Manage().Window.Maximize();

            var accept = driver.FindElement(By.XPath("/html/body/ytd-app/ytd-consent-bump-v2-lightbox/tp-yt-paper-dialog/div[4]/div[2]/div[6]/div[1]/ytd-button-renderer[2]/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            accept.Click();


            Thread.Sleep(1000);
            var searchBox = driver.FindElement(By.XPath("//input[@id='search']"));
            searchBox.SendKeys(searchTerm);
            searchBox.Submit();

            Thread.Sleep(1000);
            var filter = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[1]/div[2]/ytd-search-sub-menu-renderer/div[1]/div/ytd-toggle-button-renderer/yt-button-shape/button/yt-touch-feedback-shape/div/div[2]"));
            filter.Click();

            Thread.Sleep(1000);
            var sortLastHour = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[1]/div[2]/ytd-search-sub-menu-renderer/div[1]/iron-collapse/div/ytd-search-filter-group-renderer[1]/ytd-search-filter-renderer[1]/a/div/yt-formatted-string"));
            sortLastHour.Click();

            Thread.Sleep(1000);
            filter.Click();
            var sortByDate = driver.FindElement(By.XPath("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[1]/div[2]/ytd-search-sub-menu-renderer/div[1]/iron-collapse/div/ytd-search-filter-group-renderer[5]/ytd-search-filter-renderer[2]/a/div/yt-formatted-string"));
            sortByDate.Click();
            Thread.Sleep(1000);

            List<string> videoList = new();
            List<Video> videos = new();
            for (int i = 1; i < 6; i++)
            {
                string videoTitle = "", videoViews = "", videoChannel = "", videoURL = "";
                Thread.Sleep(1000);
                try
                {
                    string titleXPath = string.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a/yt-formatted-string", i);
                    videoTitle = driver.FindElement(By.XPath(titleXPath)).Text.Replace(";", string.Empty);

                    string viewsXPath = string.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]", i);
                    videoViews = driver.FindElement(By.XPath(viewsXPath)).Text.Replace(";", string.Empty);

                    string channelXPath = string.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a", i);
                    videoChannel = driver.FindElement(By.XPath(channelXPath)).Text.Replace(";", string.Empty);

                    string linkXPath = string.Format("/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div[2]/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[{0}]/div[1]/div/div[1]/div/h3/a", i);
                    videoURL = driver.FindElement(By.XPath(linkXPath)).GetAttribute("href");

                    if (i == 3) ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.documentElement.clientHeight);");

                }
                // when there are less than 5 videos uploaded in the last hour with the entered keyword 
                catch (NoSuchElementException e)
                {
                    Console.WriteLine(e);
                }

                string record = string.Join(";", videoTitle, videoViews, videoChannel, videoURL);
                videoList.Add(record);
                videos.Add(new Video(videoTitle, videoViews, videoChannel, videoURL));
            }

            string csvPath = Path.Combine(path, string.Format("{0}.csv", searchTerm));
            string jsonPath = Path.Combine(path, string.Format("{0}.json", searchTerm));
            if (isJson)
            {
                StreamWriter jsonWriter = File.CreateText(jsonPath);
                string json = JsonConvert.SerializeObject(videos);
                jsonWriter.Write(json);
                jsonWriter.Close();
            }

            if(isCsv) File.WriteAllLines(csvPath, videoList);
            driver.Quit();

        }

        public static void JobSite(string path, string searchTerm, bool isCsv, bool isJson)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.ictjob.be/");
            driver.Manage().Window.Maximize();

            var searchBox = driver.FindElement(By.XPath("//*[@id=\"keywords-input\"]"));
            searchBox.SendKeys(searchTerm);
            searchBox.Submit();

            Thread.Sleep(1000);
            var filter = driver.FindElement(By.XPath("//*[@id=\"sort-by-date\"]"));
            filter.Click();
            Thread.Sleep(10000);
            var sumListings = driver.FindElement(By.XPath("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[1]/div[2]/span")).Text;


            int max = Convert.ToInt32(sumListings);
            int limit = 6;
            if (max < limit)
            {
                limit = max;
                if (limit > 3) limit++;
            }

            List<string> jobList = new();
            List<Job> listings = new();
            for (int i = 1; i < limit + 1; i++)
            {
                if (i == 4) continue;
                string jobTitle = "", jobCompany = "", jobLocation = "", jobKeywords = "", jobLink = "";
                Thread.Sleep(1000);
                try
                {
                    string titleXPath = string.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]/span[2]/a/h2", i);
                    jobTitle = driver.FindElement(By.XPath(titleXPath)).Text.Replace(";", string.Empty);

                    string companyXPath = string.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]/span[2]/span[1]", i);
                    jobCompany = driver.FindElement(By.XPath(companyXPath)).Text.Replace(";", string.Empty);

                    string locationXPath = string.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]/span[2]/span[2]/span[2]/span/span", i);
                    jobLocation = driver.FindElement(By.XPath(locationXPath)).Text.Replace(";", string.Empty);

                    string keywordsXPath = string.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]/span[2]/span[3]", i);
                    jobKeywords = driver.FindElement(By.XPath(keywordsXPath)).Text.Replace(";", string.Empty);

                    string linkXPath = string.Format("/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[{0}]/span[2]/a", i);
                    jobLink = driver.FindElement(By.XPath(linkXPath)).GetAttribute("href");

                }
                catch (NoSuchElementException e)
                {
                    Console.WriteLine(e);
                }

                string record = string.Join(";", jobTitle, jobCompany, jobLocation, jobKeywords, jobLink);
                jobList.Add(record);
                listings.Add(new Job(jobTitle, jobCompany, jobLocation, jobKeywords, jobLink));


            }

            string csvPath = Path.Combine(path, string.Format("{0}.csv", searchTerm));
            string jsonPath = Path.Combine(path, string.Format("{0}.json", searchTerm));
            if (isJson)
            {
                StreamWriter jsonWriter = File.CreateText(jsonPath);
                string json = JsonConvert.SerializeObject(listings);
                jsonWriter.Write(json);
                jsonWriter.Close();
            }
            if (isCsv) File.WriteAllLines(csvPath, jobList);
            driver.Quit();
        }

        public static void Ebay(string path, string searchTerm,bool isCsv, bool isJson)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://www.benl.ebay.be/");
            driver.Manage().Window.Maximize();

            Thread.Sleep(1000);
            var accept = driver.FindElement(By.XPath("//*[@id=\"gdpr-banner-accept\"]"));
            accept.Click();

            Thread.Sleep(1000);
            var searchBox = driver.FindElement(By.XPath("//*[@id=\"gh-ac\"]"));
            searchBox.SendKeys(searchTerm);
            searchBox.Submit();

            Thread.Sleep(1000);
            var dropdownElement = driver.FindElement(By.XPath("/html/body/div[8]/div[4]/div[1]/div/div[1]/div[3]/div[1]/div/span/button"));
            dropdownElement.Click();

            Thread.Sleep(1000);
            var filter = driver.FindElement(By.XPath("/html/body/div[8]/div[4]/div[1]/div/div[1]/div[3]/div[1]/div/span/span/ul/li[3]/a"));
            filter.Click();
            Thread.Sleep(1000);

            List<string> productList = new();
            List<Product> products = new List<Product>();

            for (int i = 2; i < 7; i++)
            {
                string productTitle = "", productPrice = "", productCondition = "", productDeliveryCost = "", productDate = "", productLocation = "";
                Thread.Sleep(2000);
                try
                {
                    string titleXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/a/div/span", i);
                    productTitle = driver.FindElement(By.XPath(titleXPath)).Text.Replace(";", string.Empty);

                    string priceXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/div[2]/div[1]/span/span", i);
                    productPrice = driver.FindElement(By.XPath(priceXPath)).Text.Replace(";", string.Empty);

                    string conditionXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/div[1]/span", i);
                    productCondition = driver.FindElement(By.XPath(conditionXPath)).Text.Replace(";", string.Empty);

                    string deliveryCostXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/div[2]/div[3]/span/span", i);
                    productDeliveryCost = driver.FindElement(By.XPath(deliveryCostXPath)).Text.Replace(";", string.Empty);

                    string dateXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/div[2]/span[1]/span/span", i);
                    productDate = driver.FindElement(By.XPath(dateXPath)).Text.Replace(";", string.Empty);

                    string locationXPath = string.Format("/html/body/div[8]/div[4]/div[2]/div[1]/div[2]/ul/li[{0}]/div/div[2]/div[2]/div[4]/span/span", i);
                    productLocation = driver.FindElement(By.XPath(locationXPath)).Text.Replace(";", string.Empty);

                    if(i == 4) ((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.documentElement.clientHeight);");

                }
                catch (NoSuchElementException e)
                {
                    Console.WriteLine(e);
                }
                string record = string.Join(";", productTitle, productPrice, productCondition, productDeliveryCost, productDate, productLocation);
                productList.Add(record);
                products.Add(new Product(productTitle, productPrice, productCondition, productDeliveryCost, productDate, productLocation));

            }

            string csvPath = Path.Combine(path, string.Format("{0}.csv", searchTerm));
            string jsonPath = Path.Combine(path, string.Format("{0}.json", searchTerm));

            if (isJson)
            {
                StreamWriter jsonWriter = File.CreateText(jsonPath);
                string json = JsonConvert.SerializeObject(products);
                jsonWriter.Write(json);
                jsonWriter.Close();
            }

            if (isCsv) File.WriteAllLines(csvPath, productList);
            driver.Quit();
        }

    }
}

﻿namespace DevOps_Scraping
{
    public class Job
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Keywords { get; set; }
        public string Link { get; set; }

        public Job(string title, string company, string location, string keywords, string link)
        {
            this.Title = title;
            this.Company = company;
            this.Location = location;
            this.Keywords = keywords;
            this.Link = link;
        }
    }
}

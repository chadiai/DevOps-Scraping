namespace DevOps_Scraping
{
    public class Video
    {
        public string Title { get; set; }
        public string Views { get; set; }
        public string Channel { get; set; }
        public string Link { get; set; }

        public Video(string videotitle, string videoViews, string videoChannel, string videoURL)
        {
            this.Title = videotitle;
            this.Views = videoViews;
            this.Channel = videoChannel;
            this.Link = videoURL;
        }
    }
}

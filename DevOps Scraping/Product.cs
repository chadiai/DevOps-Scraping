namespace DevOps_Scraping
{
    public class Product
    {
        public string Title { get; set; }
        public string Price { get; set; }
        public string Condition { get; set; }
        public string DeliveryCost { get; set; }
        public string ListingDate { get; set; }
        public string Location { get; set; }


        public Product(string title, string price, string condition, string deliveryCost, string date, string location)
        {
            this.Title = title;
            this.Price = price;
            this.Condition = condition;
            this.DeliveryCost = deliveryCost;
            this.ListingDate = date;
            this.Location = location;
        }
    }
}

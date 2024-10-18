using System;

namespace OnlineAuction.Models
{
    public class ProductDisplayViewModel
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal MinimumBid { get; set; }
        public string ImagePath { get; set; } // Path to the image
        public string DocumentPath { get; set; } // Path to the document
        public DateTime BidStartDate { get; set; }
        public DateTime BidEndDate { get; set; }
        public int CategoryId { get; set; }
        public string ApplicationUserId { get; set; }
    }
}

using OnlineAuction.Areas.Identity.Data;
using System;

namespace OnlineAuction.Models
{
    public class Bid
    {
        public int BidId { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}

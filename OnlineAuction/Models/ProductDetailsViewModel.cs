using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineAuction.Models
{
    public class ProductDetailsViewModel
    {
        // Product Details
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal MinimumBid { get; set; }
        public string ImagePath { get; set; }
        public string DocumentPath { get; set; }
        public DateTime BidStartDate { get; set; }
        public DateTime BidEndDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ApplicationUserId { get; set; }

        // Bid Information (Bid History)
        public List<BidDisplayViewModel> Bids { get; set; }

        // User Bid Input
        [Display(Name = "Your Bid")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal? UserBidAmount { get; set; }

        // Highest Bid Details
        public decimal HighestBid { get; set; }
        public string HighestBidder { get; set; }
        public DateTime? HighestBidTime { get; set; }

        // Auction Status
        public string AuctionStatusMessage { get; set; }

        // To disable/enable the Place Bid button
        public bool CanPlaceBid { get; set; }
    }

    public class BidDisplayViewModel
    {
        public string BidderUsername { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
    }
}

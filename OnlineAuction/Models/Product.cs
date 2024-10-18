using OnlineAuction.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineAuction.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal MinimumBid { get; set; }

        //This will store the file path
        public string ImagePath { get; set; }

        // This will store the file path for documents
        public string DocumentPath { get; set; }

        public DateTime BidStartDate { get; set; }

        public DateTime BidEndDate { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        // Foreign key for ApplicationUser
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        // This property is not saved in the database but used for file upload

        public virtual ICollection<Bid> Bids { get; set; }


    }
}

using System.ComponentModel.DataAnnotations;

namespace OnlineAuction.Models
{
    public class CheckoutViewModel
    {
        public int BidId { get; set; }
        public string ProductTitle { get; set; }
        public decimal BidAmount { get; set; }
        public DateTime BidTime { get; set; }
        public string ProductImagePath { get; set; }

        // Billing Information
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "ZIP Code")]
        public string ZipCode { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; }
    }
}

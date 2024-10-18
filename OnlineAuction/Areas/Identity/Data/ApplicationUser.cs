using Microsoft.AspNetCore.Identity;
using OnlineAuction.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineAuction.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        // One-to-many relationship with Product
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
    }
}

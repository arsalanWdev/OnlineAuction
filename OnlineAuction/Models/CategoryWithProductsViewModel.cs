using System.Collections.Generic;

namespace OnlineAuction.Models
{
    public class CategoryWithProductsViewModel
    {
        public CategoryViewModel Category { get; set; }
        public List<ProductDisplayViewModel> Products { get; set; }
    }
}

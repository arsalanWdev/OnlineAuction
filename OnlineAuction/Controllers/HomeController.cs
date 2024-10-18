using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineAuction.Data;
using OnlineAuction.Models;
using System.Diagnostics;
using System.Linq;

namespace OnlineAuction.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all categories
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            // Initialize the HomepageViewModel
            var homepageViewModel = new HomepageViewModel
            {
                CategoriesWithProducts = new List<CategoryWithProductsViewModel>()
            };

            foreach (var category in categories)
            {
                // Fetch products for the current category
                var products = await _context.Products
                    .Where(p => p.CategoryId == category.CategoryId)
                    .ToListAsync();

                // Map products to ProductDisplayViewModel
                var productViewModels = products.Select(p => new ProductDisplayViewModel
                {
                    ProductId = p.ProductId,
                    Title = p.Title,
                    Description = p.Description,
                    MinimumBid = p.MinimumBid,
                    ImagePath = p.ImagePath, // Ensure this is the correct relative path
                    DocumentPath = p.DocumentPath, // Ensure this is the correct relative path
                    BidStartDate = p.BidStartDate,
                    BidEndDate = p.BidEndDate,
                    CategoryId = p.CategoryId,
                    ApplicationUserId = p.ApplicationUserId
                }).ToList();

                // Add to the HomepageViewModel
                homepageViewModel.CategoriesWithProducts.Add(new CategoryWithProductsViewModel
                {
                    Category = new CategoryViewModel
                    {
                        CategoryId = category.CategoryId,
                        CategoryName = category.CategoryName
                    },
                    Products = productViewModels
                });
            }

            return View(homepageViewModel);
        }

        // Action to display products of a selected category
        public IActionResult CategoryProduct(int? categoryId)
        {
            if (categoryId == null)
            {
                // Redirect to Home page if categoryId is not provided
                return RedirectToAction("Index");
            }

            // Fetch the category to ensure it exists
            var category = _context.Categories
                .FirstOrDefault(c => c.CategoryId == categoryId);

            if (category == null)
            {
                // Return 404 Not Found if category does not exist
                return NotFound();
            }

            // Fetch products belonging to the selected category
            var products = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            // Map products to ProductDisplayViewModel
            var productDisplayList = products.Select(p => new ProductDisplayViewModel
            {
                ProductId = p.ProductId,
                Title = p.Title,
                Description = p.Description,
                MinimumBid = p.MinimumBid,
                ImagePath = p.ImagePath, // Assuming ImagePath holds the relative path
                DocumentPath = p.DocumentPath, // Path to the document
                BidStartDate = p.BidStartDate,
                BidEndDate = p.BidEndDate,
                CategoryId = p.CategoryId,
                ApplicationUserId = p.ApplicationUserId
            }).ToList();

            // Populate the ViewModel
            var viewModel = new CategoryProductViewModel
            {
                CategoryName = category.CategoryName,
                Products = productDisplayList
            };

            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}

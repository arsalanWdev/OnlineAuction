using Microsoft.AspNetCore.Mvc;
using OnlineAuction.Data;
using OnlineAuction.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Add this to manage the user identity
using OnlineAuction.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;

namespace OnlineAuction.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApplicationDbContext _ApplicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager; // Add this to manage the current user
        
        public ProductsController(ILogger<ProductsController> logger,ApplicationDbContext ApplicationDbContext, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _ApplicationDbContext = ApplicationDbContext;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager; // Inject the user manager
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID
            var userProducts = await _ApplicationDbContext.Products
                                  .Where(p => p.ApplicationUserId == currentUserId)
                                  .ToListAsync();

            return View(userProducts);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.ApplicationUsers = new SelectList(_ApplicationDbContext.Users, "Id", "UserName");
            ViewBag.Categories = new SelectList(_ApplicationDbContext.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID
                var product = new Product
                {
                    Title = model.Title,
                    Description = model.Description,
                    MinimumBid = model.MinimumBid,
                    BidStartDate = model.BidStartDate,
                    BidEndDate = model.BidEndDate,
                    CategoryId = model.CategoryId,
                    ApplicationUserId = currentUserId // Assign the current user ID to the product
                };

                // Handle image and document file uploads
                if (model.ImageFile != null)
                {
                    var uniqueImageName = $"{Guid.NewGuid()}_{model.ImageFile.FileName}";
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", uniqueImageName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }
                    product.ImagePath = "Images/" + uniqueImageName;
                }

                if (model.DocumentFile != null)
                {
                    var uniqueDocumentName = $"{Guid.NewGuid()}_{model.DocumentFile.FileName}";
                    var documentPath = Path.Combine(_webHostEnvironment.WebRootPath, "Documents", uniqueDocumentName);
                    using (var stream = new FileStream(documentPath, FileMode.Create))
                    {
                        await model.DocumentFile.CopyToAsync(stream);
                    }
                    product.DocumentPath = "Documents/" + uniqueDocumentName;
                }

                try
                {
                    _ApplicationDbContext.Add(product);
                    await _ApplicationDbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Check if it's a truncation issue
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("String or binary data would be truncated"))
                    {
                        ModelState.AddModelError("", "Data exceeds allowed limits. Please ensure all fields are within the maximum length.");
                    }
                    else
                    {
                        // Log the exception and rethrow or handle other errors
                        ModelState.AddModelError("", "An error occurred while saving the data. Please try again.");
                    }
                }
            }

            ViewBag.Categories = new SelectList(_ApplicationDbContext.Categories, "CategoryId", "CategoryName");
            return View(model);
        }


        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ApplicationDbContext.Products.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID

            if (product == null || product.ApplicationUserId != currentUserId)
            {
                return Unauthorized(); // Ensure the user can only edit their own products
            }

            ViewBag.Categories = new SelectList(_ApplicationDbContext.Categories, "CategoryId", "CategoryName", product.CategoryId);

            var model = new ProductViewModel
            {
                ProductId = product.ProductId,
                Title = product.Title,
                Description = product.Description,
                MinimumBid = product.MinimumBid,
                BidStartDate = product.BidStartDate,
                BidEndDate = product.BidEndDate,
                CategoryId = product.CategoryId,
                ApplicationUserId = product.ApplicationUserId,
            };

            return View(model);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            if (id != model.ProductId)
            {
                return NotFound();
            }

            var product = await _ApplicationDbContext.Products.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if (product == null || product.ApplicationUserId != currentUserId)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update product fields
                    product.Title = model.Title;
                    product.Description = model.Description;
                    product.MinimumBid = model.MinimumBid;
                    product.BidStartDate = model.BidStartDate;
                    product.BidEndDate = model.BidEndDate;
                    product.CategoryId = model.CategoryId;

                    _ApplicationDbContext.Update(product);
                    await _ApplicationDbContext.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex) when (ex.Message.Contains("String or binary data would be truncated"))
                {
                    ModelState.AddModelError(string.Empty, "Data exceeds the allowed size. Please review the input limits.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the product.");
                }
            }

            ViewBag.Categories = new SelectList(_ApplicationDbContext.Categories, "CategoryId", "CategoryName", model.CategoryId);
            return View(model);
        }


        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _ApplicationDbContext.Products.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID

            if (product == null || product.ApplicationUserId != currentUserId)
            {
                return Unauthorized(); // Ensure the user can only delete their own products
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _ApplicationDbContext.Products.FindAsync(id);
            var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID

            if (product == null || product.ApplicationUserId != currentUserId)
            {
                return Unauthorized(); // Ensure the user can only delete their own products
            }

            _ApplicationDbContext.Products.Remove(product);
            await _ApplicationDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Details work
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _ApplicationDbContext.Products
                .Include(p => p.Bids)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Get the current date and time (UTC)
            var currentUtcTime = DateTime.UtcNow;

            // Initialize auction status message and state
            string auctionStatusMessage;
            bool canPlaceBid = true;

            // Determine if auction is not started, ongoing, or closed
            if (currentUtcTime < product.BidStartDate)
            {
                auctionStatusMessage = "Auction has not started yet.";
                canPlaceBid = false;
            }
            else if (currentUtcTime > product.BidEndDate)
            {
                auctionStatusMessage = "Auction has ended.";
                canPlaceBid = false;
            }
            else
            {
                auctionStatusMessage = "Auction is open.";
                canPlaceBid = true;
            }

            // Get the highest bid if any
            var highestBid = product.Bids.OrderByDescending(b => b.BidAmount).FirstOrDefault();
            var currentUserId = _userManager.GetUserId(User); // Get the logged-in user ID

            var model = new ProductDetailsViewModel
{
    ProductId = product.ProductId,
    Title = product.Title,
    Description = product.Description,
    MinimumBid = product.MinimumBid,
    ImagePath = product.ImagePath,
    DocumentPath = product.DocumentPath,
    BidStartDate = product.BidStartDate,
    BidEndDate = product.BidEndDate,
    CategoryId = product.CategoryId,
    CategoryName = product.Category.CategoryName,
    ApplicationUserId = product.ApplicationUserId,

    Bids = product.Bids.Select(b => new BidDisplayViewModel
    {
        BidderUsername = b.ApplicationUser?.UserName ?? "Unknown Bidder",  // Null check for ApplicationUser
        BidAmount = b.BidAmount,
        BidTime = b.BidTime
    }).ToList(),

    HighestBid = highestBid?.BidAmount ?? product.MinimumBid,
    HighestBidder = highestBid?.ApplicationUser?.UserName ?? "Unknown Bidder",  // Null check for ApplicationUser
    HighestBidTime = highestBid?.BidTime,

    AuctionStatusMessage = auctionStatusMessage,
                //CanPlaceBid = canPlaceBid 
                CanPlaceBid = currentUserId != product.ApplicationUserId
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ProductDetailsViewModel model)
        {
            // Ensure to load Bids and ApplicationUser data
            var product = await _ApplicationDbContext.Products
                .Include(p => p.Category) // Include category if needed
                .Include(p => p.Bids)
                    .ThenInclude(b => b.ApplicationUser) // Include ApplicationUser for bids
                .FirstOrDefaultAsync(p => p.ProductId == model.ProductId);

            if (product == null)
            {
                return NotFound();
            }

            var highestBid = product.Bids.OrderByDescending(b => b.BidAmount).FirstOrDefault();

            // Check if the bid is valid (must be higher than the current highest bid or the minimum bid)
            if (model.UserBidAmount.HasValue && model.UserBidAmount > (highestBid?.BidAmount ?? product.MinimumBid))
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Add the new bid to the database
                var newBid = new Bid
                {
                    ProductId = model.ProductId,
                    BidAmount = model.UserBidAmount.Value,
                    BidTime = DateTime.Now,
                    ApplicationUserId = currentUser.Id
                };

                _ApplicationDbContext.Bids.Add(newBid);
                await _ApplicationDbContext.SaveChangesAsync();

                // Redirect to refresh the page with updated bids
                //return RedirectToAction(nameof(Details), new { id = model.ProductId });
                return Json(new { success = true });
            }

            // If the bid is not valid, reload the product details with an error
            ModelState.AddModelError("UserBidAmount", "Your bid must be higher than the current highest bid.");
            return Json(new { success = false, message = "Bid amount must be higher." });

            // Reload all the necessary product data
            model.ProductId = product.ProductId;
            model.Title = product.Title;
            model.Description = product.Description;
            model.MinimumBid = product.MinimumBid;
            model.ImagePath = product.ImagePath;
            model.DocumentPath = product.DocumentPath;
            model.BidStartDate = product.BidStartDate;
            model.BidEndDate = product.BidEndDate;
            model.CategoryId = product.CategoryId;
            model.CategoryName = product.Category?.CategoryName;

            // Populate bid details
            model.Bids = product.Bids.Select(b => new BidDisplayViewModel
            {
                BidderUsername = b.ApplicationUser?.UserName ?? "Anonymous",
                BidAmount = b.BidAmount,
                BidTime = b.BidTime
            }).ToList();

            model.HighestBid = highestBid?.BidAmount ?? product.MinimumBid;
            model.HighestBidder = highestBid?.ApplicationUser?.UserName ?? "No bids yet";
            model.HighestBidTime = highestBid?.BidTime;

            // Return the view with updated model
            return View(model);
        }

        //Search
        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<Product>()); 
            }


            var products = _ApplicationDbContext.Products
                                   .Where(p => p.Title.Contains(query) || p.Description.Contains(query))
                                   .ToList();

            return View(products);
        }





    }
}



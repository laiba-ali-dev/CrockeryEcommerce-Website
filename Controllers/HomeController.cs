using Microsoft.AspNetCore.Mvc;
using ecommerce.DB;
using ecommerce.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ecommerce.Services;
using System.Net.Mail;
using System.Net;





namespace ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly mydbcontext context;
        private readonly EmailService _emailService;

        IWebHostEnvironment env;

        public HomeController(ILogger<HomeController> logger, mydbcontext context, IWebHostEnvironment env, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            this.context = context;
            this.env = env;
        }





        public IActionResult Index()
        {
            // Check if the user is logged in by looking for a session key
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                // User is logged in, so set session values in ViewBag
                ViewBag.Mysession = HttpContext.Session.GetString("UserSession");
                ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession");
                ViewBag.Myidsession = HttpContext.Session.GetString("UserId");
            }
            else
            {
                // User is not logged in, so do not set session-related ViewBag values
                ViewBag.Mysession = null;
                ViewBag.Mynamesession = null;
                ViewBag.Myidsession = null;
            }

            // Simply return the Index view without any redirect
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                await context.users.AddAsync(user);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Successfully";
                return RedirectToAction("Login");

            }
            return View();
        }

        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool isEmailTaken = context.users.Any(u => u.Email == email);
            return Json(new { isValid = !isEmailTaken });
        }


        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Index");

            }
            return View();

        }


        [HttpPost]
        public IActionResult Login(User user)
        {
            var Myuser = context.users.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (Myuser != null)
            {
                HttpContext.Session.SetString("UserSession", Myuser.Email);
                HttpContext.Session.SetString("UsernameSession", Myuser.Name);
                HttpContext.Session.SetString("UserId", Myuser.UserId.ToString());


                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Login Failed..";
            }
            return View();
        }



        public IActionResult Logout()
        {
            // Clear session data
            HttpContext.Session.Clear();

            // Redirect to Login page
            return RedirectToAction("Index");
        }





        public IActionResult Products()
        {
            // Retrieve UsernameSession from session, if exists
            ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession");

            // Fetch all products from the database
            var data = context.products.ToList();

            return View(data);
        }

        public IActionResult WishList()
        {
            // Check if the user is logged in by verifying the UserId session
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                // If not logged in, redirect to the Login page
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdString); // Convert UserId from session to integer

            // Retrieve wishlist data with detailed product information
            var wishlistData = context.wishlists
                .Where(w => w.UserId == userId) // Filter by logged-in user's ID
                .Join(context.products,
                                   wishlist => wishlist.ProductId,
                             product => product.ProductId,
                      (wishlist, product) => new
                      {
                          ProductId = product.ProductId,
                          WishlistId = wishlist.WishListId,
                          ProductName = product.Name,
                          ProductPrice = product.Price,
                          ProductDescription = product.Description,
                          ProductImage = product.ImagePath
                      })
                .ToList();

            // Set session values for ViewBag to display user information in the view
            ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession");
            ViewBag.Myidsession = userIdString;

            // Pass the wishlist data to the view
            return View(wishlistData);
        }


        [HttpPost]
        public IActionResult AddToWishlist(int productId)
        {
            // Get the UserId from session and convert to integer
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return Json(new { success = false, message = "Please log in to add items to your wishlist." });
            }

            int userId = int.Parse(userIdString);

            // Check if the product is already in the wishlist for the user
            var existingWishList = context.wishlists
                .FirstOrDefault(w => w.ProductId == productId && w.UserId == userId);

            if (existingWishList != null)
            {
                return Json(new { success = false, message = "This product is already in your wishlist." });
            }

            // Add the product to the wishlist
            var wishList = new WishList
            {
                ProductId = productId,
                UserId = userId
            };

            context.wishlists.Add(wishList);
            context.SaveChanges();

            return Json(new { success = true, message = "Product added to your wishlist successfully." });
        }

        [HttpPost]
        public IActionResult RemoveFromWishlist(int WishlistId)
        {
            var wishlistitem = context.wishlists.FirstOrDefault(c => c.WishListId == WishlistId);
            if (wishlistitem == null)
            {
                return Json(new { success = false, message = "Item not found in wishlist." });
            }

            context.wishlists.Remove(wishlistitem);
            context.SaveChanges();

            return Json(new { success = true });
        }



        public IActionResult GetWishlistCount()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return Json(new { count = 0 });
            }

            int userId = int.Parse(userIdString);

            // Count wishlist items for the logged-in user
            int wishlistCount = context.wishlists.Count(w => w.UserId == userId);

            return Json(new { count = wishlistCount });
        }




        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Get the UserId from session and convert to integer
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return Json(new { success = false, message = "Please log in to add items to your cart." });
            }

            int userId = int.Parse(userIdString);

            // Fetch the product price from the Product table
            var product = context.products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found." });
            }

            // Check if the product is already in the cart for the user
            var existingCartItem = context.carts
                .FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

            if (existingCartItem != null)
            {
                // If the product is already in the cart, increase the quantity
                existingCartItem.Quantity++;
                existingCartItem.TotalPrice = existingCartItem.Quantity * product.Price;  // Recalculate total price

                context.carts.Update(existingCartItem);
                context.SaveChanges();

                return Json(new { success = true, message = "Product quantity updated in your cart." });
            }

            // If the product is not in the cart, add it
            var cart = new Cart
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1,  // Default quantity is 1
                TotalPrice = product.Price // Set the total price based on the product price
            };

            context.carts.Add(cart);
            context.SaveChanges();




            return Json(new { success = true, message = "Product added to your cart successfully." });
        }


        public IActionResult Cart()
        {
            // Get the UserId from session
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                // User not logged in, redirect to login page
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdString);

            // Fetch cart items for the logged-in user
            var cartItems = context.carts
                .Where(c => c.UserId == userId)
                .Join(context.products,
                    cart => cart.ProductId,
                    product => product.ProductId,
                    (cart, product) => new CartViewModel
                    {
                        CartId = cart.CartId,
                        ProductId = product.ProductId,
                        Name = product.Name,
                        ImagePath = product.ImagePath, // Assuming Image is stored in Product table
                        Price = product.Price,
                        Quantity = cart.Quantity
                    }).ToList();


            ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession");
            ViewBag.Myidsession = userIdString;

            return View(cartItems);
        }



        [HttpPost]
        public IActionResult UpdateCartItems([FromBody] List<Cart> updates)
        {
            if (updates == null || updates.Count == 0)
            {
                return Json(new { success = false, message = "No updates received." });
            }

            decimal totalCartAmount = 0;

            foreach (var item in updates)
            {
                var cartItem = context.carts.FirstOrDefault(c => c.CartId == item.CartId);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.TotalPrice; // ✅ Total Price Update
                    totalCartAmount += cartItem.TotalPrice; // ✅ Total Cart Amount Calculate
                }
            }

            context.SaveChanges();

            return Json(new { success = true, message = "Cart updated successfully!", totalCartAmount });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int cartId)
        {
            var cartItem = context.carts.FirstOrDefault(c => c.CartId == cartId);
            if (cartItem == null)
            {
                return Json(new { success = false, message = "Item not found in cart." });
            }

            context.carts.Remove(cartItem);
            context.SaveChanges();

            return Json(new { success = true });
        }


        public IActionResult GetCartCount()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                return Json(new { count = 0 });
            }

            int userId = int.Parse(userIdString);

            // Count wishlist items for the logged-in user
            int cartcount = context.carts.Count(w => w.UserId == userId);

            return Json(new { count = cartcount });
        }




        // GET: Load the Checkout Page
        [HttpGet]
        public IActionResult Checkout()
        {
            // Check if user is logged in
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                // If not logged in, redirect to Login
                return RedirectToAction("Login");
            }

            //ViewBag.Myidsession = int.Parse(userIdString);
            ViewBag.Mynamesession = HttpContext.Session.GetString("UsernameSession");

            ViewBag.Myidsession = userIdString;// Pass UserId to View if needed
            return View();
        }








        [HttpPost]
        public IActionResult Checkout(Orders order)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                TempData["ErrorMessage"] = "You need to log in before checking out.";
                return RedirectToAction("Login");
            }

            int userId = int.Parse(userIdString);
            order.UserId = userId;
            order.OrderDate = DateTime.Now.ToString("yyyy-MM-dd");
            order.Status = "Pending";

            var cartItems = context.carts.Where(c => c.UserId == userId).ToList();
            var cartTotal = (int)cartItems.Sum(c => c.TotalPrice);
            order.TotalAmount = cartTotal.ToString();

            try
            {
                // Step 1: Save Order in Orders Table
                context.orders.Add(order);
                context.SaveChanges();

                // Step 2: Save Products in OrderDetails Table
                foreach (var item in cartItems)
                {
                    var orderDetail = new OrderDetails
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = (int)item.TotalPrice
                    };

                    context.orderdetails.Add(orderDetail);
                }
                context.SaveChanges();

                // Step 3: Clear Cart after checkout
                context.carts.RemoveRange(cartItems);
                context.SaveChanges();

                // Step 4: Fetch User Email
                var user = context.users.FirstOrDefault(u => u.UserId == userId);
                var userEmail = user?.Email;

                try
                {
                    // 🔹 Prepare Product List for Email
                    var productList = "<ul>";
                    foreach (var item in cartItems)
                    {
                        var product = context.products.FirstOrDefault(p => p.ProductId == item.ProductId);
                        if (product != null)
                        {
                            productList += $"<li>{product.Name} - {item.Quantity} x PKR {item.TotalPrice}</li>";
                        }
                    }
                    productList += "</ul>";

                    // 🔹 Configure SMTP Client
                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("laiba162491@gmail.com", "afob kauo cllo qyjf"),
                        EnableSsl = true,
                    };

                    // 🔹 Construct Email
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("laiba162491@gmail.com"),
                        Subject = "Order Confirmation",
                        Body = $"<h2>Order Confirmation</h2>" +
                               $"<p>Dear {order.FirstName} {order.LastName},</p>" +
                               $"<p>Your order has been placed successfully on {DateTime.Now:dd MMM yyyy}.</p>" +
                               $"<p><strong>Order Total:</strong> PKR {order.TotalAmount}</p>" +
                               $"<h3>Ordered Products:</h3>{productList}" +
                               $"<p>Thank you for shopping with us!</p>" +
                               $"<p>Best Regards,<br>Crockery Canvas</p>",
                        IsBodyHtml = true,
                    };

                    // 🔹 Add Recipients
                    if (!string.IsNullOrEmpty(userEmail))
                    {
                        mailMessage.To.Add(userEmail);
                    }

                    if (!string.IsNullOrEmpty(order.Email))
                    {
                        mailMessage.To.Add(order.Email);
                    }

                    // 🔹 Send Email
                    smtpClient.Send(mailMessage);
                }
                catch (Exception mailEx)
                {
                    TempData["ErrorMessage"] = "Order placed, but email not sent. Error: " + mailEx.Message;
                }

                TempData["OrderSuccess"] = "Your order has been placed!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("OrderConfirmation");
        }






        public IActionResult OrderConfirmation()
        {
            return View();
        }



        [HttpPost]
        public IActionResult SendMessage(string first_name, string email_address, string message)
        {
            // Step 1: Get UserId from Session
            var userIdString = HttpContext.Session.GetString("UserId");
            if (userIdString == null)
            {
                // Redirect to login if UserId is not in session
                return RedirectToAction("Login");
            }

            // Parse UserId
            int userId = int.Parse(userIdString);

            // Step 2: Save data to the database
            var contactMessage = new ContactMessages
            {
                UserId = userId,
                Name = first_name,
                Email = email_address,
                Message = message,
                MessageDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            context.contactmessages.Add(contactMessage);
            context.SaveChanges();

            // Step 3: Send Email
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("laiba162491@gmail.com", "afob kauo cllo qyjf"),
                    EnableSsl = true,
                };

                // Admin Email
                var adminMailMessage = new MailMessage
                {
                    From = new MailAddress("laiba162491@gmail.com"),
                    Subject = "New Contact Us Message",
                    Body = $"<h2>Contact Us Form</h2>" +
                           $"<p><strong>Name:</strong> {first_name}</p>" +
                           $"<p><strong>Email:</strong> {email_address}</p>" +
                           $"<p><strong>Message:</strong> {message}</p>",
                    IsBodyHtml = true,
                };

                adminMailMessage.To.Add("laiba162491@gmail.com"); // Replace with recipient email
                smtpClient.Send(adminMailMessage);

                // User Confirmation Email
                var userMailMessage = new MailMessage
                {
                    From = new MailAddress("laiba162491@gmail.com"),
                    Subject = "Thank You for Contacting Us",
                    Body = $"<h3>Dear {first_name},</h3>" +
                           $"<p>Thank you for reaching out to us. We have received your message:</p>" +
                           //$"<blockquote>{message}</blockquote>" +
                           $"<p>We will get back to you as soon as possible.</p>" +
                           $"<p>Best Regards,<br>Your Company Name</p>",
                    IsBodyHtml = true,
                };

                userMailMessage.To.Add(email_address); // Send to user's email
                smtpClient.Send(userMailMessage);

                ViewBag.Message = "Your message has been sent successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Message could not be sent. Error: " + ex.Message;
            }

            return View("ContactUs");
        }



        public IActionResult ContactUs()
        {
            return View();
        }




        //public IActionResult MyAccount()
        //{
        //    // Assuming you are using session to store logged-in user info
        //    var userIdString = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userIdString))
        //    {
        //        // Redirect to login if UserId is not in session
        //        return RedirectToAction("Login");
        //    }

        //    // Convert UserId from string to int
        //    if (!int.TryParse(userIdString, out int userId))
        //    {
        //        // Handle invalid UserId in session
        //        return RedirectToAction("Login");
        //    }

        //    var orders = context.orders
        //        .Where(o => o.UserId == userId)
        //        .ToList();

        //    return View(orders); // Pass orders to the view
        //}

        public IActionResult MyAccount()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login");
            }

            var orders = context.orders
                .Where(o => o.UserId == userId)
                .ToList();

            var user = context.users
                .FirstOrDefault(u => u.UserId == userId); // Replace `users` with your actual Users table

            var viewModel = new MyAccountViewModel
            {
                orders = orders,
                UserDetails = user
            };

            return View(viewModel);
        }



        [HttpGet]
        public IActionResult GetFilteredProducts(decimal minPrice, decimal maxPrice)
        {
            var filteredProducts = context.products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToList();

            return Json(filteredProducts);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult OrderItems()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }


    }
}

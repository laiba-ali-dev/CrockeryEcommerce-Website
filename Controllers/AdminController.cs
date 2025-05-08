using ecommerce.DB;
using ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
namespace ecommerce.Controllers
{
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;
        private readonly mydbcontext context;
        IWebHostEnvironment env;

        public AdminController(ILogger<AdminController> logger, mydbcontext context, IWebHostEnvironment env)
        {
            _logger = logger;
            this.context = context;
            this.env = env;
        }

      
      

      
        //ADMIN (REGISTER,LOGIN TO REDICRECT ON ADMINDASHBOARD) START

        public IActionResult AdminRegister()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AdminRegister(Admins user)
        {
            if (ModelState.IsValid)
            {
                await context.admins.AddAsync(user);
                await context.SaveChangesAsync();
                TempData["Success"] = "Registered Successfully";
                return RedirectToAction("AdminLogin");

            }
            return View();
        }


      
        public IActionResult AdminLogin()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                return RedirectToAction("AdminDashboard");

            }
            return View();

        }


        [HttpPost]
        public IActionResult AdminLogin(Admins user)
        {
            var Myuser = context.admins.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (Myuser != null)
            {
                HttpContext.Session.SetString("AdminSession", Myuser.Email);
                HttpContext.Session.SetString("Adminnamesession", Myuser.Username);
                HttpContext.Session.SetString("AdminId", Myuser.AdminId.ToString());


                return RedirectToAction("AdminDashboard");
            }
            else
            {
                ViewBag.Message = "Login Failed..";
            }
            return View();
        }

        public IActionResult AdminDashboard()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mysession = HttpContext.Session.GetString("AdminSession").ToString();
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                ViewBag.Myidsession = HttpContext.Session.GetString("AdminId").ToString();


                // Database se Users, Products aur Orders ka count fetch karna
                ViewBag.TotalUsers = context.users.Count();  // Total users
                ViewBag.TotalProducts = context.products.Count();  // Total products
                ViewBag.TotalOrders = context.orders.Count();  // Total orders
                ViewBag.TotalMessages = context.contactmessages.Count();  // Total messages
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
            return View();
        }

        public IActionResult AdminLogout()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                HttpContext.Session.Remove("AdminSession");
                HttpContext.Session.Remove("Adminnamesession");
                HttpContext.Session.Remove("AdminId");

                return RedirectToAction("AdminLogin");

            }
            return View();
        }

        //ADMIN (REGISTER,LOGIN TO REDICRECT ON ADMINDASHBOARD) FINISH

        ///////////                     

        //ADMIN (ADD PRODUCTS , EDIT PRODUCTS , DELETE PRODUCTS) START

     
        public IActionResult ProductsIndex()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

                var data = context.products.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }
      
        public IActionResult AddProducts()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                return View();
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult AddEmployee(ProductsViewModel emp)
        {
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                return RedirectToAction("AdminLogin");
            }

            string fileName = "";
            if (emp.Photo != null)
            {
                var ext = Path.GetExtension(emp.Photo.FileName);
                var size = emp.Photo.Length;
                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {

                    if (size <= 1000000)//1 MB
                    {

                        string folder = Path.Combine(env.WebRootPath, "images");
                        fileName = Guid.NewGuid().ToString() + "_" + emp.Photo.FileName;
                        string filePath = Path.Combine(folder, fileName);
                        emp.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                        Products p = new Products()
                        {
                            Name = emp.Name,
                            Description = emp.Description,
                            Price = emp.Price,
                            Stock = emp.Stock,
                            ImagePath = fileName,
                            Category = emp.Category,
                            CreateDate = emp.CreateDate
                        };
                        context.products.Add(p);
                        context.SaveChanges();
                        TempData["Success"] = "Employee Added..";
                        return RedirectToAction("ProductsIndex");
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image must be less than 1 MB";
                    }
                }
                else
                {
                    TempData["Ext_Error"] = "Only PNG, JPG, JPEG iamges allowed";
                }

            }

            return View();
        }



      

        public async Task<IActionResult> EditProducts(int? Id)
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                if (Id == null || context.products == null)
                {
                    return NotFound();
                }
                var data = await context.products.FindAsync(Id);
                if (data == null)
                {
                    return NotFound();
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProducts(int? Id, Products emp)
        {
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                return RedirectToAction("AdminLogin");
            }

            if (Id != emp.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                context.products.Update(emp);
                TempData["update_success"] = "updated..";
                await context.SaveChangesAsync();
                return RedirectToAction("ProductsIndex", "Admin");
            }

            return View();
        }



        public async Task<IActionResult> DeleteProducts(int? Id)
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

                if (Id == null || context.products == null)
                {
                    return NotFound();
                }
                var data = await context.products.FirstOrDefaultAsync(x => x.ProductId == Id);
                if (data == null)
                {
                    return NotFound();
                }
                return View(data);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }


        [HttpPost, ActionName("DeleteProducts")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int? Id)
        {
            if (HttpContext.Session.GetString("AdminSession") == null)

            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();
                return RedirectToAction("AdminLogin");
            }
            var data = await context.products.FindAsync(Id);
            if (data != null)
            {
                context.products.Remove(data);
            }
            await context.SaveChangesAsync();
            TempData["delete_success"] = "deleted..";

            return RedirectToAction("ProductsIndex", "Admin");

        }


        //ADMIN (ADD PRODUCTS , EDIT PRODUCTS , DELETE PRODUCTS) FINISH

        /////////// 
        public IActionResult Forms()
		{
			return View();
		}

        public IActionResult UsersList()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

                var data = context.users.ToList();
                return View(data);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }

        public IActionResult OrderList()
        {
            if (HttpContext.Session.GetString("AdminSession") != null)
            {
                ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

                // Exclude both "Delivered" and "Cancelled" orders
                var data = context.orders
                                  .Where(o => o.Status != "Delivered" && o.Status != "Cancelled")
                                  .ToList();

                return View(data);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }
        }







        //    public IActionResult UpdateOrderStatus(int id, string status)
        // {
        //if (HttpContext.Session.GetString("AdminSession") != null)
        //{
        //var order = context.orders.FirstOrDefault(o => o.OrderId == id);
        // if (order != null)
        //{
        //    order.Status = status; // Update the status
        //     context.SaveChanges(); // Save changes in the database
        //  }
        //  TempData["update_success"] = $"Order status updated to {status}!";
        //  return RedirectToAction("OrderList");
        //   }
        //  else
        //  {
        //      return RedirectToAction("AdminLogin");
        //  }
        //   }

        public IActionResult DeliveredOrders()
        {
            if (HttpContext.Session.GetString("AdminSession") == null)
            {
                return RedirectToAction("AdminLogin");
            }

            ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession");

            var deliveredOrders = context.orders.Where(o => o.Status == "Delivered").ToList();
            return View(deliveredOrders);
        }

        public IActionResult CancelledOrders()
		{
			if (HttpContext.Session.GetString("AdminSession") != null)
			{
				ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

				// Filter orders with "Cancelled" status
				var data = context.orders.Where(o => o.Status == "Cancelled").ToList();
				return View(data);
			}
			else
			{
				return RedirectToAction("AdminLogin");
			}
		}

        [HttpGet]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            var order = context.orders.FirstOrDefault(o => o.OrderId == id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found!";
                return RedirectToAction("OrderList");
            }

            order.Status = status;
            context.SaveChanges();

            var user = context.users.FirstOrDefault(u => u.UserId == order.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("OrderList");
            }

            var userEmail = user.Email;
            string subject = "";
            string body = "";

            // 📩 **Set Email Content Based on Status**
            if (status == "Delivered")
            {
                subject = "🎉 Your Order Has Been Delivered!";
                body = $"<h2>Order Delivered ✅</h2>" +
                       $"<p>Dear {order.FirstName} {order.LastName},</p>" +
                       $"<p>We are happy to inform you that your order <strong>(Order ID: {order.OrderId})</strong> has been successfully <strong>delivered</strong>.</p>" +
                       $"<p>We hope you enjoy your purchase! Thank you for shopping with us.</p>" +
                       $"<p>Best Regards,<br><strong>Crockery Canvas</strong></p>";
            }
            else if (status == "Cancelled")
            {
                subject = "⚠️ Your Order Has Been Cancelled";
                body = $"<h2>Order Cancelled ❌</h2>" +
                       $"<p>Dear {order.FirstName} {order.LastName},</p>" +
                       $"<p>We regret to inform you that your order <strong>(Order ID: {order.OrderId})</strong> has been <strong>cancelled</strong>.</p>" +
                       $"<p>If this was a mistake or you have any questions, please contact our support team.</p>" +
                       $"<p>Best Regards,<br><strong>Crockery Canvas</strong></p>";
            }
            else if (status == "On the Way")
            {
                subject = "🚚 Your Order is On The Way!";
                body = $"<h2>Order On The Way 🚚</h2>" +
                       $"<p>Dear {order.FirstName} {order.LastName},</p>" +
                       $"<p>Your order <strong>(Order ID: {order.OrderId})</strong> is now <strong>on the way</strong> and will be delivered soon.</p>" +
                       $"<p>Track your order for live updates.</p>" +
                       $"<p>Best Regards,<br><strong>Crockery Canvas</strong></p>";
            }

            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("laiba162491@gmail.com", "afob kauo cllo qyjf"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("laiba162491@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                if (!string.IsNullOrEmpty(userEmail))
                {
                    mailMessage.To.Add(userEmail);
                }

                smtpClient.Send(mailMessage);
            }
            catch (Exception mailEx)
            {
                TempData["ErrorMessage"] = "Status updated, but email not sent. Error: " + mailEx.Message;
            }

            TempData["Success"] = "Order status updated successfully & email sent!";
            return RedirectToAction("OrderList");
        }







        public IActionResult Contactmessages()
        {
			if (HttpContext.Session.GetString("AdminSession") != null)
			{
				ViewBag.Mynamesession = HttpContext.Session.GetString("Adminnamesession").ToString();

				var data = context.contactmessages.ToList();
				return View(data);
			}
			else
			{
				return RedirectToAction("AdminLogin");
			}
		}
      


    }
}

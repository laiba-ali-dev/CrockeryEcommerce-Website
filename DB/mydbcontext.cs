using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ecommerce.Models;




namespace ecommerce.DB
{
    public class mydbcontext : DbContext
    {
        public mydbcontext()
        {
        }
        public mydbcontext(DbContextOptions<mydbcontext> options) : base(options)
        {

        }

        public virtual DbSet<Cart> carts { get; set; }

        public virtual DbSet<ContactMessages> contactmessages { get; set; }


        

        public virtual DbSet<Orders> orders { get; set; }

        public virtual DbSet<Products> products { get; set; }

        public virtual DbSet<User> users { get; set; }

        public virtual DbSet<WishList> wishlists { get; set; }

        public virtual DbSet<Admins> admins { get; set; }

        public virtual DbSet<Feedback> feedback { get; set; }

        public virtual DbSet<Reviews> reviews { get; set; }


        public virtual DbSet<OrderDetails> orderdetails { get; set; }

        public virtual DbSet<BillingDetails> billingdetails { get; set; }

       


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure precision for TotalPrice in Cart table
            modelBuilder.Entity<Cart>()
                .Property(c => c.TotalPrice)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            // Configure precision for TotalAmount in Checkout table
           // modelBuilder.Entity<Checkout>()
               // .Property(c => c.TotalAmount)
               // .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            // Add other configurations here if needed

            base.OnModelCreating(modelBuilder);
        }


    }
}

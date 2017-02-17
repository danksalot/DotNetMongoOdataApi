using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace AzureMongoApi.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.Address> Addresses { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.Customer> Customers { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.CustomerAddress> CustomerAddresses { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.ProductCategory> ProductCategories { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.ProductModel> ProductModels { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.ProductDescription> ProductDescriptions { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.ProductModelProductDescription> ProductModelProductDescriptions { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.SalesOrderDetail> SalesOrderDetails { get; set; }

        public System.Data.Entity.DbSet<AzureMongoApi.Models.SalesOrderHeader> SalesOrderHeaders { get; set; }
    }
}
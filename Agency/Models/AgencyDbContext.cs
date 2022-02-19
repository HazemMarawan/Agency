using System;
using System.Data.Entity;
using System.Linq;

namespace Agency.Models
{
    public class AgencyDbContext : DbContext
    {
        // Your context has been configured to use a 'DbContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Agency.Models.DbContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'DbContext' 
        // connection string in the application configuration file.
        public AgencyDbContext()
            : base("name=DbContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Hotel> Hotels { get; set; }
        public virtual DbSet<HotelImage> HotelImages { get; set; }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<EventHotel> EventHotels { get; set; }

        public virtual DbSet<HotelFacilitie> HotelFacilities { get; set; }
        public virtual DbSet<EventHotelBenefit> EventHotelBenefits { get; set; }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Client> Clients { get; set; }

        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<ReservationDetail> ReservationDetails { get; set; }
        public virtual DbSet<ReservationComment> ReservationComments { get; set; }
        public virtual DbSet<ReservationTask> ReservationTasks { get; set; }

        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<HotelLocation> HotelLocations { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<ReservationLog> ReservationLogs { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<Email> Emails { get; set; }
        public virtual DbSet<EmailAttachment> EmailAttachments { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<PermissionGroup> PermissionGroups { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserTask> UserTasks { get; set; }
        public virtual DbSet<InitialReservation> InitialReservations { get; set; }
        public virtual DbSet<ReservationCreditCard> ReservationCreditCards { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<MailServer> MailServers { get; set; }
        
    }
}
using EventManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace EventManagement.Models;


public enum EventType
{
    Private,
    Cultural,
    Concert
}
public enum FoodType
{
    Vegetarian,
    NonVegetarian,
    Mixed,
    Jain,
    None
}
public class Event
{
    [Display(Name = "Event ID")]
    public int EventId { get; set; }

    [Display(Name = "Event Type")]
    public EventType EventType { get; set; }

    [Display(Name = "Event Name")]
    [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Event Name must be between 1 and 100 characters.")]
    public string EventName { get; set; }

    [Display(Name = "Description")]
    [StringLength(maximumLength: 500, ErrorMessage = "Description can be at most 500 characters.")]
    public string Description { get; set; }

    [Display(Name = "Date")]
    public DateTime Date { get; set; }

    [Display(Name = "Contact Information")]
    [StringLength(maximumLength: 200, ErrorMessage = "Contact Information can be at most 200 characters.")]
    [RegularExpression(@"^[\w\s]+ - \d{10}$", ErrorMessage="Enter Valid Contact Information")]
    public string ContactInformation { get; set; }

    [Display(Name = "Duration")]
    [StringLength(maximumLength: 50, ErrorMessage = "Duration can be at most 50 characters.")]
    public string Duration { get; set; }

    [Display(Name = "Hall ID")]
    public int HallId { get; set; }

    [Display(Name = "Event Hall")]
    public virtual Hall Hall { get; set; }

    [Display(Name = "Tickets")]
    public ICollection<Ticket>? Tickets { get; set; }

    [Display(Name = "Users")]
    public ICollection<ApplicationUser> Users { get; set; }

    public bool IsEventFinished { get; set; }
}

public class AdminEvent : Event
{
    [StringLength(maximumLength: 255, ErrorMessage = "Maximum length is 255 characters.")]
    [Display(Name = "Artist")]
    public string? ArtistData { get; set; }

    [Display(Name = "Display")]
    [StringLength(maximumLength: 255, ErrorMessage = "Maximum length is 255 characters.")]
    public string ImagePath { get; set; }

    [Display(Name = "Event Price")]
    public double EventPrice { get; set; }

    [Display(Name = "Featured")]
    public bool Featured { get; set; }
}

public class PrivateEvent : Event
{
    [Display(Name = "Number of Guests")]
    public int NumberOfGuests { get; set; }

    [Display(Name = "Food Type")]
    public FoodType FoodType { get; set; }
}

public class Ticket
{
    [Display(Name = "Ticket ID")]
    public int TicketId { get; set; }

    [Display(Name = "Event ID")]
    public int EventId { get; set; }

    [Display(Name = "Event")]
    public virtual Event Event { get; set; }

    [Display(Name = "Ticket Price")]
    public double Price { get; set; }

    [Display(Name = "Number of Tickets")]
    public int? NumberOfTickets { get; set; }

    [Display(Name = "User ID")]
    public string UserId { get; set; }

    [Display(Name = "User")]
    public virtual ApplicationUser User { get; set; }
}

public class Hall
{
    [Display(Name = "Hall ID")]
    [Required(ErrorMessage = "Please choose a hall")]
    public int HallId { get; set; }

    [Display(Name = "Hall Name")]
    [StringLength(maximumLength: 100, MinimumLength = 1, ErrorMessage = "Hall Name must be between 1 and 100 characters.")]
    public string HallName { get; set; }

    [Display(Name = "Venue")]
    [StringLength(maximumLength: 200, ErrorMessage = "Venue can be at most 200 characters.")]
    public string Venue { get; set; }

    [Display(Name = "Hall Price")]
    public double HallPrice { get; set; }

    [Display(Name = "Location")]
    [StringLength(maximumLength: 200, ErrorMessage = "Location can be at most 200 characters.")]
    public string Location { get; set; }

    [Display(Name = "Guest Limit")]
    public int GuestLimit { get; set; }

    [Display(Name = "Events")]
    public ICollection<Event>? Events { get; set; }
}


public class EventManagementContext : IdentityDbContext<ApplicationUser>
{

    public DbSet<Event> Events { get; set; }
    // public DbSet<CulturalEvent> CulturalEvents { get; set; }
    public DbSet<AdminEvent> AdminEvents { get; set; }
    public DbSet<PrivateEvent> PrivateEvents { get; set; }
    //public DbSet<Artist> Artists { get; set; }
    //public DbSet<Food> Foods { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    // public DbSet<Rating> Ratings { get; set; }
    public EventManagementContext(DbContextOptions<EventManagementContext> options)
        : base(options)
    {
        //Database.Migrate(); // Apply any pending migrations during application startup
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.Entity<Event>()
                .Property(e => e.EventId)
                .ValueGeneratedOnAdd();
        builder.Entity<ApplicationUser>().Property(u => u.Id).HasColumnName("UserId");

        builder.Entity<ApplicationUser>().Property(u => u.PhoneNumber).HasMaxLength(10);

        builder.Entity<Event>()
        .HasDiscriminator<string>("Discriminator")
        .HasValue<PrivateEvent>("PrivateEvent")
        .HasValue<AdminEvent>("AdminEvent");
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}

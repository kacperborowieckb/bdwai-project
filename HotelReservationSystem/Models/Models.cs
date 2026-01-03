using System.ComponentModel.DataAnnotations;

namespace HotelReservationSystem.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public decimal BasePrice { get; set; }

        public virtual ICollection<Room>? Rooms { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public required string RoomNumber { get; set; }

        public int RoomTypeId { get; set; }

        [Required]
        public required virtual RoomType RoomType { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class Guest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public virtual ICollection<Reservation>? Reservations { get; set; }
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int RoomId { get; set; }
        public virtual Room? Room { get; set; }

        public int GuestId { get; set; }
        public virtual Guest? Guest { get; set; }
    }
}
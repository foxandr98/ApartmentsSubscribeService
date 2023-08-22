using System.ComponentModel.DataAnnotations.Schema;

namespace ApartmentsSubscribeService.Model
{
    [Table("users_apartments")]
    public class UsersApartments
    {
        public User User { get; set; } = null!;

        [Column("user_id")]
        public int UserId { get; set; }

        public Apartment Apartment { get; set; } = null!;

        [Column("apartment_id")]
        public int ApartmentId { get; set; }    
    }
}

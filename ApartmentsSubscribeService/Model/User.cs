using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApartmentsSubscribeService.Model
{
    [Table("users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public User(string email)
        {
            Email = email;
        }
        public User() { }

        public List<Apartment> Apartments { get; set; } = null!;
        public List<UsersApartments> UsersApartments { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return obj is User user &&
                   Email == user.Email;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Email);
        }
    }
}

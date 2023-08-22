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
        public string Email { get; set; } = null!;

        public List<Apartment> Apartments { get; set; } = new();
        public List<UsersApartments> UsersApartments { get; set; } = new();
    }
}

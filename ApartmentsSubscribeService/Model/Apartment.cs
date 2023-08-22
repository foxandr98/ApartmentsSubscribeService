using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApartmentsSubscribeService.Model
{
    [Table("apartments")]
    public class Apartment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Url { get; set; } = null!;

        public List<User> Users { get; set; } = null!;
        public List<UsersApartments> UsersApartments { get; set; } = null!;
    }
}

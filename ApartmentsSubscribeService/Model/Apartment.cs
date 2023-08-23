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
        [Url]
        public string Url { get; set; } = null!;

        public Apartment(string url)
        {
            Url = url;
        }
        public List<User> Users { get; set; } = null!;
        public List<UsersApartments> UsersApartments { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            return obj is Apartment apartment &&
                   Url == apartment.Url;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Url);
        }
    }
}

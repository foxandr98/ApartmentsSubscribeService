using System.ComponentModel.DataAnnotations;

namespace ApartmentsSubscribeService.Model.DTO
{
    public class SubscribeDTO
    {
        [Required(ErrorMessage = "Email не может быть пустым!")]
        [EmailAddress(ErrorMessage = "Ваш email имеет некорректный формат!")]
        [MaxLength(100, ErrorMessage = "Длина email не может быть длинее 100 символов!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "URL не может быть пустым!")]
        [Url(ErrorMessage = "Ваш URL имеет некорректный формат!")]
        [MaxLength(100, ErrorMessage = "Длина ссылки не может быть длинее 100 символов!")]
        public string Url { get; set; } = null!;
    }
}

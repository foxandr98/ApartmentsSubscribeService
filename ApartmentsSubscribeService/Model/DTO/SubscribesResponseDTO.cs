namespace ApartmentsSubscribeService.Model.DTO
{
    public class SubscribesResponseDTO
    {
        public string Email { get; set; } = null!;
        public Dictionary<string, string> PricesByUrl { get; set; } = null!;

        public SubscribesResponseDTO(string email, Dictionary<string, string> pricesByUrl)
        {
            Email = email;
            PricesByUrl = pricesByUrl;
        }
    }
}

namespace ApartmentsSubscribeService.Model.DTO
{
    public class SubscribesResponseDTO
    {
        public string Email { get; set; } = null!;
        public Dictionary<string, uint> PricesByUrl { get; set; } = null!;

        public SubscribesResponseDTO(string email, Dictionary<string, uint> pricesByUrl)
        {
            Email = email;
            PricesByUrl = pricesByUrl;
        }
    }
}

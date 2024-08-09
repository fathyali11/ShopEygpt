

namespace Web.Entites.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        // Current User
        public string UserId {  get; set; }
        [ValidateNever]
        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUser { get; set; }
        // User Data
        public string? Name {  get; set; }
        public string? City {  get; set; }
        public string? PhoneNumber {  get; set; }
        public string? Email { get; set; }
        // Ordre Data
        public DateTime CreatedDate { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public string OrderStatus {  get; set; }
        public string PaymentStatus { get; set; }

        // Carrier Data
        public string? TrackNumber {  get; set; }
        public string? Carrier {  get; set; }
        // Stripe
        public string? SessionId {  get; set; }
        public string? PaymentIntentId {  get; set; }


        public decimal TotalPrice {  get; set; }

    }
}

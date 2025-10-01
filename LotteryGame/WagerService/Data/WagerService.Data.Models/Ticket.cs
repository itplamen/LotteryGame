namespace WagerService.Data.Models
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Ticket : BaseEntity
    {
        public string TicketNumber { get; set; }

        public int PlayerId { get; set; }

        public string DrawId { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Amount { get; set; }

        public int ReservationId { get; set; }

        public TicketStatus Status { get; set; }
    }
}

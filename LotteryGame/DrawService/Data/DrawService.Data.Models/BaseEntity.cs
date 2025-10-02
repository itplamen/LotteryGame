namespace DrawService.Data.Models
{
    using CouchDB.Driver.Types;

    public abstract class BaseEntity : CouchDocument
    {
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
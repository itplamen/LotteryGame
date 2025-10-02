namespace DrawService.Data.Models
{
    using Newtonsoft.Json;

    public abstract class BaseEntity
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("_rev")]
        public string Rev { get; set; }
    }
}
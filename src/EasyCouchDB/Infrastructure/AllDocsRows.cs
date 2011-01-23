using JsonFx.Json;

namespace EasyCouchDB.Infrastructure
{
    public class AllDocsRows<TDocument>
    {
        [JsonName("doc")]
        public TDocument Document { get; set; }
    }
}
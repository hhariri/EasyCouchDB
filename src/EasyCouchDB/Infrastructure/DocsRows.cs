using JsonFx.Json;

namespace EasyCouchDB.Infrastructure
{
    public class DocsRows<TDocument>
    {
        [JsonName("value")]
        public TDocument Document { get; set; }
    }
}
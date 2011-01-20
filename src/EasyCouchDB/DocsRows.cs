using JsonFx.Json;

namespace EasyCouchDB
{
    public class DocsRows<TDocument>
    {
        [JsonName("value")]
        public TDocument Document { get; set; }
    }
}
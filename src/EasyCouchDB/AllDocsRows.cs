using JsonFx.Json;

namespace EasyCouchDB
{
    public class AllDocsRows<TDocument> 
    {
        [JsonName("doc")]
        public TDocument Document { get; set; }
    }
}
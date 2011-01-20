using JsonFx.Json;

namespace EasyCouchDB
{
    public class Rows<TDocument> 
    {
        [JsonName("doc")]
        public TDocument Document { get; set; }
    }
}
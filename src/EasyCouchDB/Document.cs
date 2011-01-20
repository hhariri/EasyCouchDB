using System.ComponentModel;
using JsonFx.Json;

namespace EasyCouchDB
{
    public class Document<TKey> : IDocument<TKey>
    {
        [JsonName("_id")]
        public TKey Id { get; set; }
        [JsonName("_rev")]
        [DefaultValue("")]
        public string Revision { get; set; }
        [JsonName("internalDocType")]
        [DefaultValue("")]
        public string DocumentType { get; set; }
    }
}
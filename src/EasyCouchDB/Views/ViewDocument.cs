using JsonFx.Json;

namespace EasyCouchDB.Views
{
    public class ViewDocument : Document<string>
    {
        public ViewDocument(string designDocName)
        {
            Id = string.Format("_design/easycouchdb_{0}", designDocName);
            Language = "javascript";
        }

        [JsonName("language")]
        public string Language { get; set; }

        [JsonName("views")]
        public Views Views { get; set; }
    }
}
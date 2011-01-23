using JsonFx.Json;

namespace EasyCouchDB.Views
{
    public class MapReduce
    {
        public MapReduce(string map, string reduce)
        {
            Map = string.Format("function (doc) {{{0}}}", map);
            Reduce = string.IsNullOrEmpty(reduce) ? null : string.Format("function (keys, values) {{{0}}}", reduce);
        }

        [JsonName("map")]
        public string Map { get; set; }

        [JsonName("reduce")]
        public string Reduce { get; set; }
    }
}
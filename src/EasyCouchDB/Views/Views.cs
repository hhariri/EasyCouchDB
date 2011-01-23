using JsonFx.Json;

namespace EasyCouchDB.Views
{
    public class Views
    {
        public Views(MapReduce mapReduce)
        {
            MapReduce = mapReduce;
        }

        [JsonName("mapreduce")]
        public MapReduce MapReduce { get; set; }
    }
}
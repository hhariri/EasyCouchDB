namespace EasyCouchDB
{
    public class MultiRowResponseWrapper<TDocument> 
    {
        public int TotalRows { get; set; }
        public Rows<TDocument>[] Rows { get; set; }
    }
}
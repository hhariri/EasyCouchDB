namespace EasyCouchDB
{
    public class MultiRowResponseWrapperForAllDocs<TDocument> 
    {
        public int TotalRows { get; set; }
        public AllDocsRows<TDocument>[] Rows { get; set; }
    }
}
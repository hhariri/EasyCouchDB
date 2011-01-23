namespace EasyCouchDB.Infrastructure
{
    public class MultiRowResponseWrapperForDocs<TDocument>
    {
        public int TotalRows { get; set; }
        public DocsRows<TDocument>[] Rows { get; set; }
    }
}
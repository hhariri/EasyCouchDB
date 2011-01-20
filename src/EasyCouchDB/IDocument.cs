namespace EasyCouchDB
{
    public interface IDocument<TId>
    {
        TId Id { get; set; }
        string Revision { get; set; }
    }
}
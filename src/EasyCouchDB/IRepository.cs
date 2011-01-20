namespace EasyCouchDB
{
    public interface IRepository<TDocument>
    {
        string Save(TDocument document);
    }
}
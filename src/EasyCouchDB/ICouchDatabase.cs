using System.Collections.Generic;

namespace EasyCouchDB
{
    public interface ICouchDatabase<TDocument, TId> where TDocument : class, IDocument<TId>
    {
        string Save(TDocument document);
        TDocument GetDocument(TId id);
        void DeleteDocument(TId id);
        IEnumerable<dynamic> GetAllDocuments();
        IEnumerable<TDocument> GetDocuments();
    }
}
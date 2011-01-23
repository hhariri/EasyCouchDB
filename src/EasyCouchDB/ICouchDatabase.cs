using System.Collections.Generic;

namespace EasyCouchDB
{
    public interface ICouchDatabase<TDocument, TId> where TDocument : class, IDocument<TId>
    {
        string Save(TDocument document);
        TDocument Load(TId id);
        void Delete(TId id);
        IEnumerable<dynamic> GetAllDocuments();
        IEnumerable<TDocument> GetDocuments();
        void SaveAttachment(TId id, string filename, string imageJpeg);
        void DeleteAttachment(TId id, string attachmentName);
    }
}
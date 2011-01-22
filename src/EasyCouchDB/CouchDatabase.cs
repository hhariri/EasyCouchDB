using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using EasyCouchDB;
using EasyHttp.Http;

namespace EasyCouchDB
{
    public class CouchDatabase<TDocument, TId>: ICouchDatabase<TDocument, TId> where TDocument: class, IDocument<TId>
    {
        readonly CouchServer _couchServer;

        public CouchDatabase(CouchServer couchServer)
        {
            _couchServer = couchServer;
        }

        public string Save(TDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            document.DocumentType = typeof (TDocument).Name;

            if (document.Id != null)
            {
                var getResponse = _couchServer.Get(document.Id.ToString());

                if (getResponse.StatusCode == HttpStatusCode.OK)
                {
                    // It's an update
                    var currentDocument = getResponse.StaticBody<TDocument>();

                    if (currentDocument.Revision != document.Revision)
                    {
                        throw new DocumentConflictException();
                    }

                    document.Revision = currentDocument.Revision;
                } 
                var response = _couchServer.Put(document.Id.ToString(), document);

                return response.DynamicBody.id;
            }
            // It's a new insert with auto-assign
            var postResponse = _couchServer.Post("/", document);

            return postResponse.DynamicBody.id;

        }

        public TDocument Load(TId id)
        {

            var response = _couchServer.Get(id.ToString());
            
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return response.StaticBody<TDocument>();
            }

            throw new DocumentNotFoundException("id");
        }

        public void Delete(TId id)
        {
            var response  = _couchServer.Head(id.ToString());

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException();
            }


            response = _couchServer.Delete(String.Format("{0}?rev={1}", id, response.ETag));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException();
            }
        }

        public IEnumerable<dynamic> GetAllDocuments()
        {
            var response = _couchServer.Get("_all_docs?include_docs=true");

            var wrapper = response.StaticBody<MultiRowResponseWrapperForAllDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        public IEnumerable<TDocument> Documents()
        {
            var response = _couchServer.Head("_design/easycouchdb_views/_view/all");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Save 
                CreateView();

                response = _couchServer.Get("_design/easycouchdb_views/_view/all");
            }


            var wrapper = response.StaticBody<MultiRowResponseWrapperForDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        public void SaveAttachment(TId id, string filename, string contentType)
        {

            var response = _couchServer.Head(id.ToString());

            string url;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var revision = response.ETag;

                url = String.Format("{0}/{1}?rev={2}", id, Path.GetFileName(filename), revision);
            }
            else
            {
                url = String.Format("{0}/{1}", id, Path.GetFileName(filename));
            }

            response = _couchServer.PutFile(url, filename, contentType);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new AttachmentException(response.StatusDescription);   
            }
        }

        public void DeleteAttachment(TId id, string attachmentName)
        {
            var response = _couchServer.Head(id.ToString());

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new AttachmentException(response.StatusDescription);
            }

            var url = String.Format("{0}/{1}?rev={2}", id, Path.GetFileName(attachmentName), response.ETag);

            response = _couchServer.Delete(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new AttachmentException(response.StatusDescription);
            }

        }



        void CreateView()
        {
            dynamic mappingFunction = new ExpandoObject();

            mappingFunction.map = "function (doc) { if (doc.internalDocType=='" + typeof(TDocument).Name + "') { emit(doc.id,doc);}}";
            dynamic mapping = new ExpandoObject();

            mapping.all = mappingFunction;

            dynamic viewDocument = new ExpandoObject();

            viewDocument._id = "_design/easycouchdb_views";
            viewDocument.language = "javascript";
            viewDocument.views = mapping;

            _couchServer.Put("_design/easycouchdb_views", viewDocument);
        }
    }
}
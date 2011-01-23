using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using EasyCouchDB.Infrastructure;
using EasyCouchDB.Views;
using EasyHttp.Http;

namespace EasyCouchDB
{
    public class CouchDatabase<TDocument, TId> : ICouchDatabase<TDocument, TId> where TDocument : class, IDocument<TId>
    {
        readonly ICouchServer _server;
        readonly IViewManager _viewManager;

        public CouchDatabase(ICouchServer server) : this(server, new ViewManager(server))
        {
        }

        public CouchDatabase(ICouchServer server, IViewManager viewManager)
        {
            _server = server;
            _viewManager = viewManager;
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
                HttpResponse getResponse = _server.Get(document.Id.ToString());

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
                HttpResponse response = _server.Put(document.Id.ToString(), document);

                if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
                {
                    throw new DocumentUpdateException(response.StatusDescription);
                }
                return response.DynamicBody.id;
            }
            // It's a new insert with auto-assign
            HttpResponse postResponse = _server.Post("/", document);

            if (postResponse.StatusCode != HttpStatusCode.Created && postResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentUpdateException(postResponse.StatusDescription);
            }

            return postResponse.DynamicBody.id;
        }

        public TDocument Load(TId id)
        {
            HttpResponse response = _server.Get(id.ToString());

            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return response.StaticBody<TDocument>();
            }

            throw new DocumentNotFoundException(id.ToString());
        }

        public void Delete(TId id)
        {
            HttpResponse response = _server.Head(id.ToString());

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException();
            }


            response = _server.Delete(String.Format("{0}?rev={1}", id, response.ETag));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException(id.ToString());
            }
        }

        public IEnumerable<dynamic> GetAllDocuments()
        {
            HttpResponse response = _server.Get("_all_docs?include_docs=true");

            var wrapper = response.StaticBody<MultiRowResponseWrapperForAllDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        public IEnumerable<TDocument> GetDocuments()
        {
            if (!_viewManager.ViewExists("all"))
            {
                _viewManager.CreateView("all",
                                        "if (doc.internalDocType=='" + typeof (TDocument).Name +
                                        "') { emit(doc.id,doc);}");
            }

            return _viewManager.ExecuteView<TDocument>("all");
        }

        public void SaveAttachment(TId id, string filename, string contentType)
        {
            HttpResponse response = _server.Head(id.ToString());

            string url;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string revision = response.ETag;

                url = String.Format("{0}/{1}?rev={2}", id, Path.GetFileName(filename), revision);
            }
            else
            {
                url = String.Format("{0}/{1}", id, Path.GetFileName(filename));
            }

            response = _server.PutFile(url, filename, contentType);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new AttachmentException(response.StatusDescription);
            }
        }

        public void DeleteAttachment(TId id, string attachmentName)
        {
            HttpResponse response = _server.Head(id.ToString());

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new AttachmentException(response.StatusDescription);
            }

            string url = String.Format("{0}/{1}?rev={2}", id, Path.GetFileName(attachmentName), response.ETag);

            response = _server.Delete(url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new AttachmentException(response.StatusDescription);
            }
        }


    }
}
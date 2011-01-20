﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using EasyHttp.Http;

namespace EasyCouchDB
{
    public class CouchDatabase<TDocument, TId>: ICouchDatabase<TDocument, TId> where TDocument: class, IDocument<TId>
    {
        readonly string _baseUrl;
        readonly HttpClient _httpClient;


        public CouchDatabase(string host, int port, string database)
        {
            _baseUrl = String.Format("http://{0}:{1}/{2}", host, port, database);

            _httpClient = new HttpClient();

            _httpClient.Request.Accept = HttpContentTypes.ApplicationJson;
        }

        string GetDocumentUrl(TId documentId)
        {
            return String.Format("{0}/{1}", _baseUrl, documentId);
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
                var getResponse = _httpClient.Get(GetDocumentUrl(document.Id));

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
                _httpClient.Put(GetDocumentUrl(document.Id), document, HttpContentTypes.ApplicationJson);

                return _httpClient.Response.DynamicBody.id;
            }
            // It's a new insert with auto-assign
            _httpClient.Post(_baseUrl, document, HttpContentTypes.ApplicationJson);

            return _httpClient.Response.DynamicBody.id;

        }

        public TDocument GetDocument(TId id)
        {

            var response = _httpClient.Get(GetDocumentUrl(id));
            
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return response.StaticBody<TDocument>();
            }

            throw new DocumentNotFoundException("id");
        }

        public void DeleteDocument(TId id)
        {
            var response  = _httpClient.Get(GetDocumentUrl(id));

            if (_httpClient.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException();
            }
            var document = response.StaticBody<TDocument>();

            _httpClient.Delete(String.Format("{0}?rev={1}", GetDocumentUrl(id), document.Revision));

            if (_httpClient.Response.StatusCode != HttpStatusCode.OK)
            {
                throw new DocumentNotFoundException();
            }
        }

        public IEnumerable<dynamic> GetAllDocuments()
        {
            var response = _httpClient.Get(String.Format("{0}/_all_docs?include_docs=true", _baseUrl));

            var wrapper = response.StaticBody<MultiRowResponseWrapperForAllDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        public IEnumerable<TDocument> GetDocuments()
        {
            var response = _httpClient.Get(string.Format("{0}/_design/easycouchdb_views/_view/all", _baseUrl));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                // Save 
                dynamic mappingFunction = new ExpandoObject();

                mappingFunction.map = "function (doc) { if (doc.internalDocType=='" + typeof(TDocument).Name + "') { emit(doc.id,doc);}}";
                dynamic mapping = new ExpandoObject();

                mapping.all = mappingFunction;

                dynamic viewDocument = new ExpandoObject();

                viewDocument._id = "_design/easycouchdb_views";
                viewDocument.language = "javascript";
                viewDocument.views = mapping;

                _httpClient.Put(string.Format("{0}/_design/easycouchdb_views", _baseUrl), viewDocument, HttpContentTypes.ApplicationJson);

                response = _httpClient.Get(string.Format("{0}/_design/easycouchdb_views/_view/all", _baseUrl));
            }

            
            
            var wrapper = response.StaticBody<MultiRowResponseWrapperForDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }
    }
}
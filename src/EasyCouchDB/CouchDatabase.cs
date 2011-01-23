#region License
// Distributed under the BSD License
// =================================
// 
// Copyright (c) 2010-2011, Hadi Hariri
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Hadi Hariri nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// =============================================================
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EasyCouchDB.Infrastructure;
using EasyCouchDB.Views;
using EasyHttp.Http;

namespace EasyCouchDB
{
    public class CouchDatabase : ICouchDatabase
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

        public string Save<TDocument>(TDocument document) where TDocument : class, IDocument
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

        public TDocument Load<TDocument>(object id)
        {
            HttpResponse response = _server.Get(id.ToString());

            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                return response.StaticBody<TDocument>();
            }

            throw new DocumentNotFoundException(id.ToString());
        }

        public void Delete(object id)
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

            var wrapper = response.StaticBody<MultiRowResponseWrapperForAllDocs<dynamic>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        public IEnumerable<TDocument> GetDocuments<TDocument>()
        {
            if (!_viewManager.ViewExists("all"))
            {
                _viewManager.CreateView("all",
                                        "if (doc.internalDocType=='" + typeof (TDocument).Name +
                                        "') { emit(doc.id,doc);}");
            }

            return _viewManager.ExecuteView<TDocument>("all");
        }

        public void SaveAttachment(object id, string filename, string contentType)
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

        public void DeleteAttachment(object id, string attachmentName)
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
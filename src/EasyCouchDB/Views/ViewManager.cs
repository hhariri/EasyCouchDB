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
using System.Linq;
using System.Net;
using EasyCouchDB.Infrastructure;
using EasyHttp.Http;

namespace EasyCouchDB.Views
{
    public class ViewManager : IViewManager
    {
        // Due to limitations, we are treating each View as a separate design document and 
        // the name of the view refers to the design document

        readonly ICouchServer _server;

        public ViewManager(ICouchServer server)
        {
            _server = server;
        }

        public void CreateView(string viewName, string mapCode, string reduceCode = "")
        {
            var mapReduce = new MapReduce(mapCode, reduceCode);

            var views = new Views(mapReduce);

            var viewDocument = new ViewDocument(viewName) {Views = views};

            HttpResponse response = _server.Put(GetDesignDocUri(viewName), viewDocument);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new DocumentUpdateException(response.StatusDescription);
            }
        }

        public bool ViewExists(string viewName)
        {
            HttpResponse response = _server.Head(GetDesignDocUri(viewName));

            return response.StatusCode == HttpStatusCode.OK;
        }

        public IEnumerable<TDocument> ExecuteView<TDocument>(string viewName)
        {
            HttpResponse response = _server.Get(GetDesignDocUri(viewName) + "/_view/mapreduce");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new ViewException(response.StatusDescription);
            }

            var wrapper = response.StaticBody<MultiRowResponseWrapperForDocs<TDocument>>();

            return wrapper.Rows.Select(t => t.Document).ToList();
        }

        static string GetDesignDocUri(string viewName)
        {
            return String.Format("_design/easycouchdb_view_{0}", viewName);
        }
    }
}
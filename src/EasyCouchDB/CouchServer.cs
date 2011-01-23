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
using EasyHttp.Http;

namespace EasyCouchDB
{
    public class CouchServer : ICouchServer
    {
        readonly string _baseUrl;
        readonly HttpClient _connection;

        public CouchServer(string host, int port, string database)
        {
            _baseUrl = String.Format("http://{0}:{1}/{2}", host, port, database);

            _connection = new HttpClient();

            _connection.Request.Accept = HttpContentTypes.ApplicationJson;
        }


        public HttpResponse Get(string uri)
        {
            return _connection.Get(GetFullUri(uri));
        }

        public HttpResponse Put(string uri, object data)
        {
            _connection.Put(GetFullUri(uri), data, HttpContentTypes.ApplicationJson);

            return _connection.Response;
        }

        public HttpResponse PutFile(string uri, string fileName, string contentType)
        {
            _connection.PutFile(GetFullUri(uri), fileName, contentType);

            return _connection.Response;
        }

        public HttpResponse Post(string uri, object data)
        {
            _connection.Post(GetFullUri(uri), data, HttpContentTypes.ApplicationJson);

            return _connection.Response;
        }

        public HttpResponse Head(string uri)
        {
            _connection.Head(GetFullUri(uri));

            return _connection.Response;
        }

        public HttpResponse Delete(string uri)
        {
            _connection.Delete(GetFullUri(uri));

            return _connection.Response;
        }

        string GetFullUri(string uri)
        {
            return (uri == "/") ? _baseUrl : String.Format("{0}/{1}", _baseUrl, uri);
        }
    }
}
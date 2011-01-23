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
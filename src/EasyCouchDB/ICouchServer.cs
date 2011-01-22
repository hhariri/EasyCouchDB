using EasyHttp.Http;

namespace EasyCouchDB
{
    public interface ICouchServer
    {
        HttpResponse Get(string uri);
        HttpResponse Put(string uri, object data);
        HttpResponse Post(string uri, object data);
        HttpResponse Head(string uri);
        HttpResponse Delete(string uri);
        HttpResponse PutFile(string uri, string fileName, string contentType);
    }
}
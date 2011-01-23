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
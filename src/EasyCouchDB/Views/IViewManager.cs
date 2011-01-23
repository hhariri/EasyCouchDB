using System.Collections.Generic;

namespace EasyCouchDB.Views
{
    public interface IViewManager
    {
        void CreateView(string viewName, string mapCode, string reduceCode = "");
        bool ViewExists(string viewName);
        IEnumerable<TDocument> ExecuteView<TDocument>(string viewName);
    }
}
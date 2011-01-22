using System.Dynamic;

namespace EasyCouchDB
{
    public class ViewManager
    {

        public dynamic Create(string viewName, string mapCode, string reduceCode)
        {
            dynamic mappingFunction = new ExpandoObject();

            mappingFunction.map = string.Format("function (doc) {{ {0} }}", mapCode);
            dynamic mapping = new ExpandoObject();

            mapping.all = mappingFunction;

            dynamic viewDocument = new ExpandoObject();

            viewDocument._id = "_design/easycouchdb_views";
            viewDocument.language = "javascript";
            viewDocument.views = mapping;

            return viewDocument;
        }
    }
}
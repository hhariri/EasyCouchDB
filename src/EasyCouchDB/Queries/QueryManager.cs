namespace EasyCouchDB.Queries
{
    public class QueryManager
    {
        readonly ICouchServer _server;

        public QueryManager(ICouchServer server)
        {
            _server = server;
        }
    }
}
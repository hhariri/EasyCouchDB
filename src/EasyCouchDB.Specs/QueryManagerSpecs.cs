using EasyCouchDB.Queries;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof (QueryManager), "given document database")]
    public class when_executing_a_query : ServerAndDatabaseContext
    {
        Establish context = () => { queryManager = new QueryManager(Server); };

        Because of = () =>
        {
            //    queryManager.ExecuteQuery();

            //queryManager.ExecuteQuery()
        };

        //It should_create_the_view_document = () =>
        //{
        //    queryManager.ViewExists(DocumentId).ShouldBeTrue();
        //};

        static QueryManager queryManager;
    }
}
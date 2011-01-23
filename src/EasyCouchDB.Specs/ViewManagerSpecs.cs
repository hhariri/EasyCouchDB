using EasyCouchDB.Views;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof (ViewManager), "given document database")]
    public class when_asking_if_a_non_existing_view_eixsts : ServerAndDatabaseContext
    {
        Establish context = () => { _viewManager = new ViewManager(Server); };

        Because of = () => { exists = _viewManager.ViewExists("some_random_view_doc"); };

        It should_return_false = () => { exists.ShouldBeFalse(); };

        static ViewManager _viewManager;
        static bool exists;
    }

    [Subject(typeof (ViewManager), "given document database")]
    public class when_creating_a_view_document : ServerAndDatabaseContext
    {
        Establish context = () => { viewManager = new ViewManager(Server); };

        Because of = () => { viewManager.CreateView(DocumentId, "", ""); };

        It should_create_the_view_document = () => { viewManager.ViewExists(DocumentId).ShouldBeTrue(); };

        static ViewManager viewManager;
    }
}
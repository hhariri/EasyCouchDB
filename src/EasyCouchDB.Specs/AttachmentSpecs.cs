using System.IO;
using EasyCouchDB.Specs.Helpers;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof (CouchDatabase<User, string>), "given a document database")]
    public class when_adding_an_attachment_to_an_existing_document : ServerAndDatabaseContext
    {
        Because of = () =>
        {
            string imageFile = Path.Combine("Helpers", "test.jpg");

            Database.SaveAttachment(DocumentId, imageFile, "image/jpeg");
        };

        It should_add_it = () => { };

        static User user;
        static string id;
    }

    [Subject(typeof (CouchDatabase<User, string>), "given a document database")]
    public class when_deleting_an_attachment : ServerAndDatabaseContext
    {
        Establish context = () =>
        {
            string imageFile = Path.Combine("Helpers", "test.jpg");

            Database.SaveAttachment(DocumentId, imageFile, "image/jpeg");
        };

        Because of = () => { Database.DeleteAttachment(DocumentId, "test.jpg"); };

        It should_delete_it = () => { };

        static User user;
        static string id;
    }
}
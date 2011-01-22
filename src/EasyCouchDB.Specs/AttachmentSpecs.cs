using System.IO;
using EasyCouchDB.Specs.Helpers;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{


    [Subject(typeof(CouchDatabase<User, string>), "given a document database")]
    public class when_adding_an_attachment_to_an_existing_document : DatabaseContext
    {
        Because of = () =>
        {
            var imageFile = Path.Combine("Helpers", "test.jpg");

            couchDb.SaveAttachment(DocumentId, imageFile, "image/jpeg");
        };

        It should_add_it = () => { };

        static User user;
        static string id;
    }  
    
    [Subject(typeof(CouchDatabase<User, string>), "given a document database")]
    public class when_deleting_an_attachment : DatabaseContext
    {
        Establish context = () =>
        {
            var imageFile = Path.Combine("Helpers", "test.jpg");

            couchDb.SaveAttachment(DocumentId, imageFile, "image/jpeg");
        };

        Because of = () =>
        {
            couchDb.DeleteAttachment(DocumentId, "test.jpg");
        };

        It should_delete_it = () => { };

        static User user;
        static string id;
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using EasyCouchDB.Specs.Helpers;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_creating_a_new_document_with_no_id: DatabaseContext
    {
        Because of = () =>
        {
            user = new User {Fullname = "Jackson"};

            id = repository.Save(user);

        };

        It should_create_the_document_and_return_a_generated_id = () => id.ShouldNotBeEmpty();

        static User user;
        static string id;
    }

    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_creating_a_new_document_provided_an_id: DatabaseContext
    {
        Because of = () =>
        {
            var randomDocumentId = GetRandomDocumentId();

            user = new User {Id = randomDocumentId , Fullname = "Jackson"};

            id = repository.Save(user);

        };

        It should_create_the_document_and_return_the_given_id = () => id.ShouldEqual(user.Id);

        static User user;
        static string id;
    }

    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_updating_an_existing_document: DatabaseContext
    {
        Establish context = () =>
        {
            var randomDocumentId = GetRandomDocumentId();

            user = new User { Id = randomDocumentId, Fullname = "Jackson" };

            id = repository.Save(user);

        };

        Because of = () =>
        {
            var document = repository.GetDocument(id);

            document.Fullname = "New Name";

            repository.Save(document);

        };

        It should_update_the_document = () =>
        {
            var updatedDocument = repository.GetDocument(id);

            updatedDocument.Fullname.ShouldEqual("New Name");
        };
        static User user;
        static string id;
    }

 
   
    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_getting_a_document_by_id_that_exists: DatabaseContext
    {

        Because of = () =>
        {
            user = repository.GetDocument(DocumentId);
        };

        It should_retrieve_the_document = () => user.ShouldNotBeNull();

        It should_have_id_set_correctly = () => user.Id.ShouldEqual(DocumentId);

        It should_have_properties_set_correctly = () => user.Fullname.ShouldEqual("My First User");

        It should_have_revision_set_correctly = () => user.Revision.ShouldNotBeEmpty();

        static User user;
    }

    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_deleting_an_existing_document: DatabaseContext
    {
        Establish context = () =>
        {
            randomDocumentId = string.Format("{0}DeleteTest", GetRandomDocumentId());

            repository.Save(new User() { Id = randomDocumentId });
        };

        Because of = () =>
        {
            repository.DeleteDocument(randomDocumentId);
        };

        It should_delete_the_document = () =>
        {
            try
            {
                repository.GetDocument(randomDocumentId);
            }
            catch (DocumentNotFoundException)
            {
                true.ShouldBeTrue();
            }
       };

        static string randomDocumentId;
    }

    [Subject(typeof(Repository<User, string>), "given a document database")]
    public class when_getting_a_list_of_documents: DatabaseContext
    {
        Because of = () =>
        {
            documents = repository.GetAllDocuments();
        };

        It should_return_all_documents = () =>
        {
            documents.ShouldNotBeEmpty();
        };

        It should_set_document_properties = () =>
        {
            documents.First().Fullname.ShouldNotBeEmpty();
        };

        static IEnumerable<User> documents;
    }

    public class DatabaseContext
    {
        Establish context = () =>
        {
            DocumentId = GetRandomDocumentId();

            repository = new Repository<User, string>("localhost", 5984, "easycouchdb");

            var user = new User { Id = DocumentId, Fullname = "My First User", EmailAddress = "MyEmail@MyDomain.com" };

            repository.Save(user);
        };

        protected static Repository<User, string> repository;
        protected static string DocumentId;

        protected static string GetRandomDocumentId()
        {
            var random = new Random();
          
            return String.Format("{0}{1}", random.Next(10, 10000), DateTime.Now.ToShortTimeString());
        }
    }
}
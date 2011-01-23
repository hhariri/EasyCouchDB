#region License
// Distributed under the BSD License
// =================================
// 
// Copyright (c) 2010-2011, Hadi Hariri
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Hadi Hariri nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// =============================================================
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using EasyCouchDB.Infrastructure;
using EasyCouchDB.Specs.Helpers;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_creating_a_new_document_with_no_id : ServerAndDatabaseContext
    {
        Because of = () =>
        {
            user = new User {Fullname = "Jackson"};

            id = Database.Save(user);
        };

        It should_create_the_document_and_return_a_generated_id = () => id.ShouldNotBeEmpty();

        static User user;
        static string id;
    }

    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_creating_a_new_document_provided_an_id : ServerAndDatabaseContext
    {
        Because of = () =>
        {
            string randomDocumentId = GetRandomDocumentId();

            user = new User {Id = randomDocumentId, Fullname = "Jackson"};

            id = Database.Save(user);
        };

        It should_create_the_document_and_return_the_given_id = () => id.ShouldEqual(user.Id);

        static User user;
        static string id;
    }

    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_updating_an_existing_document : ServerAndDatabaseContext
    {
        Establish context = () =>
        {
            string randomDocumentId = GetRandomDocumentId();

            user = new User {Id = randomDocumentId, Fullname = "Jackson"};

            id = Database.Save(user);
        };

        Because of = () =>
        {
            var document = Database.Load<User>(id);

            document.Fullname = "New Name";

            Database.Save(document);
        };

        It should_update_the_document = () =>
        {
            var updatedDocument = Database.Load<User>(id);

            updatedDocument.Fullname.ShouldEqual("New Name");
        };

        static User user;
        static string id;
    }


    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_getting_a_document_by_id_that_exists : ServerAndDatabaseContext
    {
        Because of = () => { user = Database.Load<User>(DocumentId); };

        It should_retrieve_the_document = () => user.ShouldNotBeNull();

        It should_have_id_set_correctly = () => user.Id.ShouldEqual(DocumentId);

        It should_have_properties_set_correctly = () => user.Fullname.ShouldEqual("My First User");

        It should_have_revision_set_correctly = () => user.Revision.ShouldNotBeEmpty();

        static User user;
    }

    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_deleting_an_existing_document : ServerAndDatabaseContext
    {
        Establish context = () =>
        {
            randomDocumentId = string.Format("{0}DeleteTest", GetRandomDocumentId());

            Database.Save(new User {Id = randomDocumentId});
        };

        Because of = () => { Database.Delete(randomDocumentId); };

        It should_delete_the_document = () =>
        {
            try
            {
                Database.Load<User>(randomDocumentId);
            }
            catch (DocumentNotFoundException)
            {
                true.ShouldBeTrue();
            }
        };

        static string randomDocumentId;
    }

    [Subject(typeof (CouchDatabase), "given a document database")]
    public class when_getting_a_list_of_documents : ServerAndDatabaseContext
    {
        Because of = () =>
        {
            documents = from d in Database.GetDocuments<User>()
                        select d;
        };

        It should_return_all_documents = () => { documents.ShouldNotBeEmpty(); };

        It should_set_document_properties = () => { documents.First().Fullname.ShouldNotBeEmpty(); };

        static IEnumerable<User> documents;
    }


    public class ServerAndDatabaseContext
    {
        protected static ICouchDatabase Database;
        protected static ICouchServer Server;
        protected static string DocumentId;

        Cleanup cleanup = () =>
        {
            try
            {
                Database.Delete("_design/easycouchdb_view_all");
            }
            catch (Exception)
            {
                throw;
            }
        };

        Establish context = () =>
        {
            DocumentId = GetRandomDocumentId();

            Server = new CouchServer("localhost", 5984, "easycouchdb");

            Database = new CouchDatabase(Server);

            var user = new User {Id = DocumentId, Fullname = "My First User", EmailAddress = "MyEmail@MyDomain.com"};

            Database.Save(user);
        };

        protected static string GetRandomDocumentId()
        {
            var random = new Random();

            return String.Format("{0}{1}", DateTime.Now.Ticks, random.Next(10, 10000));
        }
    }
}
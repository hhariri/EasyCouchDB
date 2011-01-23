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
using EasyCouchDB.Views;
using Machine.Specifications;

namespace EasyCouchDB.Specs
{
    [Subject(typeof (ViewManager), "given document database")]
    public class when_asking_if_a_non_existing_view_eixsts : ServerAndDatabaseContext
    {
        Establish context = () => { _viewManager = new ViewManager(Server); };

        Because of = () => { exists = _viewManager.ViewExists("some_random_view_doc", "random_view"); };

        It should_return_false = () => { exists.ShouldBeFalse(); };

        static ViewManager _viewManager;
        static bool exists;
    }

    [Subject(typeof (ViewManager), "given document database")]
    public class when_creating_a_view_document : ServerAndDatabaseContext
    {
        Establish context = () => { viewManager = new ViewManager(Server); };

        Because of = () => { viewManager.CreateView(DocumentId, "", ""); };

        It should_create_the_view_document_with_view_mapreduce = () => { viewManager.ViewExists(DocumentId, "mapreduce").ShouldBeTrue(); };

        static ViewManager viewManager;
    }
}
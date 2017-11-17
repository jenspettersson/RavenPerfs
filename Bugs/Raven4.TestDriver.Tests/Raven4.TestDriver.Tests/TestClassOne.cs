using System.Linq;
using Raven.TestDriver;
using Xunit;

namespace Raven4.TestDriver.Tests
{
    public class LocalRavenServerLocator : RavenServerLocator
    {
        /* The settings file in the same folder as the ServerPath below
        {
            "ServerUrl": "http://127.0.0.1:0",
            "Setup.Mode": "Initial",
            "DataDir": "APPDRIVE:/RavenIntegrationTests",
            "RunInMemory": true
        }
        */
        public override string ServerPath => @"C:\RavenDB\IntegrationTestServer\Raven.Server.dll";
        public override string Command => "dotnet";
        public override string CommandArguments => ServerPath;
    }

    public class TestClassOne : RavenTestDriver<LocalRavenServerLocator>
    {
        // This test method has the same name as a method in TestClassTwo
        // Might not always throw but it's depending on if it's run in parallel with the other test
        [Fact]
        public void MyTest()
        {
            using (var documentStore = GetDocumentStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new TestDocument());
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var testDocuments = session.Query<TestDocument>().ToList();
                    Assert.Equal(1, testDocuments.Count);
                }
            }
        }

        // This test fails occasionally with exception "Database 'MySecondTest_1' already exists" 
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void MySecondTest(int numberOfDocumentsToStore)
        {
            using (var documentStore = GetDocumentStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    for (int i = 0; i < numberOfDocumentsToStore; i++)
                    {
                        session.Store(new TestDocument());
                    }

                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var testDocuments = session.Query<TestDocument>().ToList();
                    Assert.Equal(numberOfDocumentsToStore, testDocuments.Count);
                }
            }
        }
    }

    public class TestClassTwo : RavenTestDriver<LocalRavenServerLocator>
    {
        // This test method has the same name as a method in TestClassOne
        // Might not always throw but it's depending on if it's run in parallel with the other test
        [Fact]
        public void MyTest() 
        {
            using (var documentStore = GetDocumentStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new TestDocument());
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var testDocuments = session.Query<TestDocument>().ToList();
                    Assert.Equal(1, testDocuments.Count);
                }
            }
        }

        [Fact]
        public void MySecondTest_Different_Name()
        {
            using (var documentStore = GetDocumentStore())
            {
                using (var session = documentStore.OpenSession())
                {
                    session.Store(new TestDocument());
                    session.Store(new TestDocument());
                    session.Store(new TestDocument());
                    session.Store(new TestDocument());
                    session.SaveChanges();
                }

                using (var session = documentStore.OpenSession())
                {
                    var testDocuments = session.Query<TestDocument>().ToList();
                    Assert.Equal(4, testDocuments.Count);
                }
            }
        }
    }

    public class TestDocument
    {
        public string Id { get; set; }
    }
}

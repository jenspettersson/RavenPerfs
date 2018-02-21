using System;
using System.Collections.Generic;
using Raven.Client.Documents.Linq;
using System.Linq;
using System.Threading.Tasks;
using Raven.Client.Documents;

namespace Raven4.Projections.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var documentStore = new DocumentStore
            {
                Urls = new[] { "http://localhost:4040" },
                Database = "Raven.Lab"
            };

            documentStore.Initialize();

            RunSample(documentStore).GetAwaiter().GetResult();
        }

        private static async Task RunSample(DocumentStore documentStore)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var user = new User
                {
                    Id = "users/1",
                    Tags = new List<Tag>
                    {
                        new Tag
                        {
                            Id = "tag-id1", 
                            Name = "Tag name"
                        }
                    }
                };

                await session.StoreAsync(user, "users/1");
                await session.SaveChangesAsync();
            }

            using (var session = documentStore.OpenAsyncSession())
            {
                var query = session.Query<User>();

                var projection = from user in query
                                 let tagIds = user.Tags.Select(x => x.Id)
                                 let tags = user.Tags.Select(x => x.Name)
                                 select new
                                 {
                                     UserId = user.Id,
                                     Tags = tags,
                                     TagIds = tagIds
                                 };

                var result = await projection.FirstAsync();
            }
        }
    }

    public class Tag
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public string Id { get; set; }
        public List<Tag> Tags { get; set; }
    }
}

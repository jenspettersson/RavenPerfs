using System.Collections.Generic;
using Raven.Client.Embedded;
using Xunit;

namespace RavenPerfs.Raven2_5
{
    public class Entity
    {
        public string Id { get; set; }
        public List<string> History { get; set; }

        public Entity()
        {
            History = new List<string> { "Entity created" };
        }
    }

    public class LoadCausesChange
    {
        [Fact]
        public void Load_on_state_with_list_in_ctor()
        {
            var store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            store.Initialize();

            using (var session = store.OpenSession())
            {
                session.Store(new Entity {Id = "Entities/1"});
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var entity = session.Load<Entity>("Entities/1");

                Assert.False(session.Advanced.HasChanges);
            }
        }
    }
}
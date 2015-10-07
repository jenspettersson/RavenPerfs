using Raven.Client.Embedded;
using Xunit;

namespace RavenPerfs.Raven2_5
{
    public class FloatEntity
    {
        public string Id { get; set; }
        public float Value { get; set; }
    }

    public class LoadCausesChange_OnFloat
    {
        [Fact]
        public void Load_on_state_with_float()
        {
            var store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            store.Initialize();

            using (var session = store.OpenSession())
            {
                session.Store(new FloatEntity { Id = "FloatEntities/1", Value = 0.3f});
                session.SaveChanges();
            }

            using (var session = store.OpenSession())
            {
                var entity = session.Load<FloatEntity>("FloatEntities/1");

                Assert.False(session.Advanced.HasChanges);
            }
        }
    }
}
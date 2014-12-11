using System;
using System.Diagnostics;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using RavenPerfs.Raven3.RavenTests;
using Xunit;

namespace RavenPerfs.Raven3
{
    public class InitializeEmbeddable
    {
        [Fact]
        public void Initialize_without_index()
        {
            var stopwatch = Stopwatch.StartNew();
            var embeddableDocumentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            embeddableDocumentStore.Initialize();
            embeddableDocumentStore.Dispose();
            Console.WriteLine("Done: {0}", stopwatch.Elapsed);
        }

        [Fact]
        public void Initialize_with_index()
        {
            var stopwatch = Stopwatch.StartNew();
            var embeddableDocumentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            embeddableDocumentStore.Initialize();
            
            IndexCreation.CreateIndexes(typeof(CanCallLastOnArray).Assembly, embeddableDocumentStore);

            embeddableDocumentStore.Dispose();
            Console.WriteLine("Done: {0}", stopwatch.Elapsed);
        }

        [Fact]
        public void Initialize_without_index_100_times()
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
            {
                var embeddableDocumentStore = new EmbeddableDocumentStore
                {
                    RunInMemory = true
                };

                embeddableDocumentStore.Initialize();
                embeddableDocumentStore.Dispose();
            }
            Console.WriteLine("Done: {0}", stopwatch.Elapsed);
        }
    }
}
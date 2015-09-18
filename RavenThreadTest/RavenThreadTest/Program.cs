using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;

namespace RavenThreadTest
{
    class Program
    {
        private static Stopwatch Stopwatch;
        static void Main(string[] args)
        {
            //https://groups.google.com/forum/#!topic/ravendb/jNfe2OJ5f9c
            ThreadPool.SetMinThreads(20, 1);

            var maxTasks = 10;
            Console.WriteLine("Running 3.0.3785 with {0} tasks", maxTasks);
            var tasks = new List<Task>();

            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8081",
                DefaultDatabase = "RavenActor"
            }.Initialize();

            for (int i = 0; i < maxTasks; i++)
            {
                tasks.Add(CreateTask(documentStore));
            }
            
            Stopwatch = Stopwatch.StartNew();
            tasks.ForEach(x => x.Start());

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("All done - Took: {0} ms", Stopwatch.ElapsedMilliseconds);

            Console.ReadLine();
        }

        private static Task CreateTask(IDocumentStore documentStore)
        {
            return new Task(() =>
            {
                using (var session = documentStore.OpenSession())
                {
                    Console.WriteLine("Before Store \t" + Stopwatch.ElapsedMilliseconds);
                    session.Store(new Test
                    {
                        Name = "Test"
                    });
                    Console.WriteLine("After Store / Before SaveChanges \t" + Stopwatch.ElapsedMilliseconds);
                    session.SaveChanges();
                    Console.WriteLine("After SaveChanges \t" + Program.Stopwatch.ElapsedMilliseconds);
                }
            });
        }
    }

    public class Test
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

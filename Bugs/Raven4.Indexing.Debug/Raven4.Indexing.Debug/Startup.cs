using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;

namespace Raven4.Indexing.Debug
{
    /*
     * Content of settings.json for Ravendb:
{
  "ServerUrl": "http://localhost:4040",
  "DataDir": "APPDRIVE:/Raven",
  "Indexing": {
    "TimeToWaitBeforeMarkingAutoIndexAsIdleInMin": 1
  },
  "RunInMemory": false
}
     */

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var documentStore = new DocumentStore
            {
                Urls = new[] { "http://localhost:4040" },
                Database = "Raven.Index.Debug"
            };

            documentStore.Conventions.RegisterAsyncIdConvention<TestDocument>((dbname, testDocument) => Task.FromResult($"TestDocuments/{testDocument.Key}"));

            documentStore.Initialize();

            services.AddSingleton<IDocumentStore>(documentStore);
            services.AddScoped(svc => svc.GetService<IDocumentStore>().OpenAsyncSession());

            //Pre populate data and indexes
            PopulateData(documentStore);

            services.AddMvc();
        }

        private void PopulateData(DocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                for (int i = 0; i < 20; i++)
                {
                    session.Store(new TestDocument { Key = Guid.NewGuid().ToString("N"), Created = DateTime.UtcNow });
                    session.Store(new AnotherTestDocument { Created = DateTime.UtcNow });
                }

                session.SaveChanges();
            }

            using (var session = documentStore.OpenSession())
            {
                session
                    .Query<TestDocument>()
                    .Where(x => x.Created != null)
                    .ToList(); //Just to create an AutoIndex

                session
                    .Query<AnotherTestDocument>()
                    .Where(x => x.Created != null)
                    .ToList(); //Just to create an AutoIndex
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}

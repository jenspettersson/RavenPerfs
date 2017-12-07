using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Raven4.Indexing.Debug.Controllers
{
    [Route("api/[controller]")]
    public class TestDocumentsController : Controller
    {
        private readonly IAsyncDocumentSession _session;

        public TestDocumentsController(IAsyncDocumentSession session)
        {
            _session = session;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var testDocuments = await _session
                .Query<TestDocument>()
                .Statistics(out var stats)
                .Where(x => x.Created != null) //just to force a creation of an AutoIndex
                .ToListAsync()
                .ConfigureAwait(false);

            return Json(new
            {
                NumberOfActualResults = testDocuments.Count,
                Stats = stats
            });
        }

        // POST api/values
        [HttpPost]
        public async Task Post()
        {
            await _session.StoreAsync(new TestDocument {Key = Guid.NewGuid().ToString("N"), Created = DateTime.UtcNow}).ConfigureAwait(false);
            
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

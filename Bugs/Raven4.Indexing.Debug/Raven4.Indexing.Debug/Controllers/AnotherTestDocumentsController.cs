using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Raven4.Indexing.Debug.Controllers
{
    [Route("api/[controller]")]
    public class AnotherTestDocumentsController : Controller
    {
        private readonly IAsyncDocumentSession _session;

        public AnotherTestDocumentsController(IAsyncDocumentSession session)
        {
            _session = session;
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var testDocuments = await _session
                .Query<AnotherTestDocument>()
                .Statistics(out var stats)
                .Where(x => x.Created != null) //just to force a creation of an AutoIndex
                .ToListAsync().ConfigureAwait(false);

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
            await _session.StoreAsync(new AnotherTestDocument { Created = DateTime.UtcNow }).ConfigureAwait(false);

            await _session.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Raven.Client;
using Raven.Metrics.Bug.Models;

namespace Raven.Metrics.Bug.Controllers
{
    public class ValuesController : ApiController
    {
        public async Task<IEnumerable<SomeState>> Get()
        {
            using (var session = Storage.Store.OpenAsyncSession())
            {
                var states = await session.Query<SomeState>().ToListAsync();

                return states;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using System.Threading;

namespace ReactiveExtensionExamples.Services.Api
{
    public interface IDuckDuckGoApi
    {
        [Get("/?q={query}&format=json")]
        Task<DuckDuckGoSearchResult> Search(string query);
        
        [Get("/?q={query}&format=json")]
        Task<DuckDuckGoSearchResult> Search(string query, CancellationToken cancellationToken);
    }
}

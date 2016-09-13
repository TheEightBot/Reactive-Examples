using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace ReactiveExtensionExamples.Services.Api
{
    public interface IDuckDuckGoApi
    {
        [Get("/?q={query}&format=json")]
        Task<DuckDuckGoSearchResult> Search(string query);
    }
}

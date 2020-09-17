using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;
using System.Threading;
using ReactiveExtensionExamples.Models;
using System.Net.Http;
using System.Xml.Linq;

namespace ReactiveExtensionExamples.Services.Api
{
    public static class RssDownloader
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<IEnumerable<RssEntry>> DownloadRss (string url, CancellationToken ct)
        {
            var rssStreamResponse = await client.GetAsync(url, ct).ConfigureAwait(false);

            if (!ct.IsCancellationRequested && rssStreamResponse.IsSuccessStatusCode)
            {
                var rssStream = await rssStreamResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!ct.IsCancellationRequested)
                {
                    return
                        await Task.Run(() =>
                        {
                            XNamespace ns = "http://www.w3.org/2005/Atom";

                            var entries =
                                XDocument
                                    .Parse(rssStream)
                                    .Root
                                    .Descendants(ns + "entry");

                            return entries
                                    .Select(entry =>
                                            new RssEntry
                                            {
                                                Id = entry?.Element(ns + "id")?.Value ?? string.Empty,
                                                Author = entry?.Element(ns + "author")?.Element(ns + "name")?.Value ?? string.Empty,
                                                Category = entry?.Element(ns + "category")?.Attribute("label")?.Value ?? string.Empty,
                                                Content = entry?.Element(ns + "content")?.Value ?? string.Empty,
                                                Updated = DateTimeOffset.Parse(entry?.Element(ns + "updated")?.Value),
                                                Title = entry?.Element(ns + "title")?.Value ?? string.Empty
                                            })
                                .ToList();
                        }, ct)
                        .ConfigureAwait(false);
                }
            }

            return Enumerable.Empty<RssEntry>();
        }
    }
}

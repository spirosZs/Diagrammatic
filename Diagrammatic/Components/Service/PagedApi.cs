using Newtonsoft.Json;

// Every "get multiple" endpoint on the API is paginated: PaginationFilter defaults to 10
// items per page and its [Range(0, 20)] rejects a larger PageSize, so a plain GET silently
// returns only the first page. These helpers walk every page so dropdowns bound to a table
// show all of its rows.
namespace Diagrammatic_test.Services
{
    public static class PagedApi
    {
        // Largest value the API's PaginationFilter accepts.
        private const int MaxPageSize = 20;

        public static async Task<List<T>> GetAllPagesAsync<T>(this HttpClient httpClient, string url,
            CancellationToken token = default)
        {
            var items = new List<T>();
            var separator = url.Contains('?') ? '&' : '?';

            for (var pageNumber = 1; ; pageNumber++)
            {
                var response = await httpClient.GetAsync(
                    $"{url}{separator}pageNumber={pageNumber}&pageSize={MaxPageSize}", token);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(token);
                var page = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
                items.AddRange(page);

                if (page.Count == 0)
                {
                    return items;
                }

                var totalPages = ReadTotalPages(response);
                if (totalPages.HasValue ? pageNumber >= totalPages.Value : page.Count < MaxPageSize)
                {
                    return items;
                }
            }
        }

        // The API reports paging state in an X-Pagination header. When it is missing, fall
        // back to treating a short page as the last one.
        private static int? ReadTotalPages(HttpResponseMessage response)
        {
            if (!response.Headers.TryGetValues("X-Pagination", out var values))
            {
                return null;
            }

            var meta = values.FirstOrDefault();
            if (string.IsNullOrEmpty(meta))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<PaginationMeta>(meta)?.TotalPages;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private sealed class PaginationMeta
        {
            public int TotalPages { get; set; }
        }
    }
}

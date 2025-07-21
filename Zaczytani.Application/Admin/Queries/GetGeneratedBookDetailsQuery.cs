using System.Text.RegularExpressions;
using MediatR;
using Zaczytani.Application.Dtos;
using Zaczytani.Application.Http;
using Zaczytani.Domain.Enums;
using Zaczytani.Domain.Helpers;

namespace Zaczytani.Application.Admin.Queries;

public class GetGeneratedBookDetailsQuery : IRequest<GeneratedBookDto>
{
    public string Isbn { get; set; } = null!;

    private class GetGeneratedBookDetailsQueryHandler(BookHttpClient httpClient) : IRequestHandler<GetGeneratedBookDetailsQuery, GeneratedBookDto>
    {
        private readonly BookHttpClient _httpClient = httpClient;
        private string SearchString = string.Empty;
        private readonly string SkuPattern = @"""sku"":""(\d+)""";
        private readonly string TitlePattern = @"""name"":""([^""]+)""";
        private readonly string AuthorPattern = @"""name"":\s*""([^""]+)""\s*,\s*""type"":\s*""author""";
        private readonly string PagesPattern = @"""attribute_value"":""(\d+)"",""attribute_code"":""pages_count""";
        private readonly string IsbnPattern = @"""ean"":""(\d+)""";
        private readonly string DescriptionPattern = @"""description"":\{""html"":""([^""]+)""";
        private readonly string ReleaseDatePattern = @"""attribute_value"":""([\d\- :]+)"",""attribute_code"":""release_date""";
        private readonly string SeriesPattern = @"""series"":\[\{""url"":""[^""]+"",""name"":""([^""]+)""\}\]";
        private readonly string PublisherPattern = @"""publishers"":\[\{""url"":""[^""]+"",""name"":""([^""]+)""\}\]";
        private readonly string CategoryPattern = @"\{""id"":\d+,""name"":""([^""]+)"",""url"":""[^""]+"",""breadcrumbs"":(?:null|\[[^\]]*\])\}";

        public async Task<GeneratedBookDto> Handle(GetGeneratedBookDetailsQuery request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync("graphql?hash=2086150927&sort_1={%22position%22:%22ASC%22}&filter_1={%22price%22:{},%22customer_group_id%22:{%22eq%22:%220%22}}&search_1=%22" + request.Isbn + "%22&pageSize_1=36&currentPage_1=1&_currency=%22PLN%22");

            var skuMatch = Regex.Match(response, SkuPattern);
            SearchString = await _httpClient.GetAsync(string.Format($"graphql?hash=1665330387&sku_1=%22{skuMatch.Groups[1]}%22&pageSize_1=5&currentPage_1=1&_currency=%22PLN%22"));

            var title = Extract(TitlePattern);
            var pages = Extract(PagesPattern);
            var isbn = Extract(IsbnPattern);
            var description = Extract(DescriptionPattern);
            var releaseDate = Extract(ReleaseDatePattern);
            var series = Extract(SeriesPattern);
            var publisher = Extract(PublisherPattern);

            var bookRequestDto = new GeneratedBookDto()
            {
                Isbn = isbn,
                Description = HtmlCleaner.CleanHtmlDescription(description),
                PageNumber = GetPageNumber(pages),
                PublishingHouse = publisher,
                Title = title,
                ReleaseDate = GetReleaseDate(releaseDate),
                Series = series,
                Authors = string.Join(", ", GetMatchingAuthors()),
                Genre = GetMatchingGenres()
            };

            return bookRequestDto;
        }

        private string? Extract(string pattern)
        {
            var match = Regex.Match(SearchString, pattern);
            return match.Success ? Regex.Unescape(match.Groups[1].Value) : null;
        }

        private int? GetPageNumber(string? pages)
        {
            return pages is not null ? int.Parse(pages) : null;
        }
        private DateOnly? GetReleaseDate(string? releaseDate)
        {
            return releaseDate is not null ? DateOnly.FromDateTime(DateTime.ParseExact(releaseDate, "yyyy-MM-dd HH:mm:ss", null)) : null;
        }

        private List<string> GetMatchingAuthors()
        {
            var autorsMatches = Regex.Matches(SearchString, AuthorPattern);
            List<string> authors = new();

            foreach (Match match in autorsMatches)
            {
                if (match.Groups.Count > 1)
                {
                    authors.Add(Regex.Unescape(match.Groups[1].Value));
                }
            }

            return authors;
        }

        private List<BookGenre> GetMatchingGenres()
        {
            var categoryMatches = Regex.Matches(SearchString, CategoryPattern);
            List<string> categories = new List<string>();
            foreach (Match match in categoryMatches)
            {
                if (match.Groups.Count > 1)
                {
                    categories.Add(Regex.Unescape(match.Groups[1].Value));
                }
            }

            var matchingGenres = new List<BookGenre>();

            foreach (var category in categories)
            {
                foreach (BookGenre genre in Enum.GetValues(typeof(BookGenre)))
                {
                    var enumDescription = EnumHelper.GetEnumDescription(genre);
                    if (enumDescription.Equals(category, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingGenres.Add(genre);
                    }
                }
            }

            return matchingGenres;
        }
    }
}

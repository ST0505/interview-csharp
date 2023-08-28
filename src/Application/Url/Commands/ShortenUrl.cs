using HashidsNet;
using UrlShortenerService.Application.Common.Interfaces;

namespace UrlShortenerService.Application.Url.Commands;
public class ShortenUrl
{
    private readonly Hashids? _hashIds;
    private readonly IApplicationDbContext _context;

    public ShortenUrl(IHashids hashids, IApplicationDbContext dbContext)
    {
        _hashIds = (Hashids?)hashids;
        _context = dbContext;
    }

    public string? GetShortenedURL(string url, long id)
    {
        var shortenedUrl = string.Empty;

        try
        {
            var uri = new Uri(url);
            var encodedId = _hashIds?.EncodeLong(id);

            if (encodedId != null)
                shortenedUrl = $"{uri.Scheme}://{uri.Host}/{encodedId}";
        }
        catch (Exception)
        {
            throw;
        }

        return shortenedUrl;
    }
}

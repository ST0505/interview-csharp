using HashidsNet;
using UrlShortenerService.Application.Common.Interfaces;
using System.Web;

namespace UrlShortenerService.Application.Url.Operations;

public class GetOriginalUrlOperation
{
    private readonly Hashids? _hashIds;
    private readonly IApplicationDbContext _context;

    public GetOriginalUrlOperation(IHashids hashIds, IApplicationDbContext dbContext)
    {
        _hashIds = (Hashids?)hashIds;
        _context = dbContext;
    }

    public string? GetOriginalURL(string requestUrl)
    {
        var originalUrl = string.Empty;

        try
        {
            var decodedRequest = HttpUtility.UrlDecode(requestUrl);
            var uri = new Uri(decodedRequest);
            var encodedId = uri.AbsolutePath.ToString().TrimStart('/');

            if (_context != null)
            {
                var tmpId = _hashIds?.DecodeSingleLong(encodedId);
                var hashId = _context.Urls.Where(p => p.Id.Equals(tmpId)).FirstOrDefault();

                if (hashId != null)
                    originalUrl = hashId?.OriginalUrl;
            }
            else
                throw new Exception("Database error");
        }
        catch (Exception)
        {
            throw;
        }

        return originalUrl;
    }
}

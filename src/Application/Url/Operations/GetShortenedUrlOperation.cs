using HashidsNet;
using UrlShortenerService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;
using System.Threading;

namespace UrlShortenerService.Application.Url.Operations;
public class GetShortenedUrlOperation
{
    private readonly Hashids? _hashIds;
    private readonly IApplicationDbContext _context;

    public GetShortenedUrlOperation(IHashids hashids, IApplicationDbContext dbContext)
    {
        _hashIds = (Hashids?)hashids;
        _context = dbContext;
    }

    public async Task<string?> GetShortenedURLAsync(string requestUrl, CancellationToken ct)
    {
        var shortenedUrl = string.Empty;

        try
        {
            long urlId;
            var urlExists = _context.Urls.Where(x => x.OriginalUrl.Equals(requestUrl)).FirstOrDefault();

            if (urlExists == null || urlExists.Id <= 0)
            {
                var entity = new Domain.Entities.Url
                {
                    OriginalUrl = requestUrl
                };
                _ = _context.Urls.Add(entity);
                urlId = await _context.SaveChangesAsync(ct);
                if (urlId == 1)
                    urlId = entity.Id;
            }
            else
                urlId = urlExists.Id;

            var uri = new Uri(requestUrl);
            var encodedId = _hashIds?.EncodeLong(urlId);

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

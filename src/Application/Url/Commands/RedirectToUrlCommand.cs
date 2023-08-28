using System.Text.Encodings.Web;
using System.Web;
using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace UrlShortenerService.Application.Url.Commands;

public record RedirectToUrlCommand : IRequest<string>
{
    public string Id { get; init; } = default!;
}

public class RedirectToUrlCommandValidator : AbstractValidator<RedirectToUrlCommand>
{
    public RedirectToUrlCommandValidator()
    {
        _ = RuleFor(v => v.Id)
          .NotEmpty()
          .WithMessage("Id is required.");
    }
}

public class RedirectToUrlCommandHandler : IRequestHandler<RedirectToUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;
    private readonly ILogger<RedirectToUrlCommandHandler> _logger;

    public RedirectToUrlCommandHandler(ILogger<RedirectToUrlCommandHandler> logger, IApplicationDbContext context, IHashids hashids)
    {
        _logger = logger;
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(RedirectToUrlCommand request, CancellationToken cancellationToken)
    {
        var response = string.Empty;

        try
        {
            if (request != null && request.Id.Length > 0)
            {
                var decodedRequest = HttpUtility.UrlDecode(request.Id);
                var uri = new Uri(decodedRequest);
                var encodedId = uri.AbsolutePath.ToString().TrimStart('/');

                if (_context != null)
                {
                    var tmpId = _hashids.DecodeSingleLong(encodedId);
                    var hashId = _context.Urls.Where(p => p.Id.Equals(tmpId)).FirstOrDefault();

                    if (hashId != null)
                        response = hashId?.OriginalUrl;
                }
                else
                    throw new Exception("Database error");
            }

            await Task.CompletedTask;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing redirect.");
        }

        return response;
    }
}

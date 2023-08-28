using System;
using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace UrlShortenerService.Application.Url.Commands;

public record CreateShortUrlCommand : IRequest<string>
{
    public string Url { get; init; } = default!;
}

public class CreateShortUrlCommandValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlCommandValidator()
    {
        _ = RuleFor(v => v.Url)
          .NotEmpty()
          .WithMessage("Url is required.");
    }
}

public class CreateShortUrlCommandHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IApplicationDbContext _context;
    private readonly IHashids _hashids;
    private readonly ILogger<CreateShortUrlCommandHandler> _logger;

    public CreateShortUrlCommandHandler(ILogger<CreateShortUrlCommandHandler> logger, IApplicationDbContext context, IHashids hashids)
    {
        _logger = logger;
        _context = context;
        _hashids = hashids;
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        var response = string.Empty;

        try
        {
            long urlId;
            var urlExists = _context.Urls.Where(x => x.OriginalUrl.Equals(request.Url)).FirstOrDefault();

            if (urlExists == null || urlExists.Id <= 0)
            {
                var entity = new Domain.Entities.Url();
                entity.OriginalUrl = request.Url;
                _ = _context.Urls.Add(entity);
                urlId = await _context.SaveChangesAsync(cancellationToken);
            }
            else
                urlId = urlExists.Id;

            var obj = new ShortenUrl(_hashids, _context);
            response = obj.GetShortenedURL(request.Url, urlId);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing shorten url request.");
        }

        return response;
    }
}

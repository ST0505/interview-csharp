using FluentValidation;
using HashidsNet;
using MediatR;
using UrlShortenerService.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using UrlShortenerService.Application.Common.Exceptions;
using UrlShortenerService.Application.Url.Operations;

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
                var obj = new GetOriginalUrlOperation(_hashids, _context);
                response = obj.GetOriginalURL(request.Id);
            }

            await Task.CompletedTask;

            if (response == string.Empty)
                throw new NotFoundException();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing redirect.");
        }

        return response ?? string.Empty;
    }
}


using Egw.PubManagement.Application.Extensions;
using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <inheritdoc />
    public class DeleteParagraphHandler : ApplicationCommandHandler<DeleteParagraphInput>
    {
        /// <inheritdoc />
        public override async Task Handle(DeleteParagraphInput request, CancellationToken cancellationToken)
        {
            Paragraph paragraph = await _db.Paragraphs
                      .FirstOrDefaultAsync(r => r.ParaId == request.ParaId, cancellationToken)
                      ?? throw new NotFoundProblemDetailsException("Paragraph not found");

            _db.Paragraphs.Remove(paragraph);

            await _db.SaveChangesAsync(cancellationToken);

            await _db.FixParagraphOrdering(request.ParaId.PublicationId, Now, cancellationToken);
        }

        /// <inheritdoc />
        public DeleteParagraphHandler(PublicationDbContext db, IClock clock) : base(db, clock)
        {

        }
    }
}

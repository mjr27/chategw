using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence.Entities;
using Egw.PubManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using WhiteEstate.DocFormat;
using Egw.PubManagement.Application.Extensions;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <inheritdoc />
    public class AddParagraphHandler : ApplicationCommandHandler<AddParagraphInput>
    {
        /// <inheritdoc />
        public override async Task Handle(AddParagraphInput request, CancellationToken cancellationToken)
        {
            Publication _ = await _db.Publications
                .FirstOrDefaultAsync(r => r.PublicationId == request.PublicationId, cancellationToken)
                ?? throw new NotFoundProblemDetailsException("Publication not found");

            int paragraphId = 1;
            Paragraph[] paragraphs = await _db.Paragraphs
                .Where(r => r.PublicationId == request.PublicationId)
                .OrderBy(r => r.Order)
                .ToArrayAsync(cancellationToken);

            if (paragraphs.Length > 0)
                paragraphId = paragraphs.Max(r => r.ParagraphId) + 1;

            int? headingLevel = WemlExtensions.GetHeadingLevel(request.Content);
            bool isReferenced = WemlExtensions.IsReferenced(request.Content);

            var newParagraph = new Paragraph(new ParaId(request.PublicationId, paragraphId), Now);
            newParagraph.ChangeContent(request.Content, headingLevel, isReferenced, Now);

            if (request.Before is null)
            {
                newParagraph.ChangeOrder(int.MaxValue, Now);
            }
            else
            {
                Paragraph paragraphBefore = paragraphs
                    .Where(r => r.ParaId.ElementId == request.Before)
                    .FirstOrDefault() ?? throw new NotFoundProblemDetailsException("Paragraph before not found");

                int order = paragraphBefore.Order;

                foreach (Paragraph paragraph in paragraphs.Where(r => r.Order == order))
                {
                    if (paragraph.Order >= order)
                    {
                        paragraph.ChangeOrder(paragraph.Order + 1 + 1000000, Now);
                    }
                }

                newParagraph.ChangeOrder(order + 1000000, Now);
            }

            await _db.Paragraphs.AddAsync(newParagraph, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            await _db.FixParagraphOrdering(request.PublicationId, Now, cancellationToken);
        }

        /// <inheritdoc />
        public AddParagraphHandler(PublicationDbContext db, IClock clock) : base(db, clock)
        {

        }
    }
}

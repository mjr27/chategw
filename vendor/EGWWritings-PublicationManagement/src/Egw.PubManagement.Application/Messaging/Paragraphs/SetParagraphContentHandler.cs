using Egw.PubManagement.Core.Problems;
using Egw.PubManagement.Core.Services;
using Egw.PubManagement.Persistence;
using Egw.PubManagement.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

using WhiteEstate.DocFormat;

namespace Egw.PubManagement.Application.Messaging.Paragraphs
{
    /// <inheritdoc />
    public class SetParagraphContentHandler : ApplicationCommandHandler<SetParagraphContentInput>
    {
        /// <inheritdoc />
        public override async Task Handle(SetParagraphContentInput request, CancellationToken cancellationToken)
        {
            Paragraph paragraph = await _db.Paragraphs
                      .FirstOrDefaultAsync(r => r.ParaId == request.ParaId, cancellationToken)
                      ?? throw new NotFoundProblemDetailsException("Paragraph not found");

            int? headingLevel = WemlExtensions.GetHeadingLevel(request.Content);
            bool isReferenced = WemlExtensions.IsReferenced(request.Content);

            paragraph.ChangeContent(request.Content, headingLevel, isReferenced, Now);
            await _db.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public SetParagraphContentHandler(PublicationDbContext db, IClock clock) : base(db, clock)
        {

        }
    }
}

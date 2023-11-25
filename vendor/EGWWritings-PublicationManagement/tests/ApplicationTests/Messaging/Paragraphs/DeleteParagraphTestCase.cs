using Egw.PubManagement.Application.Messaging.Paragraphs;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;

namespace ApplicationTests.Messaging.Paragraphs
{
    public class DeleteParagraphTestCase : MediatorTestCase
    {
        public DeleteParagraphTestCase(DatabaseFixture fixture) : base(fixture)
        {
            Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
            Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
            Db.Publications.Add(new Publication(1, PublicationType.Book, "en", "AA", Now));
            Db.PublicationPlacement.Add(new PublicationPlacement(1, 1, 1, Now));

            Db.Paragraphs.Add(new Paragraph(new ParaId(1, 1), Now));
            Db.SaveChanges();
        }

        [Theory]
        [InlineData(1, 0)]

        public async Task TestDeleteParagraph(int expectedBefore, int expectedAfter)
        {
            Db.Paragraphs.Should().HaveCount(expectedBefore);

            await Execute(new DeleteParagraphInput { ParaId = new ParaId(1, 1) });
    
            Db.Paragraphs.Should().HaveCount(expectedAfter);
        }
    }
}

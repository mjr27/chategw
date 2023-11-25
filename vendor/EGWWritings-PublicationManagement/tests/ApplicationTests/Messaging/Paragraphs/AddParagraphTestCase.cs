using Egw.PubManagement.Application.Messaging.Paragraphs;
using Egw.PubManagement.Persistence.Entities;

using FluentAssertions;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Enums;

namespace ApplicationTests.Messaging.Paragraphs
{
    public class AddParagraphTestCase : MediatorTestCase
    {
        public AddParagraphTestCase(DatabaseFixture fixture) : base(fixture)
        {
            Db.Languages.Add(new Language("en", "en", "en-us", false, "English", Now) { RootFolderId = 1 });
            Db.Folders.Add(new Folder("English", null, 1, "egwwritings", Now) { Id = 1 });
            Db.Publications.Add(new Publication(1, PublicationType.Book, "en", "AA", Now));
            Db.PublicationPlacement.Add(new PublicationPlacement(1, 1, 1, Now));

            Db.Paragraphs.Add(new Paragraph(new ParaId(1, 1), Now));
            Db.SaveChanges();
        }

        [Theory]
        [InlineData(1, 2)]

        public async Task TestAddParagraph(int expectedBefore, int expectedAfter)
        {
            Db.Paragraphs.Should().HaveCount(expectedBefore);

            await Execute(new AddParagraphInput { PublicationId = 1, Content = "<w-page number=\"7\"></w-page>" });
    
            Db.Paragraphs.Should().HaveCount(expectedAfter);
        }
    }
}

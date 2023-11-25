using Egw.PubManagement.Application.Messaging.Languages;
using Egw.PubManagement.Core.Problems;

using FluentAssertions;

using WhiteEstate.DocFormat.Enums;
namespace ApplicationTests.Messaging.Languages;

public class CreateRootFolderTestCase : MediatorTestCase
{

    public CreateRootFolderTestCase(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GenerateForMissing()
    {
        await ShouldThrow<NotFoundProblemDetailsException>(new CreateRootFolderForLanguageInput { Code = "en" });
    }

    [Fact]
    public async Task GenerateForExisting()
    {
        await Execute(new CreateLanguageInput
        {
            Code = "en",
            EgwCode = "en",
            Bcp47Code = "en",
            Title = "English",
            Direction = WemlTextDirection.LeftToRight
        });
        await Execute(new CreateRootFolderForLanguageInput { Code = "en" });
        Db.Folders.Should().HaveCount(1);
        Db.Languages.Single().RootFolderId.Should().NotBeNull();
        await ShouldThrow<ConflictProblemDetailsException>(new CreateRootFolderForLanguageInput { Code = "en" });
    }

}
using System.Text.Json;
using System.Text.Json.Nodes;

using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

using WhiteEstate.DocFormat;
using WhiteEstate.DocFormat.Json;
using WhiteEstate.DocFormat.Serialization;
namespace Egw.ImportUtils.Commands;

[Command("convert-json", Description = "Converts to json")]
public class ConvertToJsonCommand : ICommand
{
    [CommandParameter(0, Description = "Input filenames")]
    public required FileInfo[] Files { get; init; }

    [CommandOption("dry", 'd', Description = "Dry run")]
    public bool DryRun { get; init; } = false;

    public ValueTask ExecuteAsync(IConsole console)
    {
        var parser = new HtmlParser();
        var wemlDeserializer = new WemlDeserializer();
        var wemlSerializer = new WemlSerializer();
        var jsonSerializer = new WemlJsonSerializer();
        var jsonDeserializer = new WemlJsonDeserializer();
        var options = new JsonSerializerOptions { WriteIndented = true };
        foreach (FileInfo file in Files)
        {
            using Stream f = file.OpenRead();
            string outputFilename = Path.ChangeExtension(file.FullName, "json");
            IHtmlDocument htmlDocument = parser.ParseDocument(f);
            WemlDocument wemlDocument = wemlDeserializer.Deserialize(htmlDocument);
            JsonObject jsonDocument = jsonSerializer.Serialize(wemlDocument);

            using (FileStream output = File.OpenWrite(outputFilename))
            {
                JsonSerializer.Serialize(output, jsonDocument, options);
            }

            if (DryRun)
            {
                string fn2 = Path.ChangeExtension(file.FullName, ".2.html");
                using FileStream f2 = File.OpenWrite(fn2);
                WemlDocument deserialized = jsonDeserializer.DeserializeDocument(jsonDocument);
                IHtmlDocument serialized = wemlSerializer.Serialize(deserialized);
                using var writer = new StreamWriter(f2);
                serialized.ToHtml(writer, new HtmlMarkupFormatter());
            }
        }

        return ValueTask.CompletedTask;
    }
}
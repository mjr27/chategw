using System.Runtime.CompilerServices;
using System.Text.Json;
using Azure.AI.OpenAI;
using ChatEgw.UI.Application.Models;
using Microsoft.Extensions.Configuration;

namespace ChatEgw.UI.Application.Impl;

internal class OpenAiInstructGenerationServiceImpl : IInstructGenerationService
{
    private readonly OpenAIClient _openai;
    private readonly string _prompt;

    public OpenAiInstructGenerationServiceImpl(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("OpenAI");
        _openai = new OpenAIClient(section.GetValue<string>("Key"));
        _prompt = section.GetValue<string?>("PromptTemplate")
                  ?? """
                     Synthesize a comprehensive answer from the text above for the given question.
                     Provide a clear and concise response that summarizes the key points and information presented
                     in the text.
                     Your answer should be in your own words and be no longer than 100 words.\n\n

                     Question: #QUERY#

                     Related text:
                     #TEXT#


                     """;
    }

    public async IAsyncEnumerable<string> AutoComplete(string query, List<AnswerResponse> answers,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        string prompt = GeneratePrompt(query, answers);
        prompt = LimitWords(prompt, 2500);
        var options = new CompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo-instruct",
            Temperature = 0.1f,
            MaxTokens = 200,
            Prompts =
            {
                prompt
            }
        };
        StreamingResponse<Completions>? streaming =
            await _openai.GetCompletionsStreamingAsync(options, cancellationToken);
        Console.WriteLine("BEFORE 1");
        await foreach (Completions? choice in streaming)
        {
            Console.WriteLine(JsonSerializer.Serialize(choice));
            if (choice is null)
            {
                break;
            }

            yield return choice.Choices[0].Text;
        }

        Console.WriteLine("AFTER 1");
    }

    private string LimitWords(string prompt, int maxCount)
    {
        return string.Join(" ", prompt.Split(' ').Take(maxCount)) + "\n\n";
    }

    private string GeneratePrompt(string query, IReadOnlyCollection<AnswerResponse> answers)
    {
        string answer = _prompt.Replace("#QUERY#", query)
            .Replace("#TEXT#", string.Join("\n", answers.Select(r => r.Content)));
        return answer;
    }
}
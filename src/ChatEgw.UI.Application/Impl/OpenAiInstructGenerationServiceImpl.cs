using System.Runtime.CompilerServices;
using Azure.AI.OpenAI;
using ChatEgw.UI.Application.Models;
using Microsoft.Extensions.Configuration;

namespace ChatEgw.UI.Application.Impl;

internal class OpenAiInstructGenerationServiceImpl : IInstructGenerationService
{
    private readonly OpenAIClient _openai;
    private readonly string _prompt;

    private const int ResponseTokenCount = 250;

    public OpenAiInstructGenerationServiceImpl(IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("OpenAI");
        _openai = new OpenAIClient(section.GetValue<string>("Key"));
        _prompt = section.GetValue<string?>("PromptTemplate")
                  ?? """
                     Synthesize a comprehensive answer from the text above for the given question.
                     Each paragraph of text ends with reference number. 
                     Provide a clear and concise response that summarizes the key points and information presented
                     in the text. Include quoted reference numbers at the end of your answer.
                     If no answer can be found, please write "No answer found".
                     Your answer should be in your own words and be no longer than #COUNT# words.\n\n

                     Question: #QUERY#

                     Related text:
                     #TEXT#
                     """;
    }

    public string GetPrompt(string query, List<AnswerResponse> answers)
    {
        string prompt = GeneratePrompt(query, answers);
        prompt = LimitWords(prompt, 3000 );
        return prompt;
    }

    public async IAsyncEnumerable<string> AutoComplete(string query, List<AnswerResponse> answers,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var options = new CompletionsOptions
        {
            DeploymentName = "gpt-3.5-turbo-instruct",
            Temperature = 0.01f,
            MaxTokens = ResponseTokenCount + 50,
            Prompts =
            {
                GetPrompt(query, answers)
            }
        };
        StreamingResponse<Completions>? streaming =
            await _openai.GetCompletionsStreamingAsync(options, cancellationToken);
        await foreach (Completions? choice in streaming)
        {
            if (choice is null)
            {
                break;
            }

            yield return  choice.Choices[0].Text;
        }
    }


    private static string LimitWords(string prompt, int maxCount)
    {
        return string.Join(" ", prompt.Split(' ').Take(maxCount * 3 / 4)) + "\n\n";
    }

    private string GeneratePrompt(string query, IEnumerable<AnswerResponse> answers)
    {
        var commonText = string.Join("\n", answers
            .Select((r, i) => r.Content + " ( " + i + " )"));
        string answer = _prompt.Replace("#QUERY#", query)
                            .Replace("#TEXT#", commonText)
                            .Replace("#COUNT#", ResponseTokenCount.ToString())
                        + "\n\n\n";
        return answer;
    }
}
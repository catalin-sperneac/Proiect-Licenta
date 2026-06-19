using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace TriviaConfiguration.Services;

public static class ApiService
{
    private const string ApiKey = "API key";
    private static readonly HttpClient http = new();

    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private const int MaxChars = 20000;

    public static async Task<string> GenerateTriviaAsync(
        int mcCount,
        int inputCount,
        string additionalInstructions,
        string language,
        string? filePath
    )
    {
        string fileText = "";

        if (!string.IsNullOrWhiteSpace(filePath))
        {
            var info = new FileInfo(filePath);

            if (info.Length > MaxFileSize)
                throw new Exception("File too large (max 10MB).");

            fileText = ExtractFileText(filePath);

            if (fileText.Length > MaxChars)
                fileText = fileText[..MaxChars];
        }

        var systemPrompt = $@"
You generate trivia questions in a strict machine-readable format.

Multiple choice format:
question_text/answer1/answer2/answer3/answer4/correct_answer

Input format:
question_text/correct_answer

Rules:
- No explanations
- No numbering
- No bullet points
- No extra text
- question_text, answer1, answer2, answer3, answer4 and correct_answer shouldn't contain the character '/'
- answer1 MUST be the correct answer
- answers from input questions must be positive integers (>=0)
- Generate exactly {mcCount} multiple choice and {inputCount} input questions

Language: {language}

Extra:
{additionalInstructions}

FILE CONTENT:
{fileText}
";

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = systemPrompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(request);

        var response = await http.PostAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={ApiKey}",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        var result = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(result);

        using var doc = JsonDocument.Parse(result);

        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? "";
    }

    //FILE EXTRACTOR
    private static string ExtractFileText(string path)
    {
        string ext = Path.GetExtension(path).ToLower();

        return ext switch
        {
            ".txt" => File.ReadAllText(path),

            ".pdf" => ExtractPdf(path),

            ".docx" => ExtractDocx(path),

            ".pptx" => ExtractPptx(path),

            _ => "Unsupported file type"
        };
    }

    //PDF
    private static string ExtractPdf(string path)
    {
        var sb = new StringBuilder();

        using var doc = PdfDocument.Open(path);

        foreach (var page in doc.GetPages())
            sb.AppendLine(page.Text);

        return sb.ToString();
    }

    //DOCX
    private static string ExtractDocx(string path)
    {
        var sb = new StringBuilder();

        using var doc = WordprocessingDocument.Open(path, false);

        var body = doc.MainDocumentPart?.Document.Body;

        if (body == null)
            return "";

        sb.Append(body.InnerText);

        return sb.ToString();
    }

    //PPTX
    private static string ExtractPptx(string path)
    {
        var sb = new StringBuilder();

        using var doc = PresentationDocument.Open(path, false);

        var slides = doc.PresentationPart?.SlideParts;

        if (slides == null)
            return "";

        foreach (var slide in slides)
        {
            foreach (var text in slide.Slide.Descendants<A.Text>())
            {
                sb.AppendLine(text.Text);
            }
        }

        return sb.ToString();
    }
}
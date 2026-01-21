using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace MyZombieProject.Services;

public sealed class GeminiService
{
    private readonly IHttpClientFactory _httpClientFactory;

    // cachea vald modell så vi inte listar modeller varje gång
    private static string? _cachedModelName;

    public GeminiService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> AskSurvivalAdvisorAsync(
        string userQuestion,
        string apiKey,
        string? preferredModelFromSettings = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            return "Gemini API key saknas (GeminiApiKey i settings.json).";

        if (string.IsNullOrWhiteSpace(userQuestion))
            return "Du måste skriva en fråga.";

        var http = _httpClientFactory.CreateClient("gemini");
        http.DefaultRequestHeaders.Remove("x-goog-api-key");
        http.DefaultRequestHeaders.Add("x-goog-api-key", apiKey);

        // ✅ välj en modell som faktiskt finns för DIN nyckel
        var modelName = await ResolveModelAsync(http, preferredModelFromSettings);

        if (string.IsNullOrWhiteSpace(modelName))
            return "Hittade ingen Gemini-modell som stöder generateContent för din nyckel.";

        var systemInstruction = """
Du är "Shelter Advisor" i en fiktiv postapokalyptisk överlevnadssimulator.
Din uppgift är att ge säkra, icke-våldsamma råd för vardagsöverlevnad i ett skyddsrum: vatten, mat, värme, hygien, sjukdomsförebyggande, logistik och gruppmoral.

Säkerhetsregler:
- Inga vapen, inget våld, inga instruktioner för att skada människor.
- Inga sprängämnen, inga farliga kemikalier, inget olagligt.
- Om användaren frågar om farliga saker: avböj och ge säkra alternativ inom shelter-logistik.

Svara alltid på svenska och i detta format (skriv alla rubriker):
1) Snabb bedömning (max 2 meningar)
2) Prioriterad checklista (8 punkter)
3) Vattenplan (liter/person/dygn + konkreta regler)
4) Matplan (ransonering + enkel tillagning + energi)
5) Värme & skydd mot kyla/regn (säkra åtgärder)
6) Hygien & sjukdom (rutiner + förebyggande)
7) Grupp & moral (2–4 råd)
8) Nästa fråga (1 fråga)

VIKTIGT: Skriv ett komplett svar och avsluta med texten: "[SLUT]".
""";

        var body = new GenerateContentRequest
        {
            SystemInstruction = new SystemInstruction
            {
                Parts = new[] { new Part { Text = systemInstruction } }
            },
            Contents = new[]
            {
                new Content
                {
                    Role = "user",
                    Parts = new[] { new Part { Text = userQuestion } }
                }
            },
            GenerationConfig = new GenerationConfig
            {
                Temperature = 0.6,
                MaxOutputTokens = 2048
            }
        };

        try
        {
            // modelName brukar redan komma som "models/xxx"
            var endpoint = $"v1beta/{modelName}:generateContent?key={apiKey}";

            // 1) första försöket
            var resp = await http.PostAsJsonAsync(endpoint, body);

            if (!resp.IsSuccessStatusCode)
            {
                var errText = await resp.Content.ReadAsStringAsync();
                return $"Gemini API error ({(int)resp.StatusCode}): {errText}";
            }

            var data = await resp.Content.ReadFromJsonAsync<GenerateContentResponse>();
            var text = ExtractAllText(data);

            var finish = data?.Candidates?.FirstOrDefault()?.FinishReason ?? "n/a";
            var block = data?.PromptFeedback?.BlockReason ?? "none";

            // Om svaret är kort eller saknar [SLUT], be om fortsättning (post-apoc scenario behålls)
            if (!string.IsNullOrWhiteSpace(text) &&
                (!text.Contains("[SLUT]", StringComparison.OrdinalIgnoreCase) || text.Length < 700))
            {
                var continueBody = new GenerateContentRequest
                {
                    SystemInstruction = body.SystemInstruction,
                    Contents = new[]
    {
        // 1) Original user fråga
        new Content
        {
            Role = "user",
            Parts = new[] { new Part { Text = userQuestion } }
        },

        // 2) Modellens första svar som "assistant"
        new Content
        {
            Role = "model",
            Parts = new[] { new Part { Text = text } }
        },

        // 3) Ny user instruktion: fortsätt och repetera inte
        new Content
        {
            Role = "user",
            Parts = new[]
            {
                new Part
                {
                    Text = """
Fortsätt exakt där du slutade. Upprepa inte tidigare text.
Fortsätt med nästa rubrik/punkt och avsluta med [SLUT].
"""
                }
            }
        }
    },
                    GenerationConfig = body.GenerationConfig
                };

                var contResp = await http.PostAsJsonAsync(endpoint, continueBody);

                if (contResp.IsSuccessStatusCode)
                {
                    var contData = await contResp.Content.ReadFromJsonAsync<GenerateContentResponse>();
                    var contText = ExtractAllText(contData);

                    if (!string.IsNullOrWhiteSpace(contText))
                        text = (text + "\n\n" + contText).Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(text))
                return $"Inget svar returnerades från Gemini. (finishReason={finish}, blockReason={block})";

            // (Valfritt men bra för debug i skolan)
            return $"(Model: {modelName})\nfinishReason={finish}, blockReason={block}\n\n{text.Trim()}";
        }
        catch (Exception ex)
        {
            return "Unexpected error: " + ex.Message;
        }
    }

    private static string ExtractAllText(GenerateContentResponse? data)
    {
        return string.Join(
            "\n",
            data?.Candidates?
                .SelectMany(c => c.Content?.Parts ?? Array.Empty<Part>())
                .Select(p => p.Text)
                .Where(t => !string.IsNullOrWhiteSpace(t))
            ?? Array.Empty<string>()
        ).Trim();
    }

    private static async Task<string?> ResolveModelAsync(HttpClient http, string? preferred)
    {
        // 1) Om du sätter GeminiModel i settings.json → använd den direkt
        if (!string.IsNullOrWhiteSpace(preferred))
            return preferred.StartsWith("models/") ? preferred : $"models/{preferred}";

        // 2) Om vi redan hittat en modell tidigare → återanvänd
        if (!string.IsNullOrWhiteSpace(_cachedModelName))
            return _cachedModelName;

        // 3) Annars: lista modeller och välj en som stöder generateContent
        var models = await http.GetFromJsonAsync<ListModelsResponse>("v1beta/models");

        var candidates = models?.Models?
            .Where(m => m?.SupportedGenerationMethods?.Contains("generateContent") == true)
            .Select(m => m!.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .ToList();

        if (candidates == null || candidates.Count == 0)
            return null;

        // Prioritera “flash” om den finns, annars “pro”, annars första bästa
        string? pick =
            candidates.FirstOrDefault(n => n.Contains("flash", StringComparison.OrdinalIgnoreCase)) ??
            candidates.FirstOrDefault(n => n.Contains("pro", StringComparison.OrdinalIgnoreCase)) ??
            candidates[0];

        _cachedModelName = pick;
        return pick;
    }

    // --- DTOs ---

    private sealed class ListModelsResponse
    {
        [JsonPropertyName("models")]
        public ModelInfo[]? Models { get; set; }
    }

    private sealed class ModelInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("supportedGenerationMethods")]
        public string[]? SupportedGenerationMethods { get; set; }
    }

    private sealed class GenerateContentRequest
    {
        [JsonPropertyName("systemInstruction")]
        public SystemInstruction? SystemInstruction { get; set; }

        [JsonPropertyName("contents")]
        public Content[]? Contents { get; set; }

        [JsonPropertyName("generationConfig")]
        public GenerationConfig? GenerationConfig { get; set; }
    }

    private sealed class SystemInstruction
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
    }

    private sealed class Content
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
    }

    private sealed class Part
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    private sealed class GenerationConfig
    {
        [JsonPropertyName("temperature")]
        public double? Temperature { get; set; }

        [JsonPropertyName("maxOutputTokens")]
        public int? MaxOutputTokens { get; set; }
    }

    private sealed class GenerateContentResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[]? Candidates { get; set; }

        [JsonPropertyName("promptFeedback")]
        public PromptFeedback? PromptFeedback { get; set; }
    }

    private sealed class PromptFeedback
    {
        [JsonPropertyName("blockReason")]
        public string? BlockReason { get; set; }
    }

    private sealed class Candidate
    {
        [JsonPropertyName("content")]
        public CandidateContent? Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("safetyRatings")]
        public SafetyRating[]? SafetyRatings { get; set; }
    }

    private sealed class SafetyRating
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }

    private sealed class CandidateContent
    {
        [JsonPropertyName("parts")]
        public Part[]? Parts { get; set; }
    }
}
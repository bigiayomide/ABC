using ABC.Web.Models;
using ABC.Web.Services.Interfaces;
using Newtonsoft.Json.Linq;

namespace ABC.Web.Services.Implementations;

public class ContentService : IContentService
{
    private readonly JObject _contentObject;
    private readonly ILogger<ContentService> _logger;

    public ContentService(IWebHostEnvironment env, ILogger<ContentService> logger)
    {
        _logger = logger;

        // Use Path.Combine instead of string concatenation
        var folderPath = Path.Combine(env.WebRootPath, "content");

        try
        {
            var jsonFiles = Directory.GetFiles(folderPath, "*.json");
            if (jsonFiles is { Length: > 0 })
            {
                var contentString = File.ReadAllText(jsonFiles[0]);
                _contentObject = JObject.Parse(contentString);
            }
            else
            {
                _logger.LogError("No JSON files found in the content directory.");
                throw new FileNotFoundException("No JSON files found in the content directory.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while loading the content JSON file.");
            throw;
        }
    }

    public Section? GetPreface()
    {
        if (!IsSectionExist("preface"))
        {
            _logger.LogWarning("Preface section not found in the content.");
            return null;
        }

        var preface = GetSection("preface");
        return preface;
    }

    public Section? GetSection(string sectionName)
    {
        var jsonObject = GetJsonObject();
        if (jsonObject.TryGetValue(sectionName, StringComparison.OrdinalIgnoreCase, out var property))
            return property.ToObject<Section>();
        _logger.LogWarning($"Section '{sectionName}' not found in the content.");
        return null;
    }

    public bool IsLinkingComplete()
    {
        try
        {
            return ValidateNavigation(GetJsonObject());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during navigation validation.");
            return false;
        }
    }


    private JObject GetJsonObject()
    {
        if (_contentObject == null) throw new InvalidOperationException("Content JSON is not loaded.");
        return _contentObject;
    }

    private bool IsSectionExist(string sectionName)
    {
        return GetJsonObject().Properties()
            .Any(p => string.Equals(p.Name, sectionName, StringComparison.OrdinalIgnoreCase));
    }

    private bool ValidateNavigation(JObject sections, string currentSection = "preface",
        HashSet<string>? visited = null)
    {
        visited ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (!visited.Add(currentSection))
            return true;

        if (!sections.TryGetValue(currentSection, StringComparison.OrdinalIgnoreCase, out var sectionToken))
        {
            _logger.LogWarning($"Section '{currentSection}' does not exist in the content.");
            return false;
        }

        var navigation = sections[currentSection]?.ToObject<Section>()?.Navigation;

        if (navigation == null) return true;
        foreach (var navItem in navigation.Where(navItem =>
                     !sections.ContainsKey(navItem.Section) ||
                     !ValidateNavigation(sections, navItem.Section, visited)))
        {
            _logger.LogWarning($"Navigation link to section '{navItem.Section}' is invalid.");
            return false;
        }

        return true;
    }
}
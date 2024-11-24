using ABC.Web.Middlewares;
using ABC.Web.Models;
using ABC.Web.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

namespace ABC.Web.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly JObject _contentObject;
        private readonly ILogger<ContentService> _logger;
        public ContentService(IWebHostEnvironment env, ILogger<ContentService> logger)
        {
            _logger = logger;
            string folderPath = env.WebRootPath + "\\content";
            try
            {
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
                if (jsonFiles == null || jsonFiles.Length == 0)
                {
                    throw new FileNotFoundException("No JSON content files found in the content directory.");
                }
                string contentString = File.ReadAllText(jsonFiles[0]);
                _contentObject = JObject.Parse(contentString);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException("No JSON content files found in the content directory.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while loading the content JSON file: {ex.Message}");
                throw;
            }
        }

        private JObject GetJsonObject()
        {
            if (_contentObject == null)
            {
                throw new InvalidOperationException("Content JSON is not loaded.");
            }
            return _contentObject;
        }

        public Section GetPreface()
        {
            if (!IsSectionExist("preface"))
            {
                _logger.LogWarning("Preface section not found in the content.");
                return null;
            }
            Section preface = GetSection("preface");
            return preface;
        }
        private bool IsSectionExist(string sectionName)
        {
            return GetJsonObject().Properties().Any(p => string.Equals(p.Name, sectionName, StringComparison.OrdinalIgnoreCase));
        }

        public Section GetSection(string sectionName)
        {
            JObject jsonObject = GetJsonObject();
            if (jsonObject.TryGetValue(sectionName, StringComparison.OrdinalIgnoreCase, out JToken property))
            {
                return property.ToObject<Section>();
            }
            _logger.LogWarning($"Section '{sectionName}' not found in the content.");
            return null;
        }

        public bool IsLinkingComplete()
        {
            try
            {
                return ValidateNavigation(GetJsonObject(), "preface");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during navigation validation.");
                return false;
            }
        }

        private bool ValidateNavigation(JObject sections, string currentSection = "preface", HashSet<string> visited = null)
        {
            visited ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (visited.Contains(currentSection))
                return true;

            visited.Add(currentSection);

            if (!sections.TryGetValue(currentSection, StringComparison.OrdinalIgnoreCase, out JToken sectionToken))
            {
                _logger.LogWarning($"Section '{currentSection}' does not exist in the content.");
                return false;
            }

            var navigation = sections[currentSection].ToObject<Section>().Navigation;

            foreach (var navItem in navigation)
            {
                if (!sections.ContainsKey(navItem.Section) || !ValidateNavigation(sections, navItem.Section, visited))
                {
                    _logger.LogWarning($"Navigation link to section '{navItem.Section}' is invalid.");
                    return false;
                }
            }

            return true;
        }
    }
}

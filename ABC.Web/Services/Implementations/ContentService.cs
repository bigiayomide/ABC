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
        private readonly IMapper _mapper;
        private readonly ILogger<ContentService> _logger;
        private HashSet<string> _visited;
        public ContentService(IWebHostEnvironment env, IMapper mapper, ILogger<ContentService> logger) 
        {
            _mapper = mapper;
            _logger = logger;
            string folderPath = env.WebRootPath + "\\content";
            try
            {
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");
                if(jsonFiles != null)
                {
                    string contentString = File.ReadAllText(jsonFiles[0]);
                    _contentObject = JObject.Parse(contentString);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }
            _visited = new();
        }

        private JObject GetJsonObject()
        {
            return _contentObject;
        }

        public Preface GetPreface()
        {
            if (!IsPrefaceExist()) return null;
            Preface preface = _mapper.Map<Preface>(GetSection("preface"));
            return preface;
        }

        private bool IsPrefaceExist()
        {
            return GetJsonObject().Properties().Any(p => string.Equals(p.Name, "preface", StringComparison.OrdinalIgnoreCase));
        }

        public Section GetSection(string propertyName)
        {
            if (GetJsonObject() is JObject obj)
            {
                if (obj.TryGetValue(propertyName, out JToken property))
                {
                    return property.ToObject<Section>();
                }
            }
            return null;
        }

        public bool IsLinkingComplete()
        {
            return ValidateNavigation(GetJsonObject(), "preface");
        }

        private bool ValidateNavigation(JObject sections, string currentSection = "preface", HashSet<string> visited = null)
        {
            // Initialize the visited set to detect circular references
            visited ??= new HashSet<string>();

            // If we've already visited this section, skip to avoid infinite loops
            if (visited.Contains(currentSection))
                return true;

            // Add the current section to the visited set
            visited.Add(currentSection);

            // Check if the current section exists in the dictionary
            if (!sections.ContainsKey(currentSection))
                return false;

            // Get the current section's navigation
            var navigation = sections[currentSection].ToObject<Section>().Navigation;

            // Recursively validate all linked sections
            foreach (var navItem in navigation)
            {
                if (!sections.ContainsKey(navItem.Section) || !ValidateNavigation(sections, navItem.Section, visited))
                    return false;
            }

            // All links are valid
            return true;
        }


    }
}

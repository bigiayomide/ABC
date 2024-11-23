using ABC.Web.Middlewares;
using ABC.Web.Models;
using ABC.Web.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

namespace ABC.Web.Services.Implementations
{
    public class ContentService : IContentService
    {
        private readonly JObject _contentObject;
        private readonly IMapper _mapper;
        private readonly ILogger<ContentService> _logger;
        public ContentService(IWebHostEnvironment env, IMapper mapper, ILogger<ContentService> logger) 
        {
            string contentString = File.ReadAllText(env.WebRootPath + "\\content\\the-adventures-of-sherlock-holmes-sample.json");
            _contentObject = JObject.Parse(contentString);
            _mapper = mapper;
            _logger = logger;
        }

        private JObject GetJsonObject()
        {
            return _contentObject;
        }

        public Preface GetPreface()
        {
            var preface = _mapper.Map<Preface>(GetSection("preface"));
            if (IsPrefaceExist()) return preface;
            return null;
        }

        public bool IsPrefaceExist()
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
    }
}

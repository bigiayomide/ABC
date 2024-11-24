using ABC.Web.Models;

namespace ABC.Web.Services.Interfaces;

public interface IContentService
{
    bool IsLinkingComplete();
    Section? GetPreface();
    Section? GetSection(string propertyName);
}
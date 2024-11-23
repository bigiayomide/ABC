using ABC.Web.Models;
using Newtonsoft.Json.Linq;

namespace ABC.Web.Services.Interfaces
{
    public interface IContentService
    {
        bool IsPrefaceExist();
        Preface GetPreface();
        Section GetSection(string propertyName);
    }
}

namespace ABC.Web.Models.Profile;

public class ContentMappingProfile : AutoMapper.Profile
{
    public ContentMappingProfile()
    {
        CreateMap<Section, Preface>().ReverseMap();
    }
}
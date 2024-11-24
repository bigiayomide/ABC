namespace ABC.Web.Models;

public class Section
{
    public string Title { get; set; }
    public List<string> Content { get; set; }
    public List<NavigationInfo> Navigation { get; set; }
}
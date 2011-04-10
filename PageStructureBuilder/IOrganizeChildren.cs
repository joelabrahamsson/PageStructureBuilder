using EPiServer.Core;

namespace PageStructureBuilder
{
    public interface IOrganizeChildren
    {
        PageReference GetParentForPage(PageData page);
    }
}

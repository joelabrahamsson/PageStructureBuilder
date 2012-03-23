using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace PageStructureBuilder
{
    /// <summary>
    /// Inherit a page type from this to automatically organise pages into folders
    /// named with the first letter of the page name.
    /// </summary>
    /// <typeparam name="TContainer">The type page that will provide page type based folders.</typeparam>
public abstract class PageTypeStructureBase<TContainer> : 
    SingleLevelStructureBase<TContainer>
    where TContainer : PageData
{
    protected override string GetContainerPageName(
        PageData childPage)
    {
        var pageType = PageType.Load(childPage.PageTypeID);
        return pageType.Name;
    }
}
}

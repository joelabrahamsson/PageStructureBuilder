using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace PageStructureBuilder
{
    public abstract class PageTypeStructureBase<TContainer> : SingleLevelStructureBase<TContainer>
        where TContainer : PageData
    {
        protected override string GetContainerPageName(PageData childPage)
        {
            var pageType = PageType.Load(childPage.PageTypeID);
            return pageType.Name;
        }
    }
}

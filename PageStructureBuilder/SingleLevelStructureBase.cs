using EPiServer.Core;
using PageTypeBuilder;

namespace PageStructureBuilder
{
    public abstract class SingleLevelStructureBase<TContainer> : TypedPageData, IOrganizeChildren
        where TContainer : PageData
    {
        public virtual PageReference GetParentForPage(PageData page)
        {
            if (page is TContainer)
            {
                return PageLink;
            }

            if (string.IsNullOrEmpty(page.PageName))
            {
                return PageLink;
            }

            var structureHelper = new StructureHelper();

            var container = structureHelper.GetOrCreateChildPage<TContainer>(PageLink, GetContainerPageName(page));
            return container.PageLink;
        }

        protected abstract string GetContainerPageName(PageData childPage);
    }
}

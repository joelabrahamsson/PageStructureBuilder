using EPiServer.Core;
using PageTypeBuilder;

namespace PageStructureBuilder
{
    /// <summary>
    /// Base class for all single folder based automatic page organisation.
    /// </summary>
    /// <typeparam name="TContainer">The type of the container (folder) pages.</typeparam>
    public abstract class SingleLevelStructureBase<TContainer> 
        : TypedPageData, IOrganizeChildren
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

            var container = structureHelper
                .GetOrCreateChildPage<TContainer>(
                PageLink, GetContainerPageName(page));
            return container.PageLink;
        }

        protected abstract string GetContainerPageName(PageData childPage);
    }
}

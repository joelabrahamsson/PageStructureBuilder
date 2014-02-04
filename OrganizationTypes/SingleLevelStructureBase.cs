using EPiServer.Core;
using PageStructureBuilder.Extensions;
using PageTypeBuilder;

namespace PageStructureBuilder.OrganizationTypes
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

            var container = PageLink.GetOrCreateChildPage<TContainer>(GetContainerPageName(page));
            return container.PageLink;
        }

        /// <summary>
        /// Derived classes should override this method to control
        /// the naming convention of the generated containing pages (i.e. folders).
        /// </summary>
        /// <param name="childPage">The child page for which to find the folder name.</param>
        /// <returns>The name of the folder this page should be placed in.</returns>
        protected abstract string GetContainerPageName(PageData childPage);
    }
}

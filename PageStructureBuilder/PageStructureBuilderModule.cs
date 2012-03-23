using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;

namespace PageStructureBuilder
{
    /// <summary>
    /// Class for automatically putting pages in a predefined folder hierarchy<br/>
    /// on the fly as they created / moved. E.g. root/News/yyyy/mm/dd/pagename.<br/>
    /// The structure is defined by the page type, which must implement <see cref="IOrganizeChildren"/>.<br/>
    /// See also <a href="http://joelabrahamsson.com/entry/automatically-organize-episerver-pages">http://joelabrahamsson.com/entry/automatically-organize-episerver-pages</a>
    /// </summary>
    [ModuleDependency(typeof(PageTypeBuilder.Initializer))]
    public class PageStructureBuilderModule : IInitializableModule
    {
        /// <summary>
        /// Attaches to EpiServer events for page creation / moving
        /// </summary>
        /// <param name="context">The context.</param>
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatingPage += DataFactoryCreatingPage;
            DataFactory.Instance.MovedPage += DataFactoryMovedPage;
        }

        /// <summary>
        /// Detaches from EpiServer events for page creation / moving
        /// </summary>
        /// <param name="context">The context.</param>
        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatingPage -= DataFactoryCreatingPage;
            DataFactory.Instance.MovingPage -= DataFactoryMovedPage;
        }

        public void Preload(string[] parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Event handling method which intercepts pages being created
        /// and if the original parent implements <see cref="IOrganizeChildren"/>
        /// then changes the new page's parent to fit rules for that parent's type.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EPiServer.PageEventArgs"/> instance containing the event data.</param>
        private static void DataFactoryCreatingPage(object sender, PageEventArgs e)
        {
            // change the parent this page is going to be added under to the new parent
            e.Page.ParentLink = GetNewParent(e.Page.ParentLink, e.Page);
        }

        /// <summary>
        /// Event handling method which intercepts pages being moved
        /// and if the original parent implements <see cref="IOrganizeChildren"/>
        /// then changes destination parent to fit rules for that parent's type.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EPiServer.PageEventArgs"/> instance containing the event data.</param>
        private static void DataFactoryMovedPage(object sender, PageEventArgs e)
        {
            var page = DataFactory.Instance.GetPage(e.PageLink);
            var parentLink = GetNewParent(e.TargetLink, page); // todo: explain why move uses "getpage(pagelink)", but create uses "page" as param to getnewparent
            if (!PageReference.IsValue(parentLink))
            {
                return; //no new parent found
            }

            if (e.TargetLink.CompareToIgnoreWorkID(parentLink))
            {
                return; //parent is unchanged from original request
            }
            DataFactory.Instance.Move(page.PageLink, parentLink, AccessLevel.NoAccess, AccessLevel.NoAccess);
        }

        /// <summary>
        /// Finds the highest parent in the hierarchy of parents until an entry that doesn't
        /// implement <see cref="IOrganizeChildren"/> is found (or a repeated parent is found).
        /// </summary>
        /// <param name="originalParentLink">The original parent link.</param>
        /// <param name="page">The page.</param>
        /// <returns>The highest parent in the hierarchy that implements IOrganizeChildren</returns>
        private static PageReference GetNewParent(PageReference originalParentLink, PageData page)
        {
            var queriedParents = new List<PageReference>();
            var organizingParent = GetPageAsIOrganizeChildren(originalParentLink);
            var parentLink = originalParentLink;

            while (organizingParent != null 
                && !ParentAlreadyQueried(queriedParents, parentLink))
            {
                queriedParents.Add(parentLink);
                var newParentLink = organizingParent.GetParentForPage(page);
                if (PageReference.IsValue(newParentLink))
                {
                    parentLink = newParentLink;
                }
                organizingParent = GetPageAsIOrganizeChildren(parentLink);
            }
            return parentLink;
        }

        /// <summary>
        /// Gets the page as a <see cref="IOrganizeChildren"/> if it's page type
        /// implements this interface, otherwise null is returned.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        /// <returns></returns>
        private static IOrganizeChildren GetPageAsIOrganizeChildren(PageReference pageLink)
        {
            if (PageReference.IsNullOrEmpty(pageLink))
            {
                return null;
            }

            return DataFactory.Instance.GetPage(pageLink) as IOrganizeChildren;
        }

        /// <summary>
        /// Checks if a parent has already been queried. This would happen
        /// if a parent returns itself as a parent.<br/>
        /// Based on <see cref="PageReference.CompareToIgnoreWorkID"/>,
        /// - will return true there is a match where ID and RemoteSite
        /// are the same, otherwise false.
        /// </summary>
        /// <param name="queriedParents">The list of parents to search.</param>
        /// <param name="parentLink">The parent to check for.</param>
        /// <returns>True if the parent has been found in the list, false otherwise.</returns>
        private static bool ParentAlreadyQueried(IEnumerable<PageReference> queriedParents, PageReference parentLink)
        {
            return queriedParents.Any(p => p.CompareToIgnoreWorkID(parentLink));
        }
    }
}

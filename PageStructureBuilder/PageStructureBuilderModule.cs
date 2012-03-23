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
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatingPage += DataFactoryCreatingPage;
            DataFactory.Instance.MovedPage += DataFactoryMovedPage;
        }

        private static void DataFactoryCreatingPage(object sender, PageEventArgs e)
        {
            var parentLink = e.Page.ParentLink;
            var page = e.Page;
            parentLink = GetNewParent(parentLink, page);

            e.Page.ParentLink = parentLink;
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

            var organizingParent = GetChildrenOrganizer(originalParentLink);

            PageReference parentLink = originalParentLink;
            while (organizingParent != null 
                && !ParentAlreadyQueried(queriedParents, parentLink))
            {
                queriedParents.Add(parentLink);
                var newParentLink = organizingParent.GetParentForPage(page);
                if (PageReference.IsValue(newParentLink))
                {
                    parentLink = newParentLink;
                }
                organizingParent = GetChildrenOrganizer(parentLink);
            }
            return parentLink;
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

        /// <summary>
        /// Gets the page as a <see cref="IOrganizeChildren"/> if it's page type
        /// implements this interface, otherwise null is returned.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        /// <returns></returns>
        private static IOrganizeChildren GetChildrenOrganizer(PageReference pageLink)
        {
            if (PageReference.IsNullOrEmpty(pageLink))
            {
                return null;
            }

            return DataFactory.Instance.GetPage(pageLink) as IOrganizeChildren;
        }

        private static void DataFactoryMovedPage(object sender, PageEventArgs e)
        {
            var parentLink = e.TargetLink;
            var page = DataFactory.Instance.GetPage(e.PageLink);
            parentLink = GetNewParent(parentLink, page);

            if (PageReference.IsValue(parentLink) && !e.TargetLink.CompareToIgnoreWorkID(parentLink))
            {
                DataFactory.Instance.Move(page.PageLink, parentLink, AccessLevel.NoAccess, AccessLevel.NoAccess);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatingPage -= DataFactoryCreatingPage;
            DataFactory.Instance.MovingPage -= DataFactoryMovedPage;
        }

        public void Preload(string[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}

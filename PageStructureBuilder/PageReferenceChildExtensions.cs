using System;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using PageTypeBuilder;

namespace PageStructureBuilder
{
    /// <summary>
    /// Contains extension methods for PageReference to simplify finding / creating child pages.<br/>
    /// Allows code to be agnostic as to whether the page already exists.<br/>
    /// See also <a href="http://joelabrahamsson.com/entry/automatically-organize-episerver-pages">http://joelabrahamsson.com/entry/automatically-organize-episerver-pages</a>
    /// </summary>
    public static class PageReferenceChildExtensions
    {
        /// <summary>
        /// Gets the existing child page or create it and return it if it is new.
        /// </summary>
        /// <typeparam name="TChildType">The type of page to find / create.</typeparam>
        /// <param name="parent">The parent link.</param>
        /// <param name="pageName">Name of the page to find / create.</param>
        /// <param name="action">Whether to publish the page immediately if it is new.</param>
        /// <returns>The new / existing page at the specified location.</returns>
        public static TChildType GetOrCreateChildPage<TChildType>(this PageReference parent, string pageName, SaveAction action = SaveAction.Publish)
            where TChildType : PageData
        {
            return parent.GetExistingChild<TChildType>(pageName) ?? parent.CreateChild<TChildType>(pageName, action);
        }

        /// <summary>
        /// Find an existing child page by type and name.
        /// </summary>
        /// <typeparam name="TChildType">The type of the result.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="pageName">Name of the page to find.</param>
        /// <returns></returns>
        public static TChildType GetExistingChild<TChildType>(this PageReference parent, string pageName)
            where TChildType : PageData
        {
            var children = DataFactory.Instance.GetChildren(parent);
            return children
                .OfType<TChildType>()
                .FirstOrDefault(c => c.PageName.Equals(
                    pageName, StringComparison.InvariantCulture));
        }

        /// <summary>
        /// Creates a child page of the specified type.
        /// </summary>
        /// <typeparam name="TChildType">The type of the result.</typeparam>
        /// <param name="parent">The parent.</param>
        /// <param name="pageName">Name of the new page.</param>
        /// <param name="action">Whether to publish the page immediately.</param>
        /// <returns></returns>
        public static TChildType CreateChild<TChildType>(this PageReference parent, string pageName, SaveAction action = SaveAction.Publish)
            where TChildType : PageData
        {
            var resultPageTypeId = PageTypeResolver.Instance.GetPageTypeID(typeof(TChildType));
            if (resultPageTypeId == null)
            {
                throw new Exception(String.Format("PageTypeID not found for {0}", typeof(TChildType)));
            }
            var child = DataFactory.Instance.GetDefaultPageData(parent, resultPageTypeId.Value) as TChildType;
            if (child == null)
            {
                throw new Exception(string.Format(
                    "Failed to GetDefaultPageData for parent {0} and PageTypeID {1}", parent, resultPageTypeId));
            }
            child.PageName = pageName;
            DataFactory.Instance.Save(child, action, AccessLevel.NoAccess);
            return child;
        }
    }
}

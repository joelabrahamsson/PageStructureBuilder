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
    /// Helper to simplify creating child pages.<br/>
    /// Allows code to be agnostic as to whether the page
    /// already exists.<br/>
    /// See also <a href="http://joelabrahamsson.com/entry/automatically-organize-episerver-pages">http://joelabrahamsson.com/entry/automatically-organize-episerver-pages</a>
    /// </summary>
    public class StructureHelper
    {
        /// <summary>
        /// Gets the existing child page or create it and return it if it is new.
        /// </summary>
        /// <typeparam name="TResult">The type of page to find / create.</typeparam>
        /// <param name="parentLink">The parent link.</param>
        /// <param name="pageName">Name of the page to find / create.</param>
        /// <returns>The new / existing page at the specified location.</returns>
        public virtual TResult GetOrCreateChildPage<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            return GetExistingChild<TResult>(parentLink, pageName) ?? CreateChild<TResult>(parentLink, pageName);
        }

        private static TResult GetExistingChild<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            var children = DataFactory.Instance.GetChildren(parentLink);
            return children
                .OfType<TResult>()
                .FirstOrDefault(c => c.PageName.Equals(
                    pageName, StringComparison.InvariantCulture));
        }

        private static TResult CreateChild<TResult>(
            PageReference parentLink, string pageName)
            where TResult : PageData
        {
            var resultPageTypeId = PageTypeResolver.Instance
                .GetPageTypeID(typeof(TResult));
            if (resultPageTypeId == null)
            {
                throw new Exception(String.Format("PageTypeID not found for {0}", typeof(TResult)));
            }
            var child = DataFactory.Instance.GetDefaultPageData(
                parentLink, resultPageTypeId.Value) as TResult;
            if (child == null)
            {
                throw new Exception(string.Format(
                    "Failed to GetDefaultPageData for parent {0} and PageTypeID {1}", parentLink, resultPageTypeId));
            }
            child.PageName = pageName;
            DataFactory.Instance.Save(
                child, SaveAction.Publish, AccessLevel.NoAccess);
            return child;
        }
    }
}

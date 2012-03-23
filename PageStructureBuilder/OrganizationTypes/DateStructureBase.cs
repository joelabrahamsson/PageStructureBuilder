using System;
using EPiServer.Core;
using PageStructureBuilder.Extensions;
using PageTypeBuilder;

namespace PageStructureBuilder.OrganizationTypes
{
    /// <summary>
    /// Inherit from this page type to make your pages automatically
    /// move themselves to the date based folder structure of yyyy/mm/dd/pagename<br/>
    /// The default date source is creation date. To change this override <see cref="GetStructureDate"/>.
    /// </summary>
    /// <typeparam name="TYear">Desired PageData type for year level container pages.</typeparam>
    /// <typeparam name="TMonth">Desired PageData type for month level container pages.</typeparam>
    /// <typeparam name="TDay">Desired PageData type for day level container pages.</typeparam>
    /// <example>Usage: <code>
    /// [PageType]
    /// public class Container : TypedPageData { }
    /// [PageType]
    /// public class DateStructure : DateStructureBase&lt;Container, Container, Container&gt; { }
    /// </code></example>
    /// <remarks>
    /// See also <a href="http://joelabrahamsson.com/entry/automatically-organize-episerver-pages-part-2/">http://joelabrahamsson.com/entry/automatically-organize-episerver-pages-part-2/</a>
    /// </remarks>
    public abstract class DateStructureBase<TYear, TMonth, TDay> 
        : TypedPageData, IOrganizeChildren
        where TYear : PageData 
        where TMonth : PageData 
        where TDay : PageData
    {
        public PageReference GetParentForPage(PageData page)
        {
            if (page is TYear)
            {
                return PageLink;
            }

            var pageDate = GetStructureDate(page);
            var yearPage = PageLink.GetOrCreateChildPage<TYear>(pageDate.Year.ToString());
            var monthPage = yearPage.PageLink.GetOrCreateChildPage<TMonth>(pageDate.Month.ToString());
            var dayPage = monthPage.PageLink.GetOrCreateChildPage<TDay>(pageDate.Day.ToString());
            return dayPage.PageLink;
        }

        /// <summary>
        /// Returns the date that will be used when creating the page's folder
        /// hierarchy. Default implementation uses the Created property of the page.<br/>
        /// Override this method to use a different date for the hierarchy.
        /// </summary>
        /// <param name="pageToGetParentFor">The page to get parent for.</param>
        /// <returns></returns>
        protected virtual DateTime GetStructureDate(
            PageData pageToGetParentFor)
        {
            return pageToGetParentFor.Created;
        }
    }
}

using System;
using EPiServer.Core;
using PageTypeBuilder;

namespace PageStructureBuilder
{
    public abstract class DateStructureBase<TYear, TMonth, TDay> : TypedPageData, IOrganizeChildren
        where TYear : PageData where TMonth : PageData where TDay : PageData
    {
        public PageReference GetParentForPage(PageData page)
        {
            if (page is TYear)
            {
                return PageLink;
            }

            var pageDate = GetStructureDate(page);
            var structureHelper = new StructureHelper();
            var yearPage = structureHelper.GetOrCreateChildPage<TYear>(PageLink, pageDate.Year.ToString());
            var monthPage = structureHelper.GetOrCreateChildPage<TMonth>(yearPage.PageLink, pageDate.Month.ToString());
            var dayPage = structureHelper.GetOrCreateChildPage<TDay>(monthPage.PageLink, pageDate.Day.ToString());
            return dayPage.PageLink;
        }

        protected virtual DateTime GetStructureDate(PageData pageToGetParentFor)
        {
            return pageToGetParentFor.Created;
        }
    }
}

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
    [ModuleDependency(typeof(PageTypeBuilder.Initializer))]
    public class PageStructureBuilderModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.CreatingPage += DataFactoryCreatingPage;
            DataFactory.Instance.MovedPage += DataFactoryMovedPage;
        }

        void DataFactoryCreatingPage(object sender, PageEventArgs e)
        {
            var parentLink = e.Page.ParentLink;
            var page = e.Page;
            parentLink = GetNewParent(parentLink, page);

            e.Page.ParentLink = parentLink;
        }

PageReference GetNewParent(
    PageReference originalParentLink, PageData page)
{
    var queriedParents = new List<PageReference>();

    var organizingParent = GetChildrenOrganizer(originalParentLink);

    PageReference parentLink = originalParentLink;
    while (organizingParent != null 
        && ListContains(queriedParents, parentLink))
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

bool ListContains(List<PageReference> queriedParents, PageReference parentLink)
{
    return queriedParents.Count(p => p.CompareToIgnoreWorkID(parentLink)) == 0;
}

IOrganizeChildren GetChildrenOrganizer(PageReference pageLink)
{
    if (PageReference.IsNullOrEmpty(pageLink))
    {
        return null;
    }

    return DataFactory.Instance.GetPage(pageLink) as IOrganizeChildren;
}

        void DataFactoryMovedPage(object sender, PageEventArgs e)
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

using EPiServer.Core;

namespace PageStructureBuilder
{
    public abstract class AlphabeticalStructureBase<TCharacter> 
        : SingleLevelStructureBase<TCharacter>
        where TCharacter : PageData, new()
    {
        protected override string GetContainerPageName(PageData childPage)
        {
            return childPage.PageName[0].ToString().ToUpperInvariant();
        }
    }
}

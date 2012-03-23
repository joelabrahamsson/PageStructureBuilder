using EPiServer.Core;

namespace PageStructureBuilder
{
    /// <summary>
    /// Inherit a page type from this to automatically organise pages into folders
    /// named with the first letter of the page name.
    /// </summary>
    /// <typeparam name="TCharacter">The type page that will provide single character folders.</typeparam>
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

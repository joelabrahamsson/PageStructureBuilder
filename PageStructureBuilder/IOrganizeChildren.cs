using EPiServer.Core;

namespace PageStructureBuilder
{
    /// <summary>
    /// Implement this interface to create a folder (really just a page)
    /// that will automatically organise child pages of the same type into a
    /// desired hierarchy.
    /// </summary>
    public interface IOrganizeChildren
    {
        /// <summary>
        /// Gets the corrected parent page for new / moved page.<br/>
        /// This will possibly be a new folder in the hierarchy.<br/>
        /// The location is defined by the implementing class.
        /// </summary>
        /// <param name="page">The page to get a corrected parent page.</param>
        /// <returns></returns>
        PageReference GetParentForPage(PageData page);
    }
}

# About this fork

* Source at https://github.com/timabell/PageStructureBuilder
* Originally forked from https://github.com/joelabrahamsson/PageStructureBuilder

# Background

Read the following 2 part blog entry from the original author

* http://joelabrahamsson.com/entry/automatically-organize-episerver-pages
* http://joelabrahamsson.com/entry/automatically-organize-episerver-pages-part-2/

# What's different from upstream

* Code documentation added
* Code improved for readability
* Child page creation / finding turned into public extension methods of PageReference (for independent reuse).

# Usage

## Automatic folder hierarchies

1. Create a page type that implements IOrganizeChildren that will act as folders for your pages
 * Inherit one of the classes in namespace OrganizationTypes or create you own with its own rules.
1. Build and run your EpiServer application
1. In the episerver edit mode interface, under structure, add a new page (which will act as your folder),
   of the type you just created.
1. Add a new page of any type as a child of the page (folder) you just added, and watch as it is created in
   the correct place complete with any hierarchy that needed to be built.

## Child find / create extension methods

You may find it useful to use the extension methods for finding / creating child pages.

To use these, reference namespace PageStructureBuilder.Extensions and then call one of the following as
a method on a PageReference object:

* GetOrCreateChildPage(pageName)
* GetExistingChild(pageName)
* CreateChild(pageName)

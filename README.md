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

To use these, reference namespace PageStructureBuilder.Extensions and then call one of the following as a method on a PageReference object:

* GetOrCreateChildPage(pageName)
* GetExistingChild(pageName)
* CreateChild(pageName)

## Build dependencies

As well as referencing this project's dll in your project, you will also need to add this project's dependencies. 

* Install nuget http://nuget.org/
* Add the episerver nuget package source http://nuget.episerver.com/feed/packages.svc/
* Copy the nuget references from into your project https://github.com/timabell/PageStructureBuilder/blob/master/packages.config
* Enable nuget package restore to have nuget automatically fetch the dependencies.

## User side (i.e. EpiServer Admin users)

The automatically organised pages are used as follows:

* Create folder (i.e. a page) that is one of the self organising Page Types created by your developers.
* Create a new page of any type directly inside this new folder (not in any subfolders if they already exist).
* Watch with wonder as the newly created page jumps into the correct subfolder, which is created as required.

Note that the automatic organisation also applies to pages that are moved into the self-organising folder, so if you move a page into the root of the self-organising folder it will jump back into the folder designated by the developers.

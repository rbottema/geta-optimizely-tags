# Geta Tags for EPiServer

![](<http://tc.geta.no/app/rest/builds/buildType:(id:TeamFrederik_Tags_TagsDebug)/statusIcon>)
[![Platform](https://img.shields.io/badge/Platform-.NET%205.0-blue.svg?style=flat)](https://msdn.microsoft.com/en-us/library/w0x726c2%28v=vs.110%29.aspx)
[![Platform](https://img.shields.io/badge/EPiServer-%2012-orange.svg?style=flat)](http://world.episerver.com/cms/)

## Description

Geta Tags is a library that adds tagging functionality to EPiServer content.

## Features

- Define tag properties
- Query for data
- Admin page for managing tags
- Tags maintenance schedule job

See the [editor guide](docs/editor-guide.md) for more information.

## How to get started?

Start by installing NuGet package (use [EPiServer NuGet](http://nuget.episerver.com/)):

    Install-Package Geta.Tags

The latest version is compiled for .NET 4.6.1 and EPiServer 11.
Geta Tags library uses [tag-it](https://github.com/aehlke/tag-it) jQuery UI plugin for selecting tags.
To add Tags as a new property to your page types you need to use the UIHint attribute like in this example:

```csharp
[UIHint("Tags")]
public virtual string Tags { get; set; }

[TagsGroupKey("mykey")]
[UIHint("Tags")]
public virtual string Tags { get; set; }

[CultureSpecific]
[UIHint("Tags")]
public virtual string Tags { get; set; }
```

Register tags in Startup.cs using folllowing service extension 
```csharp
services.AddGetaTags();
```

Use ITagEngine to query for data:

```csharp
IEnumerable<ContentData> GetContentByTag(string tagName);
IEnumerable<ContentData> GetContentsByTag(Tag tag);
IEnumerable<ContentData> GetContentsByTag(string tagName, ContentReference rootContentReference);
IEnumerable<ContentData> GetContentsByTag(Tag tag, ContentReference rootContentReference);
IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames);
IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags);
IEnumerable<ContentReference> GetContentReferencesByTags(string tagNames, ContentReference rootContentReference);
IEnumerable<ContentReference> GetContentReferencesByTags(IEnumerable<Tag> tags, ContentReference rootContentReference);
```

## Customize Tag-it behaviour
You can customize the [Tag-it.js](https://github.com/aehlke/tag-it) settings by using the GetaTagsAttribute.
The following settings can currently be customized

- allowSpaces - defaults to **false**
- allowDuplicates - defaults to **false**
- caseSensitive - defaults to **true**
- readOnly - defaults to **false**
- tagLimit - defaults to **-1** (none)

```csharp
[CultureSpecific]
[UIHint("Tags")]
[GetaTags(AllowSpaces = true, AllowDuplicates = true, CaseSensitive = false, ReadOnly = true)]
public virtual string Tags { get; set; }
```

## Tags management

Tags management panel is available via following url:

```csharp 
<project_url>/EPiServer/Geta.Tags/GetaTagsAdmin
```

## Local development setup

Use Geta.Tags.Sample (Episerver 12 preview version) project for testing

## Package maintainer

https://github.com/patkleef

## Changelog

[Changelog](CHANGELOG.md)

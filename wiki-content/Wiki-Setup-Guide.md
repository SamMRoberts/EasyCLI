# EasyCLI Wiki Setup Guide

This guide explains how to set up the GitHub Wiki for the EasyCLI repository using the prepared content.

## Wiki Content Overview

The wiki has been structured with the following pages:

1. **Home** - Main landing page with overview and navigation
2. **Getting Started** - Installation and first steps
3. **Core Features** - ANSI styling, prompts, and basic functionality
4. **CLI Enhancement Features** - Professional CLI patterns (v0.2.0+)
5. **Shell Framework** - Interactive shell and custom commands
6. **Output and Scripting** - Output formats and automation support
7. **API Reference** - Complete API documentation
8. **Examples and Tutorials** - Practical usage examples
9. **Contributing** - Development guidelines and contribution process

## Setting Up the Wiki

### Step 1: Enable Wiki

1. Go to the [EasyCLI repository](https://github.com/SamMRoberts/EasyCLI)
2. Click on **Settings** tab
3. Scroll down to **Features** section
4. Check the **Wikis** checkbox to enable the wiki

### Step 2: Create Wiki Pages

1. Navigate to the **Wiki** tab in the repository
2. Click **Create the first page**
3. For each page, follow this pattern:

#### Home Page
- **Page Title**: `Home`
- **Content**: Copy content from `wiki-content/Home.md`

#### Getting Started Page
- **Page Title**: `Getting-Started`
- **Content**: Copy content from `wiki-content/Getting-Started.md`

#### Core Features Page
- **Page Title**: `Core-Features`
- **Content**: Copy content from `wiki-content/Core-Features.md`

#### CLI Enhancement Features Page
- **Page Title**: `CLI-Enhancement-Features`
- **Content**: Copy content from `wiki-content/CLI-Enhancement-Features.md`

#### Shell Framework Page
- **Page Title**: `Shell-Framework`
- **Content**: Copy content from `wiki-content/Shell-Framework.md`

#### Output and Scripting Page
- **Page Title**: `Output-and-Scripting`
- **Content**: Copy content from `wiki-content/Output-and-Scripting.md`

#### API Reference Page
- **Page Title**: `API-Reference`
- **Content**: Copy content from `wiki-content/API-Reference.md`

#### Examples and Tutorials Page
- **Page Title**: `Examples-and-Tutorials`
- **Content**: Copy content from `wiki-content/Examples-and-Tutorials.md`

#### Contributing Page
- **Page Title**: `Contributing`
- **Content**: Copy content from `wiki-content/Contributing.md`

### Step 3: Configure Sidebar (Optional)

Create a sidebar for easy navigation:

1. Create a new page titled `_Sidebar`
2. Add the following content:

```markdown
## EasyCLI Wiki

**Getting Started**
- [Home](Home)
- [Getting Started](Getting-Started)

**Core Documentation**
- [Core Features](Core-Features)
- [CLI Enhancement Features](CLI-Enhancement-Features)
- [Shell Framework](Shell-Framework)
- [Output and Scripting](Output-and-Scripting)

**Reference**
- [API Reference](API-Reference)
- [Examples and Tutorials](Examples-and-Tutorials)

**Contributing**
- [Contributing](Contributing)

**External Links**
- [Repository](https://github.com/SamMRoberts/EasyCLI)
- [NuGet Package](https://www.nuget.org/packages/SamMRoberts.EasyCLI)
- [Issues](https://github.com/SamMRoberts/EasyCLI/issues)
- [Discussions](https://github.com/SamMRoberts/EasyCLI/discussions)
```

### Step 4: Verify Links

After creating all pages, verify that internal links work correctly:

- All `[Page Name](Page-Name)` links should navigate properly
- External links to repository, issues, etc. should work
- Code examples should display correctly with syntax highlighting

## Wiki Content Features

### Comprehensive Coverage

The wiki provides complete documentation covering:

- **Installation and setup** for both .NET and PowerShell users
- **Core features** with extensive code examples
- **Professional CLI patterns** for enterprise applications
- **Interactive shell framework** for building CLI applications
- **Output contracts** for automation and scripting
- **Complete API reference** with all classes and methods
- **Real-world examples** and step-by-step tutorials
- **Contribution guidelines** for developers

### User-Friendly Structure

- **Progressive disclosure** - Start simple, get more advanced
- **Rich examples** - Practical code samples throughout
- **Cross-references** - Easy navigation between related topics
- **Quick start** - Get users productive immediately
- **Best practices** - Professional guidance and patterns

### Maintenance-Friendly

- **Modular structure** - Each page focuses on specific topics
- **Consistent formatting** - Standard structure across all pages
- **Version awareness** - Clear indication of version-specific features
- **Update guidance** - Clear areas that need updates for new versions

## Alternative: GitHub Pages

If you prefer a more sophisticated documentation site, the wiki content can also be used with GitHub Pages:

1. Create a `docs/` directory in the repository
2. Copy all wiki content files to `docs/`
3. Add a `_config.yml` file for Jekyll configuration
4. Enable GitHub Pages in repository settings

This would provide:
- Custom themes and styling
- Better navigation and search
- Integration with repository workflow
- Version control for documentation changes

## Maintenance

### Keeping Wiki Updated

1. **Version releases** - Update version numbers and features
2. **API changes** - Update API reference and examples
3. **New features** - Add documentation and examples
4. **Bug fixes** - Update examples if they affect documented behavior
5. **Community feedback** - Address questions and improve clarity

### Regular Reviews

Schedule regular reviews to:
- Check for broken links
- Verify code examples still work
- Update screenshots if UI changes
- Add community-requested examples
- Improve clarity based on user feedback

## Success Metrics

Track wiki effectiveness through:
- **GitHub wiki page views** (if available)
- **Issue reduction** - Fewer documentation-related issues
- **Community engagement** - More discussions and contributions
- **User success** - Positive feedback and testimonials

---

This wiki structure provides comprehensive, user-friendly documentation that will help users get the most out of EasyCLI while encouraging community contributions and engagement.
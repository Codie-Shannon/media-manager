# Recovered UI and resource inventory

Date: 2026-07-26

Status: Group 1 baseline inventory

## Purpose and scope

This inventory records the recovered WPF presentation system before restoration or redesign. It covers the reusable controls library, its XAML resource graph, application views, publication-safe image assets, and the principal coupling that later groups must preserve or replace.

Generated output, NuGet packages, private databases, recovery backups, and unpublished copyrighted samples are outside this inventory.

## Inventory summary

| Area | Recovered total | Baseline role |
| --- | ---: | --- |
| Controls-library XAML files | 55 | Theme entry point, shared resources, and control-family dictionaries |
| Control-family style dictionaries | 52 | Default appearance and templates for public controls |
| Public concrete controls | 52 | Reusable WPF controls consumed by the application and tester |
| Control templates | 138 | Visual trees and interaction states |
| Data templates | 4 | Search results and selector-driven presentation |
| Styles | 182 | Shared and control-specific presentation rules |
| Brush resources | 184 | Dark-theme colors, borders, foregrounds, and state colors |
| Keyed XAML resources | 370 | Templates, styles, brushes, converters, and selectors |
| Converter resource instances | 16 | Presentation and state conversion |
| Application XAML files | 11 | App resources, main shell, six libraries, and three viewers |
| Application source image assets | 8 | Default covers, multi-select art, and one dormant logo |
| Controls-library image assets | 3 | Window icon and synthetic folder textures |
| Tester image assets | 4 | Publication-safe control demonstration fixtures |

Counts are source-level counts. A template or style can serve more than one interaction state or application workflow.

## Resource loading graph

`Themes/Generic.xaml` is the controls library theme entry point. It merges resources in this order:

1. `Resources/Styles.xaml`, which itself merges `Resources/ColorBrushes.xaml`;
2. the custom message-box dictionary;
3. element, folder-browser, information-pane, navigation, options-panel, miscellaneous, picture, submenu, and viewer dictionaries.

All 52 control-family dictionaries are therefore available through the library's default WPF theme metadata. Application views consume public controls directly and rely on their default styles; the application does not merge each control dictionary independently.

`Resources/Styles.xaml` also registers the shared `ElementSelector`. The recovered selectors, converters, and pack-URI paths are part of the presentation contract even when they are not visible controls.

## Controls by family

| Family | Style dictionaries | Public controls | Primary role |
| --- | ---: | --- | --- |
| Custom message box | 1 | `CustomMessageBox` | Application-owned modal messages |
| Elements | 5 | `ContextMenuItem`, `CoverFolder`, `CoverItem`, `ImageFolder`, `ImageItem` | Library tiles, folders, item cards, and item menus |
| Folder browser | 2 | `FolderBrowser`, `fbSelectionTextBox` | Folder selection window and selected-path field |
| Information pane | 6 | `ipCover`, `ipDetails`, `ipDetailsTitle`, `ipProperty`, `ipRating`, `ipTitle` | Selected-item metadata, cover, rating, and property display |
| Navigation bar | 3 | `NavigationBar`, `NavigationBarItem`, `NavigationBarItemSeparator` | In-library command bar |
| Navigation view | 4 | `NavigationView`, `NavigationViewBack`, `NavigationViewItem`, `NavigationViewItemHeader` | Primary libraries, settings access, and viewer navigation |
| Options panel | 16 | `optButton`, `optFolderBrowserDialog`, `optIconButton`, `optIconButtonGroup`, `optLinkTextBox`, `optNumericBox`, `optOpenDialog`, `optPanel`, `optRadioButton`, `optRadioButtonGroup`, `optSearchBox`, `optSearchBoxGame`, `optSearchBoxMovie`, `optTextBox`, `optTextBoxLong`, `optTitle` | Add/edit/settings forms and metadata search |
| Other | 1 | `Loading` | Busy-state presentation |
| Picture | 1 | `imgPicture` | Zoomable and scrollable picture surface |
| Submenu | 9 | `subButton`, `subCheckBox`, `subComboBox`, `subComboBoxItemSeparator`, `subGroup`, `subGroupSeparator`, `subMenu`, `subRadioButton`, `subToggleButton` | Sorting, filtering, view selection, and secondary commands |
| Viewer | 4 | `Viewer`, `viewBar`, `viewIconButton`, `viewVolumeBar` | Video, music, and picture viewing controls |

The concrete controls are supported by recovered base classes for elements, folders, icon buttons, loading, navigation, numeric input, open dialogs, radio groups, ratings, search, selection, submenu groups, and text input. These base classes contain dependency properties and behavior that must be considered alongside the XAML templates.

## Supporting presentation code

- Two routed-command containers support folder browsing and open-dialog behavior.
- Sixteen converter classes cover folder selection, vacant-state visibility, navigation settings, dialog layout, metadata search layout/messages, ratings, score/review display, loading geometry, and volume icons.
- `ElementSelector` chooses card presentation.
- `StackExtension` and the picture animation helper support layout and picture interaction.
- The controls library contains local models for folder, movie search, game search, and open-dialog items.
- Icon values are represented by the recovered `Icons` type and are rendered primarily with `Segoe MDL2 Assets`.

## Application view map

| Application surface | Recovered presentation responsibilities |
| --- | --- |
| `MainWindow` | Six-library navigation, settings panel, loading state, and view-model template selection |
| `MoviesView` | Cover cards/folders, metadata details, add/edit panels, sorting/filtering submenu, and movie search |
| `TVShowsView` | Cover cards/folders, show/season metadata, numeric input, add/edit panels, sorting/filtering submenu, and search |
| `VideosView` | Image cards/folders, details, add/edit panels, sorting/filtering submenu, and player launch |
| `PicturesView` | Image cards/folders, details, add/edit panels, sorting/filtering submenu, and gallery launch |
| `MusicView` | Image cards/folders, track/album details, add/edit panels, sorting/filtering submenu, and player launch |
| `GamesView` | Cover cards/folders, game details and rating, add/edit panels, sorting/filtering submenu, and game search |
| `VideoPlayerView` | Viewer surface, navigation, transport bar, icon controls, and volume |
| `MusicPlayerView` | Cover/title presentation, transport icons, and volume |
| `PictureGalleryView` | Scrollable picture surface, viewer bar, navigation, and picture actions |

The recovered controls tester remains a style and behavior demonstration surface. Navigation labels that do not switch configured pages are incomplete demonstrations, not evidence of an application-navigation defect.

## Image and texture resources

### Full application

The application source contains default covers for games, movies, music, pictures, and videos; episode and song multi-select images; and `Logo_Dark.png`. The seven cover/multi-select files are project content copied to the output directory. `Logo_Dark.png` is present in source but is not listed as a project resource in the recovered project file.

### Controls library

- `blankicon.ico` is the recovered neutral window icon.
- `CoverFolder_Icon.png` and `ImageFolder_Icon.png` are synthetic publication-safe replacements in this curated repository.

### Controls tester

The tester contains `cover.jpg`, `Image.jpg`, `SongMultiselect.png`, and `SyntheticBackdrop.jpg`. Recovered commercial game artwork was excluded or replaced before publication. The committed tester assets are demonstration fixtures, not representative user-library data.

## Preserved visual characteristics

- Dark charcoal and black surfaces with white text and cyan/red selection accents.
- Large fixed command targets and cover/image tile formats.
- Segoe UI typography with Segoe MDL2 icon glyphs.
- Heavy use of explicit pixel dimensions, margins, and template-local layout.
- Shared resource keys for hover, pressed, selected, disabled, and focus states.
- Separate cover-oriented and image-oriented card families.
- Modal options panels over the active library rather than separate pages.

These characteristics describe the recovered baseline; they are not a mandate for the modern design.

## Restoration and redesign constraints

- The app depends on the library's default theme and pack-URI resource loading. Group 2 must preserve that behavior when the stale DLL reference becomes a project reference.
- Dependency properties and code-behind behavior must be inventoried with templates before a control is replaced; appearance and behavior are not cleanly separated everywhere.
- Repeated generic template-key names occur in separate merged dictionaries. Any resource reorganization must be regression-tested for lookup and merge-order changes.
- Hard-coded dimensions are likely to affect resizing, high-DPI behavior, and accessibility.
- Icon-font glyphs need accessible names and a deliberate replacement strategy if the modern UI adopts SVG or another icon system.
- Search controls contain provider-specific result templates. Group 4 must separate provider-neutral state before those templates are redesigned.
- The custom message box, folder browser, and destructive commands require behavioral testing, not visual review alone.
- The original screenshots and recovered-state walkthrough are the fixed "before" evidence for Group 6 comparisons.

## Group 1 disposition

The controls, styles, templates, resources, view usage, and publication-safe assets are now sufficiently inventoried to freeze the recovered presentation baseline. No restoration or modernization changes were made as part of this inventory.

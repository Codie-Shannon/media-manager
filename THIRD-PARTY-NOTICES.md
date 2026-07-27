# Third-party notices

This file records third-party components referenced by the Media Manager solution. It is a notice index, not a replacement for the complete upstream license texts.

## Portable application runtime

| Component | Version | License / notice source |
| --- | --- | --- |
| Dapper | 2.1.11 | [Apache-2.0](https://licenses.nuget.org/Apache-2.0) |
| Entity Framework | 6.4.4 | [Apache-2.0](https://licenses.nuget.org/Apache-2.0) |
| MediaInfo.DotNetWrapper | 1.0.7 | [MIT](https://github.com/StefH/MediaInfo.DotNetWrapper/blob/master/LICENSE) |
| MediaInfo.Native / MediaInfoLib | 17.12 | [BSD-2-Clause](https://github.com/MediaArea/MediaInfoLib/blob/master/LICENSE) |
| Ookii.Dialogs.Wpf | 5.0.x | [BSD-3-Clause](https://licenses.nuget.org/BSD-3-Clause) |
| SharpZipLib | 1.4.2 | [MIT](https://licenses.nuget.org/MIT) |
| System.Data.SQLite packages | 1.0.118.0 | [Public domain](https://system.data.sqlite.org/home/doc/trunk/www/copyright.wiki) |
| VirtualizingWrapPanel | 1.5.7 | [MIT](https://licenses.nuget.org/MIT) |
| Newtonsoft.Json | 13.0.3 | [MIT](https://licenses.nuget.org/MIT) |
| Microsoft .NET support packages | versions in `packages.config` | [MIT and package-specific legacy Microsoft terms](https://www.nuget.org/profiles/Microsoft) |

The MediaInfo.Native package also supplies dynamically loaded native builds of MediaInfo and its networking dependencies (`libcurl`, `libssh2`, and OpenSSL). Their upstream notices and source are available from [MediaInfoLib](https://github.com/MediaArea/MediaInfoLib), [curl](https://curl.se/docs/copyright.html), [libssh2](https://www.libssh2.org/license.html), and [OpenSSL](https://www.openssl.org/source/license.html).

## Controls demonstration project

`src/MediaControlsTester` references these components for its media-control demonstration. They are not included in the Media Manager portable package:

| Component | Version | License / notice source |
| --- | --- | --- |
| FFME.Windows | 4.4.350 | [Package composite notice: Ms-PL and bundled component terms](https://github.com/unosquare/ffmediaelement/blob/master/LICENSE) |
| FFmpeg.AutoGen | 4.4.0 | [LGPL-3.0](https://github.com/Ruslan-B/FFmpeg.AutoGen/blob/master/LICENSE.txt) |
| FFmpeg native libraries used by FFME | package-defined | [LGPL-2.1-or-later by default; build options can change terms](https://github.com/FFmpeg/FFmpeg/blob/master/LICENSE.md) |

## Metadata services

Media Manager can query TMDB and IGDB using credentials supplied by the user. Those services, their data, and their marks retain their own terms.

- "This product uses the TMDB API but is not endorsed or certified by TMDB."
- Game metadata is identified as supplied by IGDB.

See [metadata-provider-migration.md](docs/metadata-provider-migration.md) for setup and attribution details.

## Media Manager copyright

Third-party licenses apply only to their respective components. No reuse license has been selected for Media Manager itself; see [licensing.md](docs/licensing.md).

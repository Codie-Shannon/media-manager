# Original application history

Media Manager is an origin project: the first serious application I built during my software-development diploma.

It was the application through which I learned and applied foundational software-development skills at meaningful scale:

- C# and WPF desktop development;
- application structure and state management;
- reusable custom controls and styling;
- file-system scanning and management;
- media metadata and search;
- SQLite persistence;
- debugging and error handling;
- interaction and interface design.

The original scope was already substantial. Media Manager was designed to organise movies, TV shows, videos, pictures, music, and games; navigate folder hierarchies; display structured detail panes; manage local files; launch or play supported media; persist its library locally; and use a separate `MediaControlsLibrary` project for its bespoke WPF controls.

## Recovery

Years later, the project was recovered from a damaged drive alongside its controls library, experiments, backups, build output, and prototype research. The recovered snapshot represented real project history, but it was only partially functional and was no longer a reproducible or safely publishable application: dependencies were disconnected, generated output and private data were mixed with source, browser-based metadata integrations were brittle, and important workflows required repair and verification.

That describes the condition of the recovered snapshot, not a definitive judgment about the application when it was originally developed or submitted. The damaged-drive material cannot prove that every historical file survived or establish the exact condition of every workflow at that earlier point.

The recovery workspace contains many historical projects, but this repository intentionally imports only:

- the latest file/folder-system application source;
- the recovered controls library;
- the recovered controls tester.

Historical backups, experiments, generated output, databases, and package caches remain outside Git in the recovery workspace and immutable archive.

This repository does not rewrite the project as if it were newly invented. It preserves the original application’s identity and architecture while documenting the work performed by the developer it helped shape.

## Two project lines

The recovered lineage is being completed in two forms:

- **Original-interface functional restoration:** a separate Student Projects edition based on the pre-modern Group 4 snapshot. It retains the original visual design while receiving the remaining functional corrections and dedicated verification evidence.
- **Modern release:** the corrected `v1.0.1` application in this repository, with supported metadata providers, strengthened local data handling, the modern interface, verified player surfaces, packaging, and release evidence.

The original screenshots and walkthrough demonstrate authentic recovered interface states. They do not claim that the recovered snapshot was fully functional.

## Public-repository curation

Recovered tester sample artwork and uncertain stock-like folder imagery were replaced in this repository copy with neutral synthetic assets. The original files remain in the external recovery archive. No application logic was changed by those substitutions.

The recovered application project also listed a stale SQLite database as copied content. The database file was excluded and its project entry removed so no user or historical database is source-controlled; runtime database creation remains the application's responsibility.

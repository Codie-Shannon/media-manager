# Modernisation case study

Media Manager is a modernised origin project, not a new application presented without history. The work began by preserving and documenting the partially functional recovered diploma-era snapshot, then progressively repaired its workflows before and alongside the architecture and interface work.

## Engineering approach

The modernisation followed a controlled sequence:

1. Freeze the recovered baseline and preserve an external immutable archive.
2. Curate the authoritative application and custom controls library into a clean repository.
3. Restore reproducible Debug and Release builds.
4. Verify and repair the original workflows using isolated synthetic data, with Groups 3-5 intended to establish a fully functional pre-modern acceptance gate.
5. Replace brittle browser scraping with supported metadata-provider APIs.
6. Strengthen local persistence, backup, recovery, logging, and release behavior.
7. Redesign the interface only after original and restored states were documented.
8. Package the result with repeatable tests and before-and-after evidence.
9. Reopen the release after live review exposed a player-layout gap missed by the earlier open/return playback check, then correct and evidence all three player surfaces in `v1.0.1`.

## What was preserved

- The application’s six-library local desktop identity.
- The separate reusable `MediaControlsLibrary`.
- Existing hierarchy, card, detail, file, playback, and launch concepts.
- Compatible SQLite identifiers and schema fields where migration offered no user benefit.
- Fixed original and restored screenshots as historical evidence.

## What changed

- Stale generated-library references became repository project references.
- Direct IMDb, Metacritic, Selenium, and browser-driver paths were removed from the active application.
- TMDB and IGDB now sit behind `IMetadataProvider` with neutral models, cancellation, timeouts, cache fallback, encrypted settings, and manual/offline behavior.
- Destructive and failure-prone workflows were repaired and verified.
- Backups, restore validation, recovery, health checks, path-redacted export, and rolling logs were added.
- A disposable `--demo` profile and repeatable stability suite make verification safe.
- The WPF shell, navigation, commands, cards, details, forms, settings, loading, empty, error, focus, and accessibility states were modernised as one coherent system.
- A portable x64 package can be rebuilt without embedding personal data, credentials, databases, logs, caches, or recovered media.

## What the repository demonstrates

- C# and WPF desktop engineering;
- custom reusable control development;
- local-first and privacy-first product design;
- filesystem and persistence workflows;
- supported metadata-provider integration;
- recovery and modernisation of an existing codebase;
- technical-debt assessment and incremental architecture work;
- controlled user-interface redesign;
- testing, evidence, packaging, and release discipline.

## Evidence

The `docs/screenshot-groups` directory contains three matching 13-image sequences:

- `original` — publication-safe captures of the partially functional recovered snapshot;
- `restored` — the reproducible build/startup checkpoint and selected restored interface states, not complete functional-acceptance evidence;
- `modern` — the completed modern interface, supplemented by dedicated `v1.0.1` player captures.

Manual verification records and build results are stored under `docs/manual-tests` and `docs/build-status.md`.

A separate Student Projects edition preserves the original interface while receiving the remaining functional corrections and a new complete verification record. It is distinct from both the recovered snapshot and this modern release.

## AI-assisted development

AI-assisted development tools were used during restoration, debugging, testing, and documentation. Product direction, architecture, scope, review, validation, and final implementation decisions remained under the author’s control.

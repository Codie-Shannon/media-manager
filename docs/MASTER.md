# Media Manager restoration master plan

This repository follows seven required restoration groups. All seven are complete. Group 8 remains conditional and is not required.

## Group 1 - Freeze and prove the recovered baseline

- Preserve the external immutable archive.
- Curate the authoritative source into a clean repository.
- Record solution structure, frameworks, packages, build results, and launch behavior.
- Capture original-state application and controls-tester evidence.
- Inventory controls, styles, templates, resources, and existing workflows.

## Group 2 - Restore the full application

- Replace the stale controls DLL hint with a project reference.
- Restore packages and required runtime dependencies.
- make Debug and Release builds reproducible from a clean checkout.
- Fix startup directory creation and surface startup failures.
- Capture restored-state screenshots.

Checkpoint meaning: `group-2-complete` proves reproducible restore, build, startup, and restored-state capture only. It is a historical milestone, not the recommended release and not a claim that the later functional-verification or player work was already complete.

## Group 3 - Verify and stabilize existing functionality

- Test navigation, folders, media cards, details, add, edit, remove, delete, Explorer actions, and local launching/playback.
- Repair known destructive deletion and ownership defects.
- Add synthetic integration fixtures and manual-test records.

## Group 4 - Introduce metadata-provider architecture

- Isolate legacy direct-site automation behind `IMetadataProvider`.
- Add provider-neutral models, cancellation, timeouts, caching, and offline behavior.
- Remove or disable direct IMDb scraping paths.

## Group 5 - Data reliability and release preparation

- Add schema versioning and migrations.
- Add backup, export, import, and recovery behavior appropriate to local data.
- Remove count-derived identifiers and improve transactional consistency.

Acceptance note: Groups 3-5 were intended to establish the fully functional pre-modern application. A later live review found that the recorded playback check proved open/return behavior but did not cover the constrained full-window player layout. The broader Group 5 work remains valid, while `v1.0.1` closes that escaped defect in the modern release. The separate Student Projects edition will receive the functional correction and a new complete acceptance pass without adopting the modern theme.

## Group 6 - Modern UI redesign

- Use original and restored screenshots as fixed before references.
- Modernize the shell, navigation, cards, details, forms, settings, and state presentation.
- Preserve familiar workflows and verify keyboard and resize behavior.

## Group 7 - Packaging and portfolio closure

- Produce and test a portable build and/or installer.
- Complete documentation, screenshots, release notes, demo video, and case study.
- Tag and publish only after the repository and release package pass privacy and licensing review.

Status: complete. The corrected v1.0.1 portable ZIP, release proof, public case study, matched visual evidence, privacy audit, licensing record, and repository tags close the required plan.

## Optional Group 8

Use only if legacy-data migration, damaged recovered files, or packaging complexity requires a separate closure pass. The v1.0.1 correction did not identify such a blocker.

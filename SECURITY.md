# Security policy

## Supported release

`v1.0.1` is the supported portfolio release. The recovered historical snapshot and intermediate group tags are preserved as engineering evidence; they are not production-supported distributions.

## Reporting a security or privacy problem

Please report security or privacy concerns privately to [codie.shannon.work@outlook.com](mailto:codie.shannon.work@outlook.com). Include the affected version, a concise reproduction, and the impact where possible.

Do not post personal media paths, databases, logs, backups, provider credentials, access tokens, or private screenshots in a public issue. Redact local usernames and library locations from diagnostic material.

## Credential and data boundaries

- TMDB, IGDB, and Twitch credentials are optional and must remain outside the repository.
- Provider settings are protected for the current Windows user and stored in the local application profile.
- The portable build contains no user database, media library, provider credential, cache, log, backup, or recovery copy.
- Security reports should use the disposable `--demo` profile whenever possible.

## Historical dependency scope

This repository documents the dependencies needed to reproduce the portfolio release. It does not claim that every dependency retained from the recovered application is suitable for new production or commercial use. Commercial distribution would require a fresh dependency, signing, installer, update, and threat-model review.

# Licensing review

## Media Manager source

No public reuse license has been selected for the Media Manager source. In the absence of a license, copyright remains with Codie Shannon and this repository does not grant permission to copy, modify, or redistribute the project.

This is intentional: Group 7 records the rights position without choosing an open-source license on the author's behalf. A future license can be added as a separate, explicit decision.

## Third-party software

NuGet package metadata, package-embedded notices, project references, and the portable runtime were reviewed. Active dependencies use permissive or public-domain terms, with additional notice obligations for the controls demonstration's FFME/FFmpeg components. The portable application does not include the FFME demonstration runtime.

The distributable package includes [THIRD-PARTY-NOTICES.md](../THIRD-PARTY-NOTICES.md). That inventory distinguishes application runtime dependencies from source/demo-harness dependencies and links to upstream license sources.

## Metadata services

- The settings UI and documentation state: "This product uses the TMDB API but is not endorsed or certified by TMDB."
- Game metadata is identified as supplied by IGDB.
- Provider logos are not redistributed.
- Users supply their own provider credentials, which remain outside source control.

The metadata services retain their own terms and branding rules. Their use does not license the Media Manager source.

## Project assets and evidence

Public demo covers and tester backdrops are generated synthetic assets. Screenshot groups show only synthetic application records. Recovered personal libraries, commercial posters/covers, private paths, databases, and credentials remain outside the repository.

This review is an engineering provenance record, not legal advice.

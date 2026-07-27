# Evidence

## Recovered-state walkthrough

`recovered-state-walkthrough.mp4` is the fixed Group 1 application walkthrough:

- duration: 00:01:17;
- video: H.264, 1280 x 720, 30 frames per second;
- audio: none;
- scope: application-window capture only;
- content: synthetic movie fixture, selected-item details, six library views, sorting state, and add-element selector;
- SHA-256: `ddbf08826a4b75beeea4c2350c120b3a4f5c48f0a80364f766702103e4c9dc32`.

The file was spot-checked at the beginning, middle, and end after recording.

## Modern-interface walkthrough

`modern-interface-walkthrough.mp4` is the Group 7 release walkthrough:

- duration: 00:00:42;
- video: H.264, 1782 x 964, 24 frames per second;
- audio: none;
- scope: application-window capture only;
- content: synthetic demo profile, modern shell, six library views, generated cards, empty state, and release settings;
- SHA-256: `722f4a015c1d70f6b04b1f030d792698589c38afa6c96a5340d24e5c729039c3`.

Frames at 2, 8, 14, 20, 26, 32, and 39 seconds were extracted and checked after recording. The capture shows no personal path, credential, real library, or provider response.

## Corrected player surfaces

`player-surfaces` contains three unmodified v1.0.1 application-window captures:

- `video-player.png` - responsive full-window video transport and viewer navigation;
- `music-player.png` - inline now-playing card with local playback controls;
- `picture-viewer.png` - full-window picture viewer with rotate and navigation controls.

All three use the disposable synthetic demo profile. The combined README image is `docs/screenshot-groups/contrast/04-modern-playback-surfaces.png`.

SHA-256:

- video player: `9B34EF98A1299331F0F4F250B8735B553E77A89624FA87EE4EC7F5DBC44C8D19`;
- music player: `18790A8E77383F36E31F6DFF9031F2275DC839032937B340176025D4C0B16FB4`;
- picture viewer: `9418C2769925CF4FF34A595F8FD94FD630998CBFE01338DB9D82EAB959AA6AFC`;
- combined player image: `47357726B1470824C6020ED2C8DF50AF3214B0020BCBD5B22C5CD4F945335E41`.

Build logs and test summaries remain reproducible command output rather than committed machine-specific files.

Raw private screenshots, databases, personal paths, and copyrighted recovered samples must remain outside Git.

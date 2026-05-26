# Changelog

## 1.5.0 - 2026-05-26

- Added a new namespace setting to use the script name as the default namespace
- Added real-time namespace sync from `Script name` while creating a new script
- Improved namespace autofill so manual edits stop further automatic overwrites until the form is reset
- Expanded the pending script preview to show the relative creation path for each queued script
- Added a direct repository link in `MainWindow`
- Fixed outdated stylesheet references after moving resources from `Assets` to `Packages`
- Fixed the invalid `unity-text-align` USS property warning

## 1.4.0 - 2026-05-20

- Expanded `SettingsWindow` with generation-focused configuration
- Added name normalization modes
- Added default namespace modes, including namespace generation from path
- Added route restrictions and automatic folder creation settings
- Added post-generation actions and queue cleanup behavior
- Added template defaults for extra imports and header comments
- Updated manager generation flow to respect the new settings
- Prevented repeated clicks on `Generate` while generation is in progress

## 1.3.0 - 2026-02-27

- Unity 6.3 support
- Minor fixes

## 1.2.1 - 2026-02-25

- Some code refactoring
- Visual window fixes

## 1.2.0 - 2025-10-28

- Git URL import fix
- Resource system refactor
- Assembly Definition

## 1.1.0 - 2025-10-28

- Custom configuration window for tool settings

## 1.0.1 - 2025-10-23

- Fixed Unity security issue

## 1.0.0 - 2025-05-26

- Initial beta release

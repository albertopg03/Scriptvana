<p align="left">
  <img src="https://raw.githubusercontent.com/albertopg03/Scriptvana/d9310619ceb83b7ec5a9fd40279625b79cf2a4e3/Multimedia/ScriptvanaLogo.png" alt="Scriptvana Logo" width="420">
</p>

Unity Editor tool for creating and generating multiple C# scripts from a single workflow.

[![Unity Version](https://img.shields.io/badge/Unity-6000.3-black.svg)](https://unity.com)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.5.0-green.svg)](CHANGELOG.md)

## Overview

Scriptvana is focused on one job: speeding up repetitive script creation inside Unity.

Instead of creating one file at a time, you can queue several scripts, review them before generation, edit queued items, and then generate everything in one batch using built-in templates.

The tool currently includes:

- A main manager window for building and editing a generation queue
- A settings window for controlling naming rules, default namespaces, route restrictions, template defaults, and post-generation behavior
- Built-in templates for `MonoBehaviour`, `ScriptableObject`, `Interface`, and `EmptyClass`
- Persistent configuration through `EditorPrefs`

## Installation

### Via Git URL

1. Open `Window > Package Manager`
2. Click `+`
3. Select `Add package from git URL...`
4. Paste:

```text
https://github.com/albertopg03/Scriptvana.git?path=/Assets/Scriptvana
```

### Manual installation

1. Download or clone this repository
2. Copy `Assets/Scriptvana` into your Unity project's `Assets` folder
3. Let Unity reimport the files

## Requirements

- Unity `6000.3.0f1` or compatible `6000.3`
- Works as an Editor extension

## Windows

### Manager

Open from:

`Tools > Scriptvana > Manager`

Main capabilities:

- Add scripts to a pending generation list
- Edit queued scripts by selecting them from the list
- Remove queued scripts before generation
- Preview the relative target path for each queued script
- Generate all queued scripts in batch
- Prevent repeated clicks on `Generate` while generation is running

Form fields:

- `Script Name`
- `Type`
- `Name Space`
- `Script Path`

Main behaviors:

- `Add Script` adds a new entry to the queue
- Selecting a queued script switches the form into edit mode
- `Apply Changes` updates the selected queued item
- `New Script` exits edit mode and resets the form

### Settings

Open from:

`Tools > Scriptvana > Settings`

Settings are grouped around the real generation flow, not just visual preferences.

## Current features

### Batch generation

Queue multiple scripts and generate them together from the manager window.

### Built-in templates

Available templates:

- `MonoBehaviour`
- `ScriptableObject`
- `Interface`
- `EmptyClass`

All templates support imports and optional namespaces.

### Queue editing

Before generating, you can:

- review queued entries
- modify an item
- delete an item

### Naming validation and normalization

Scriptvana validates script names and can also normalize them depending on the selected configuration.

Supported normalization modes:

- `Disabled`
- `PascalCase`
- `PascalCaseWithUnderscores`
- `UppercaseFirstLetter`

### Namespace defaults

The namespace field can be resolved in four ways:

- `Empty`
- `Fixed`
- `FromPath`
- `ScriptName`

When `FromPath` is enabled, Scriptvana derives the namespace from the target folder path.

When `ScriptName` is enabled, Scriptvana autofills the namespace from the current script name while you type. If you manually edit the namespace field, Scriptvana stops overwriting it until the form is reset.

### Route and folder rules

You can configure:

- default script path
- whether the path is manually editable in the manager
- base path restriction
- automatic folder creation

This is useful when you want all generated scripts to stay inside a controlled folder structure.

### Post-generation behavior

After generation, Scriptvana can:

- do nothing
- clear only the form
- clear the queue and the form

It can also automatically:

- do nothing
- select the generated script
- open the generated script
- ping the generated folder

### Template defaults

You can define:

- additional imports appended to generated files
- an optional header comment included in every generated file

## Settings reference

### Name normalization

| Setting | Purpose |
|---|---|
| `Min characters` | Minimum valid length for script names |
| `Normalization mode` | Defines how Scriptvana normalizes names before validation/generation |

### Default namespace

| Setting | Purpose |
|---|---|
| `Use script name as namespace` | Uses the current script name as the default namespace while creating a new script |
| `Namespace mode` | Chooses how the namespace field is resolved |
| `Fixed namespace` | Used when namespace mode is `Fixed` |

### Routes and scripting

| Setting | Purpose |
|---|---|
| `Manual editable path` | Allows the path field to be edited directly in the manager |
| `Default path` | Default target folder used when starting a new script |
| `Restrict to base path` | Forces generated scripts to stay inside a configured root folder |
| `Base path` | Root folder used by path restriction |
| `Auto create directories` | Creates missing folders automatically during generation |

### After generation

| Setting | Purpose |
|---|---|
| `Open generated asset` | Controls what Unity focuses after generation |
| `Post generation behavior` | Controls whether the form or queue is cleared |

### Template defaults

| Setting | Purpose |
|---|---|
| `Additional imports` | Extra `using` lines added to generated files |
| `Include header comment` | Enables a generated comment block at the top of each file |
| `Header comment` | The text included when header comments are enabled |

## Validation behavior

Scriptvana validates:

- script name format
- minimum script name length
- namespace format
- duplicate entries in the queue
- route validity according to current settings
- naming convention warnings according to the selected normalization mode

Validation runs before an item is accepted into the queue or updated.

## Project structure

Important folders:

- `Assets/Scriptvana/Editor/Windows`
- `Assets/Scriptvana/Editor/Services`
- `Assets/Scriptvana/Editor/Persistence`
- `Assets/Scriptvana/Editor/Validations`
- `Assets/Scriptvana/Editor/Resources/Templates`
- `Assets/Scriptvana/Editor/Resources/UI`

## Changelog

See [CHANGELOG.md](CHANGELOG.md).

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE).

## Author

Alberto Peña

- GitHub: [@albertopg03](https://github.com/albertopg03)
- Repository: [Scriptvana](https://github.com/albertopg03/Scriptvana)


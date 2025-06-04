# Scriptvana User Manual

## Overview

This manual is intended to guide users through the installation, configuration, and usage of **Scriptvana**, a custom Unity Editor extension developed to optimize common workflows during game development.

This tool is designed to **streamline repetitive tasks**, **automate common processes** within the Unity Editor, and provide an **intuitive interface** for developers and designers. It's especially useful in medium to large projects where efficiency and organization make the difference.

---

## Table of Contents

1. [Installation](#installation)
2. [Getting Started](#getting-started)
3. [User Interface Overview](#user-interface-overview)
4. [Core Features](#core-features)
5. [Integration with Existing Projects](#integration-with-existing-projects)
6. [Common Use Cases](#common-use-cases)
7. [Troubleshooting](#troubleshooting)
8. [FAQ](#faq)
9. [Changelog](#changelog)

---

## Installation

### Requirements

- Unity version: `2022.3 LTS` or higher.  
  *Note: The tool may work on earlier versions, but they have not been tested.*
- Compatible with URP, HDRP, and Built-in Render Pipeline  
- Supported OS: Windows, macOS

### Steps

1. Download the `.unitypackage` file or clone the repository from GitHub.
2. Open your Unity project.
3. Import the package via `Assets > Import Package > Custom Package...`.
4. Ensure there are no compile errors in the Console.

> ⚠️ If you already have a previous version installed, it is recommended to delete it before importing the new one.

---

## Getting Started

1. After importing, access the tool via `Window > Scriptvana`.
2. A new dockable window will appear in the Unity Editor.
3. Select a GameObject in the scene or project, if required.
4. Follow the on-screen instructions or navigate through the available tabs.

---

## User Interface Overview

The tool's interface is designed to offer a clean and streamlined experience for script creation inside the Unity Editor.

- **Input Fields**: Editable fields to define the script name, namespace (optional), and target folder within the `Assets/` directory.
- **Script Preview Panel**: Displays a live preview of the script that will be generated. This allows users to review the full code before creating the file.
- **Validation Messages**: If any input is invalid (e.g. incorrect path, invalid characters in the name), the tool provides clear, contextual error messages to guide the user.
- **Create Button**: Once all required fields are correctly filled out, clicking this button generates the script file at the specified location.

> The tool window can be opened via `Tools > Scriptvana` within the Unity Editor.

## Core Features

### ✅ Automatic Scene Organizer  
Organizes objects within the hierarchy based on their type or tag.

- Automatically groups GameObjects under parent folders.
- Improves readability and navigation in complex scenes.
- Supports custom grouping rules.

---

## Integration with Existing Projects

The tool is designed to be **non-intrusive**, and all core code is encapsulated under the `Scriptvana` namespace.

**Recommendations:**
- Avoid directly modifying core scripts.

---

## Common Use Cases

- Creation of standalone scripts
- Bulk generation of multiple scripts
- Preview and edit of script templates before creation
- Selection of a valid asset path within the `Assets` directory
- Error handling and validation of paths, names, and settings

---

## Troubleshooting

| Issue                              | Suggested Solution                                                   |
|------------------------------------|-----------------------------------------------------------------------|
| Tool does not appear in the menu   | Check the Console for compile errors or naming conflicts.            |
| Changes are not applied            | Ensure the correct objects or fields are selected and editable.      |

---

## FAQ

**Q: Can I customize the templates?**  
A: Yes, with some limitations. You can access and edit the code under the `Templates` section. Please proceed with caution. Future updates aim to provide additional built-in templates and support for custom user-created templates.

**Q: Does it work with URP or HDRP?**  
A: Yes, the tool has no graphical dependencies and is compatible with all pipelines.

---

## Changelog

| Version     | Date       | Notes                                |
|-------------|------------|---------------------------------------|
| 1.0.0-beta  | 2025-05-26 | Versión beta inicial.                 |

---

*Thank you for using this tool. Your feedback is key to continued improvement.*

# 🚀 Scriptvana

**Agile and massive script generation tool for Unity.**

Scriptvana is a powerful Unity Editor extension that streamlines the script creation workflow. Stop wasting time creating boilerplate code manually—generate multiple scripts at once with customizable templates, smart validations, and an intuitive UI.

[![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-black.svg)](https://unity.com)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Version](https://img.shields.io/badge/Version-1.2.0-green.svg)](CHANGELOG.md)

---

## ✨ Features

- **Batch Script Creation**: Queue multiple scripts and generate them all at once
- **Customizable Templates**: Built-in templates for MonoBehaviour, ScriptableObject, Interface, and Empty Class
- **Smart Validations**: Real-time validation for script names, paths, and naming conventions
- **Flexible Configuration**: Configure default paths, minimum name length, and naming conventions
- **User-Friendly UI**: Built with Unity's UI Toolkit for a modern, responsive interface
- **Path Browser**: Visual folder selection within your Assets directory
- **Edit Mode**: Modify queued scripts before generation
- **Persistent Settings**: Your preferences are saved between sessions

---

## 📦 Installation

### Via Git URL (Recommended)

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the `+` button in the top-left corner
3. Select `Add package from git URL...`
4. Paste the following URL:
   ```
   https://github.com/albertopg03/Scriptvana.git?path=/Assets/Scriptvana
   ```
5. Click `Add`

### Manual Installation

1. Download or clone this repository
2. Copy the `Scriptvana` folder into your project's `Assets` directory
3. Unity will automatically import the package

---

## 🎮 Getting Started

### Opening Scriptvana

Access the tool via Unity's top menu:
- **Main Window**: `Tools > Scriptvana > Manager`
- **Settings Window**: `Tools > Scriptvana > Settings`

### Creating Your First Script

1. Open the **Manager** window
2. Fill in the script details:
   - **Script Name**: The name of your class (e.g., `PlayerController`)
   - **Script Type**: Choose from MonoBehaviour, ScriptableObject, Interface, or Empty Class
   - **Namespace** (optional): Organize your code with namespaces
   - **Path**: Target folder within Assets (use the 📁 button to browse)
3. Click **Add Script** to queue it
4. Add more scripts if needed
5. Click **Create Scripts** to generate all queued scripts

---

## 🛠️ User Interface Overview

### Main Window

**Form Section:**
- **Script Name Field**: Enter the desired class name
- **Script Type Dropdown**: Select the template type
- **Namespace Field**: Optional namespace for the script
- **Path Field**: Target directory (click 📁 to browse)
- **Add Script Button**: Queue the script for generation

**Script List:**
- View all queued scripts before generation
- Click any script to edit its properties
- Remove scripts with the ❌ button

**Actions:**
- **Create Scripts**: Generate all queued scripts
- **Exit Editor Mode**: Return to add mode after editing

### Settings Window

Configure global preferences:
- **Minimum Characters**: Set minimum script name length (default: 3)
- **Use Naming Convention**: Enable PascalCase validation
- **Manually Editable Path**: Allow manual path editing
- **Default Path**: Set your preferred default directory

---

## 📚 Script Templates

Scriptvana includes four built-in templates:

### 1. MonoBehaviour
Standard Unity component script with Start() and Update() methods.

### 2. ScriptableObject
Data container script with CreateAssetMenu attribute.

### 3. Interface
Interface definition with basic structure.

### 4. Empty Class
Minimal C# class template.

> 💡 **Tip**: All templates support optional namespaces and include basic Unity imports.

---

## 🔧 Configuration

### Global Settings

Open `Tools > Scriptvana > Settings` to configure:

| Setting | Description | Default |
|---------|-------------|---------|
| Min Characters | Minimum script name length | 3 |
| Naming Convention | Enforce PascalCase naming | Enabled |
| Manually Editable Path | Allow typing paths directly | Disabled |
| Default Path | Starting directory for new scripts | Assets/Scripts |

Settings are automatically saved using Unity's EditorPrefs.

---

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📋 Requirements

- **Unity Version**: 2021.3 LTS or higher
- **Render Pipelines**: Compatible with Built-in, URP, and HDRP
- **Platforms**: Windows, macOS, Linux

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📝 Changelog



---

## 🙏 Acknowledgments

- Built with Unity's UI Toolkit
- Inspired by the need for faster iteration in game development
- Thanks to the Unity community for feedback and support

---

## 📬 Contact

**Alberto Peña**
- GitHub: [@albertopg03](https://github.com/albertopg03)
- Repository: [Scriptvana](https://github.com/albertopg03/Scriptvana)

---

<p align="center">
  Made with ❤️ for the Unity community
</p>

<p align="center">
  <i>If you find this tool useful, consider giving it a ⭐ on GitHub!</i>
</p>

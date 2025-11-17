# .NET Runtime Compiler Plugin
[![Auto build](https://github.com/DKorablin/Plugin.Compiler/actions/workflows/release.yml/badge.svg)](https://github.com/DKorablin/Plugin.Compiler/releases/latest)

Runtime compilation helper for patching small fragments of code in production without rebuilding the whole solution. Target frameworks: .NET Framework 4.8 and .NET 8 (Windows).

## Problem
Minor hotfixes often require recompiling and redeploying many assemblies. This plugin lets you inject updated source at runtime, compile it, and persist the result, reducing turnaround time for small fixes.

## Features
- Compile C# source snippets at runtime into separate assemblies (.NET Framework uses CodeDom; .NET 8 uses Roslyn)
- Add / manage assembly references through a UI dialog
- Maintain per‑snippet settings (language, references, metadata)
- Support partial (incremental) compilation workflows
- Persist compiled assemblies and linkage info for future runs
- Simple WinForms editors for code, references, and search/navigation
- Export code to Batch or PowerShell scripts for external use

## Use Cases
- Hotfix a method without full solution rebuild
- Prototype alternative implementations rapidly
- Extend existing types via generated partials (where supported)

## High Level Architecture
- Bll: Compilation, settings, assembly/link management
- Xml: Project / extension loading utilities
- UI: WinForms editors and dialogs for source + references
- Shared: Reusable UI and reflection helpers

## Installation
To install the Runtime Compiler Plugin, follow these steps:
1. Download the latest release from the [Releases](https://github.com/DKorablin/Plugin.Compiler/releases)
2. Extract the downloaded ZIP file to a desired location.
3. Use the provided [Flatbed.Dialog (Lite)](https://dkorablin.github.io/Flatbed-Dialog-Lite) executable or download one of the supported host applications:
	- [Flatbed.Dialog](https://dkorablin.github.io/Flatbed-Dialog)
	- [Flatbed.MDI](https://dkorablin.github.io/Flatbed-MDI)
	- [Flatbed.MDI (WPF)](https://dkorablin.github.io/Flatbed-MDI-Avalon)

## Safety / Guidelines
- Restrict usage to trusted administrators (runtime compilation can execute arbitrary code).
- Keep backups of original assemblies.
- Log changes and CRC/version info for traceability.
- Validate references to avoid mismatched binding redirects.

## Limitations
- Multi-target: net48 (CodeDom, multiple languages) and net8.0-windows (Roslyn, C# only).
- Not a full build system; focuses on isolated patches/snippets.
- Large refactors or cross‑assembly interface changes still require a normal build.

## Disclaimer
Use in production at your own risk; ensure proper review before deploying runtime‑compiled code.
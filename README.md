# BepInEx.ModManager
[![](https://img.shields.io/github/downloads/hanabi1224/BepInEx.ModManager/total.svg)](https://github.com/hanabi1224/BepInEx.ModManager/releases)
[![](https://img.shields.io/github/downloads/hanabi1224/BepInEx.ModManager/latest/total.svg)](https://github.com/hanabi1224/BepInEx.ModManager/releases/latest)
[![](https://img.shields.io/github/v/release/hanabi1224/BepInEx.ModManager)](https://github.com/hanabi1224/BepInEx.ModManager/releases/latest)
[![](https://github.com/hanabi1224/BepInEx.ModManager/actions/workflows/main.yml/badge.svg)](https://github.com/hanabi1224/BepInEx.ModManager/actions/workflows/main.yml)
[![MIT License](https://img.shields.io/github/license/hanabi1224/BepInEx.ModManager.svg)](https://github.com/hanabi1224/BepInEx.ModManager/blob/main/LICENSE)

This a general mod manager for BepInEx built with Electron.

## Supported platforms

windows

## Features

- Automatically detect and list steam unity games
- Manually add non-steam unity games (UnityPlayer.dll should present in that folder)
- Install / Uninstall BepInEx for a game with automatic detection of 32bit / 64bit
- Install / Uninstall / plugins for a game with dependencies auto-installed
- Upload local plugins to the tool and use later
- Notify when plugin has updates or missing dependencies
- Plugin repository / github release page support
- Steam game id based mod discovery
- Automatically detect and download plugin updates
- Automatical self update via electron-updater

## Known Issues

- It does not support external mod resource files, embed them into mod DLL instead using [embedded resource](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.assembly.getmanifestresourcestream?view=net-5.0#System_Reflection_Assembly_GetManifestResourceStream_System_String_).

## Publish mods

Merge the zip archive of your mod into [BepInEx.ModRepo](https://github.com/hanabi1224/BepInEx.ModRepo) main branch and it will be automatically published and picked up by the tool.

## Installation

Download and install from the [release](https://github.com/hanabi1224/BepInEx.ModManager/releases/latest) page for the first time.

The tool will update itself automatically once installed.

## Development

<!-- - Install [.Net Framework 4.6.2 SDK](https://dotnet.microsoft.com/download/dotnet-framework/thank-you/net462-developer-pack-offline-installer) -->
- Install [.Net 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0)
- Install [nodejs lts](https://nodejs.org/en/)
- Install [yarn](https://classic.yarnpkg.com/en/docs/install#windows-stable)
- Install npm package with ```yarn```
- Run with ```yarn start```
- Auto-fresh UI change with ```yarn watch```
- Package installer with ```yarn dist```

## Screenshots

![ss1](doc/screenshots/ss1.jpg)
![ss2](doc/screenshots/ss2.jpg)
![ss3](doc/screenshots/ss3.jpg)

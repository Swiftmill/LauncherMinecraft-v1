# Astra Launcher

Astra Launcher est un launcher Minecraft moderne pour Windows, inspiré de l'esthétique Apple (glassmorphism, animations douces) et prêt à être ouvert dans Visual Studio ou Visual Studio Code. Il fournit une base complète pour gérer les profils de jeu, l'authentification Microsoft, les mises à jour automatiques et des paramètres avancés orientés power-users.

> **Note sur les assets :** les visuels fournis sont stockés en `.txt` pour rester compatibles avec GitHub dans ce dépôt d'exemple. Renommez-les avec leur extension d'origine (`.png`, `.ico`) avant de générer votre build finale.

## Sommaire

- [Fonctionnalités clés](#fonctionnalités-clés)
- [Structure du projet](#structure-du-projet)
- [Prérequis](#prérequis)
- [Mise en place](#mise-en-place)
- [Lancement en mode développement](#lancement-en-mode-développement)
- [Compilation en .exe (Release)](#compilation-en-exe-release)
- [Configuration & assets](#configuration--assets)
- [Intégration Microsoft OAuth](#intégration-microsoft-oauth)
- [Mécanisme d'auto-update](#mécanisme-dauto-update)
- [Personnalisation du design](#personnalisation-du-design)
- [Dépannage & diagnostics](#dépannage--diagnostics)

## Fonctionnalités clés

- **UI ultra moderne** : fenêtre sans bordure, coins arrondis, ombres douces, fond glassmorphism avec thèmes clair/sombre, animations d'intro et transitions fluides.
- **Dashboard complet** : bouton "Jouer" central, sélection de profils (Vanilla, Forge, Snapshots…), statistiques et actualités (Minecraft + Launcher).
- **Gestion des profils** : profils stockés dans `config/versions.json`, support multi-dossiers `.minecraft` et loaders Forge/Fabric prêts à être intégrés.
- **Comptes Microsoft** : squelette pour OAuth officiel (device code flow) avec stockage chiffré des tokens via DPAPI (`ProtectedData`).
- **Paramètres avancés** : RAM min/max, arguments JVM, thème, langue, chemins personnalisés, options réseau & CDN.
- **Auto-update** : vérification d'une nouvelle version via endpoint JSON, affichage d'un popup et téléchargement sécurisé (SHA-256) d'une nouvelle build.
- **Logs & diagnostics** : journaux structurés, page Diagnostic avec accès aux logs et copie des infos système.
- **Architecture propre** : séparation UI / ViewModels / Services / Models, configuration JSON persistée.

## Structure du projet

```
LauncherMinecraft-v1/
├── AstraLauncher.sln
├── assets/
│   ├── icon.txt
│   ├── logo.txt
│   └── backgrounds/glass-wave.txt
├── config/
│   ├── news.json
│   ├── settings.sample.json
│   └── versions.json
├── src/
│   └── AstraLauncher/
│       ├── AstraLauncher.csproj
│       ├── App.xaml(.cs)
│       ├── MainWindow.xaml(.cs)
│       ├── Views/
│       ├── ViewModels/
│       ├── Models/
│       ├── Services/
│       ├── Helpers/
│       └── Resources/
└── README.md
```

## Prérequis

- **Windows 10/11** avec `.NET SDK 8.0` ou supérieur : [Téléchargement .NET](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Visual Studio 2022 (Workload .NET Desktop) **ou** Visual Studio Code + extension C#.
- Accès internet pour la compilation des dépendances NuGet (standard du SDK .NET).

## Mise en place

1. Cloner ce dépôt ou copier le dossier `LauncherMinecraft-v1` sur votre machine Windows.
2. Ouvrir `AstraLauncher.sln` dans Visual Studio **ou** ouvrir le dossier `src/AstraLauncher` dans VS Code.
3. Copier `config/settings.sample.json` vers `%APPDATA%/AstraLauncher/settings.json` pour démarrer avec une configuration prête.
4. (Optionnel) Ajuster `config/versions.json` et `config/news.json` selon vos besoins.

## Lancement en mode développement

Depuis un terminal développeur (`Developer PowerShell`) à la racine du dépôt :

```bash
cd src\AstraLauncher
dotnet restore
dotnet run
```

Visual Studio : `F5` lancera directement le projet en mode Debug.

## Compilation en .exe (Release)

```bash
cd src\AstraLauncher
dotnet publish -c Release -r win10-x64 /p:PublishSingleFile=true /p:PublishTrimmed=false
```

Le binaire final (`AstraLauncher.exe`) sera disponible dans :
`src\AstraLauncher\bin\Release\net8.0-windows\win10-x64\publish`

### Signature & mise à jour

- **Signature** : utilisez `signtool` ou votre solution de signature habituelle sur `AstraLauncher.exe`.
- **Distribution** : hébergez le .exe (ou un installeur MSIX/InnoSetup). Mettez à jour l'endpoint JSON d'update (voir ci-dessous).

## Configuration & assets

- `appsettings.json` : références vers dossiers, endpoint d'update, etc. Copié automatiquement dans le dossier de sortie.
- `config/versions.json` : profils de jeu (Vanilla/Forge/Fabric). Ajoutez vos profils (ex: modpacks) en précisant `loader` et `gameDirectory`.
- `config/news.json` : flux d'actualités (Minecraft & launcher). Alimentez cette structure avec votre backend ou un CDN.
- `assets/` : icônes, logo et fond glassmorphism. Les fichiers sont stockés en `.txt` ; renommez-les en `.png`/`.ico` avant de builder pour rétablir les visuels.

## Intégration Microsoft OAuth

Le service `AuthenticationService` implémente les hooks nécessaires au **Device Code Flow** :

1. Demande d'un *device code* auprès de Microsoft (`https://login.microsoftonline.com/consumers/oauth2/v2.0/devicecode`).
2. Polling jusqu'à obtention du token.
3. Échange vers Xbox Live, puis Minecraft Services (suivre la [documentation officielle](https://wiki.vg/Microsoft_Authentication_Scheme)).
4. Stockage du `refresh_token` chiffré via `ProtectedData` (DPAPI) dans `%APPDATA%/AstraLauncher/accounts.dat`.

> Les secrets client doivent être stockés côté serveur, jamais dans le client. Le code inclut du pseudo-code et des points d'extension (`TODO`) à remplir avec vos identifiants officiels.

## Mécanisme d'auto-update

- `UpdateService` interroge `Launcher.UpdateEndpoint` (défini dans `appsettings.json`).
- Le JSON attendu :

```json
{
  "version": "1.0.1",
  "notes": "Changelog markdown",
  "downloadUrl": "https://cdn.example.com/launcher/AstraLauncher-1.0.1.exe",
  "sha256": "HEX_SHA256"
}
```

- Si la version distante est supérieure à la version locale (`settings.json`), un popup s'affiche.
- `DownloadUpdateCommand` ouvre le lien dans le navigateur. Pour une mise à jour silencieuse, implémentez `UpdateService.DownloadUpdateAsync` afin de télécharger le nouvel exécutable, vérifier son hash et lancer un process de remplacement (self-update, bootstrapper, etc.).

## Personnalisation du design

- Couleurs & thèmes : `Resources/Themes/*.xaml` et `Resources/Styles/Controls.xaml`.
- Animations : `MainWindow.xaml` (storyboard d'intro) et `Views/*` pour les transitions.
- Police : WPF utilise `Segoe UI Variable` par défaut ; vous pouvez intégrer votre police (`Resources/Fonts/`).
- Logo & icônes : remplacez le contenu de `assets/logo.txt` et `assets/icon.txt`, puis renommez-les avec leurs extensions respectives (`.png`, `.ico`) avant le packaging final.
- Fond glass : renommez `assets/backgrounds/glass-wave.txt` en `.png` (ou remplacez-le par votre propre image) pour activer le fond avec texture. Sinon, le dégradé par défaut est utilisé.

## Dépannage & diagnostics

- Les logs se trouvent dans `%APPDATA%/AstraLauncher/logs` (configurable via paramètres).
- La page **Diagnostic** permet d'ouvrir le dossier de logs et de copier les informations système (OS, version .NET, mémoire).
- `SettingsService` recharge automatiquement la configuration au démarrage et la sauvegarde proprement à la fermeture.
- En cas d'erreur non gérée, une boîte de dialogue propre s'affiche et l'exception est loggée.

## Roadmap suggérée

- Implémenter les appels réels à l'API Mojang/Microsoft pour la gestion des versions et l'authentification.
- Ajouter un moteur de téléchargement multithread avec reprise.
- Intégrer une page Mods avec gestion Fabric/Forge.
- Ajouter un système de télémétrie opt-in (statistiques anonymes).
- Emballer le launcher avec MSIX ou un installateur personnalisé.

Bon build ! 🚀

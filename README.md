
# GuildManager

Jeu de gestion de guilde d'aventuriers, en solo ou en coopération, développé en **C# / .NET 10** avec une interface **WPF**.

Recrutez des aventuriers, envoyez-les en quête, gérez votre or et votre nourriture, et faites grandir le prestige de votre guilde.

---

## Sommaire

- [Fonctionnalités](#fonctionnalités)
- [Modes de jeu](#modes-de-jeu)
- [Architecture](#architecture)
- [Prérequis](#prérequis)
- [Installation](#installation)
- [Lancer le jeu](#lancer-le-jeu)
- [Jouer en coopération](#jouer-en-coopération)
- [Sauvegardes](#sauvegardes)
- [Règles du jeu](#règles-du-jeu)
- [Dépannage](#dépannage)
- [Structure du dépôt](#structure-du-dépôt)

---

## Fonctionnalités

- Recrutement d'aventuriers générés aléatoirement (classes : mage, guerrier, tank)
- Préparation de quêtes avec calcul du pourcentage de réussite en temps réel
- Système de tour (« Flip ») : résolution des quêtes, récompenses, blessures, morts, guérison
- Progression des aventuriers : expérience et montée de niveau
- Gestion des ressources : or, nourriture et prestige
- Mode solo avec sauvegarde locale et scénario à dialogues
- Mode coopération multijoueur avec ressources partagées en temps réel

---

## Modes de jeu

### Solo

Partie entièrement locale. La progression est sauvegardée dans `data/savesolo.json` après chaque tour et peut être reprise via **Continuer**.

### Coopération

Un joueur **héberge** la partie (le serveur API démarre dans l'application), les autres la **rejoignent** avec l'adresse IP de l'hôte.

| Élément | Partagé entre les joueurs | Propre à chaque joueur |
|---|---|---|
| Or et nourriture | ✅ | |
| Sauvegarde (`saves.json`) | ✅ | |
| Aventuriers recrutés | | ✅ |
| Candidats au recrutement | | ✅ |
| XP, niveaux, blessures, morts | | ✅ |
| Quêtes en cours | | ✅ |

Chaque gain ou dépense d'or et de nourriture est envoyé au serveur, sauvegardé, puis diffusé instantanément aux autres joueurs via SignalR.

---

## Architecture

| Projet | Rôle |
|---|---|
| `GuildManager.clients` | Interface WPF (vues, ViewModels, services client, navigation) |
| `GuildManager.Aplication` | Logique de jeu : `Game`, `Quest`, `Adventurer`, gestionnaires |
| `GuildManager.Api` | API ASP.NET Core (contrôleurs REST) et hub SignalR |
| `GuildManager.Infrastructure` | Accès aux données (Entity Framework Core, PostgreSQL) |

**Technologies**

- .NET 10, WPF
- ASP.NET Core (API REST hébergée dans le processus du client)
- SignalR (synchronisation temps réel)
- Entity Framework Core + PostgreSQL (comptes utilisateurs)
- Sauvegardes JSON (parties solo et coop)

**Principaux flux**

```
Client WPF ──HTTP──▶ API (port 5080) ──▶ saves.json / PostgreSQL
     ▲                   │
     └──── SignalR ◀─────┘   (ResourcesUpdated : or / nourriture)
```

---

## Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Windows (WPF)
- [PostgreSQL](https://www.postgresql.org/download/) accessible en local
- Un éditeur : Visual Studio 2022+ ou VS Code

---

## Installation

```powershell
git clone <url-du-depot>
cd GuildManager
```

Configurez la connexion à la base de données dans `appsettings.json` (chaîne de connexion PostgreSQL), puis restaurez les dépendances :

```powershell
dotnet restore
```

Les migrations Entity Framework sont appliquées automatiquement au démarrage d'une partie.

---

## Lancer le jeu

```powershell
dotnet build
dotnet run
```

Depuis le menu principal, choisissez **Solo** ou **Coop**.

---

## Jouer en coopération

### Héberger

1. Menu **Coop** : le serveur démarre automatiquement sur le port `5080`.
2. Communiquez l'adresse IP affichée aux autres joueurs.
3. Cliquez sur **Entrer dans la partie**, puis créez un compte ou connectez-vous.

### Rejoindre

1. Menu **Coop → Rejoindre**.
2. Saisissez l'adresse IP de l'hôte.
3. Créez un compte ou connectez-vous.

> Le port  doit être joignable : autorisez-le dans le pare-feu de l'hôte si les joueurs sont sur des machines différentes.

---

## Sauvegardes

| Fichier | Contenu | Portée |
|---|---|---|
| `data/savesolo.json` | Parties solo (progression complète) | Locale |
| `data/saves.json` | Pot commun de la guilde coop (or, nourriture, membres) | Partagée, gérée par l'hôte |

Les chemins sont relatifs au répertoire de travail de l'application.

---

## Règles du jeu

- **Tour** : le bouton « Flip » fait passer un tour et résout toutes les quêtes en cours.
- **Quêtes** : le taux de réussite dépend de la puissance de l'équipe face aux ennemis (ou du niveau moyen pour les quêtes de recherche).
- **Blessures et morts** : une défaite en combat peut blesser, tuer ou faire fuir les aventuriers. Les blessés guérissent après quelques tours ; un aventurier mort est retiré de la guilde.
- **Nourriture** : chaque aventurier consomme de la nourriture à chaque tour. Sans nourriture, certains quittent la guilde.
- **Expérience** : les quêtes réussies rapportent de l'XP, partagée entre les participants.
- **Défaite** : la guilde fait faillite si l'or tombe à zéro.

---

## Dépannage

**Erreurs de build `BG1002`, `RG1000` ou `CS2001` (fichiers `.baml` / `.g.cs` introuvables)**

Le dossier `obj` est dans un état incohérent. Fermez le jeu, puis :

```powershell
Get-ChildItem -Recurse -Include bin,obj -Directory | Remove-Item -Recurse -Force
dotnet restore
dotnet build -m:1
```

Si l'erreur revient, vérifiez que le projet n'est pas synchronisé par OneDrive et ajoutez-le aux exclusions de l'antivirus.

**Impossible de démarrer la partie (port 5080 occupé)**

Fermez l'autre instance du jeu ou l'application qui utilise ce port.

**Impossible de joindre l'hôte**

Vérifiez l'adresse IP, que l'hôte a bien lancé sa partie, et que le pare-feu autorise le port `5080`.

**L'or et la nourriture ne se mettent pas à jour chez les autres joueurs**

Chaque joueur doit se connecter via l'écran de connexion coop, qui établit la connexion SignalR. Dans la console de l'hôte, les lignes `[GuildHub] Connexion établie` puis `rejoint le groupe guild-1` doivent apparaître pour chaque client.

---

## Structure du dépôt

```
GuildManager/
├── data/                      # Sauvegardes JSON, données de quêtes
├── src/
│   ├── GuildManager.clients/  # Interface WPF
│   │   ├── View/              # Vues XAML et code-behind
│   │   ├── ViewModel/         # ViewModels
│   │   └── Services/          # API client, temps réel, navigation, sauvegarde solo
│   ├── GuildManager.Aplication/   # Logique de jeu
│   ├── GuildManager.Api/          # Contrôleurs et hub SignalR
│   └── GuildManager.Infrastructure/ # Persistance (EF Core)
├── appsettings.json
└── README.md
```

---

## Auteurs

Projet développé par l'équipe GuildManager.

## Licence

À définir.

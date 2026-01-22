# Plateforme d'Échange CS:GO - CS:GOAT

Une application web full-stack complète pour l'échange de skins CS:GO et l'ouverture de coffres. Construite avec ASP.NET Core API, Blazor frontend et backend machine learning Python.

## 📋 Aperçu du Projet

CS:GOAT est une plateforme complète pour échanger des skins CS:GO, ouvrir des coffres et gérer l'inventaire de jeu. La plateforme inclut :
- **Gestion des Skins et Coffres** : Parcourir, lister et échanger des objets CS:GO
- **Système Aléatoire Équitable** : Ouverture de coffre transparente avec équité prouvable
- **Gestion d'Inventaire** : Suivre et gérer les collections utilisateur
- **Intégration des Paiements** : Support de Stripe et PayPal
- **Connexion Steam** : Authentification fluide via Steam
- **Machine Learning** : Prédiction des prix et analyse des tendances
- **Notifications en Temps Réel** : Alertes utilisateur et mises à jour des transactions

## 🏗️ Architecture

Le projet se compose de plusieurs composants interconnectés :

### Services Backend

#### 1. **API ASP.NET Core** (`S5_01_App_CS_GOAT`)
- Serveur API RESTful construit avec .NET 8.0
- Entity Framework Core avec base de données PostgreSQL
- Authentification JWT
- Support CORS pour frontend Blazor

**Technologies Clés :**
- Framework : ASP.NET Core 8.0
- Base de données : PostgreSQL (avec Npgsql)
- ORM : Entity Framework Core
- Authentification : JWT Bearer + Steam OpenID
- Mapping : AutoMapper
- Paiements : Stripe, PayPal

**Contrôleurs Principaux :**
- `CaseController` - Gestion et opérations des coffres
- `SkinController` - Listing et détails des skins
- `UserController` - Gestion des utilisateurs
- `InventoryItemController` - Gestion d'inventaire utilisateur
- `ItemTransactionController` - Trading de skins
- `MoneyTransactionController` - Transactions de solde
- `RandomTransactionController` - Tirage aléatoire équitable/ouverture de coffre
- `SteamLoginController` - Authentification Steam
- `StripeController` / `PayPalController` - Traitement des paiements
- `NotificationController` - Notifications utilisateur
- Et 13+ contrôleurs spécialisés

#### 2. **Frontend Blazor WebAssembly** (`S5_01_Blazor_CS_GOAT`)
- Interface utilisateur interactive côté client
- Architecture basée sur des services
- Composants UI Radzen
- Visionneuse 3D pour les skins
- Design réactif

**Fonctionnalités Clés :**
- Navigation en temps réel des skins
- Gestion d'inventaire
- Historique des transactions
- Notifications utilisateur
- Gestion de compte

#### 3. **Backend ML Python** (`S5_01_Flask_CS_GOAT`)
- Microservice basé sur Flask
- Prédiction des prix et analyse des tendances
- Modèles machine learning (TensorFlow, scikit-learn)
- Prévision de séries chronologiques (Prophet)

**Technologies :**
- Framework : Flask
- ML : TensorFlow, scikit-learn
- Prévision : Prophet
- Base de données : PostgreSQL (via psycopg2)
- Serveur : Gunicorn

### Bibliothèque Partagée
- **Shared** - DTOs, Énumérations, Interfaces et Exceptions partagées utilisées dans tous les projets

### Tests
- **S5_01_App_CS_GOATTests** - Tests unitaires et d'intégration pour l'API
- **S5_01_Blazor_CS_GOATTests** - Tests UI pour frontend Blazor (MSTest)

## 🚀 Démarrage Rapide

### Prérequis
- **.NET 8.0 SDK**
- **PostgreSQL 12+**
- **Python 3.9+**
- **Visual Studio 2022** ou **Visual Studio Code**
- **Node.js** (pour les outils de build frontend)

### Configuration de la Base de Données

1. Créez une base de données PostgreSQL :
```sql
CREATE DATABASE CSGOAT;
```

2. Configurez la chaîne de connexion dans `appsettings.Development.json` :
```json
"ConnectionStrings": {
    "LocalConnectionString": "Server=localhost;port=5432;Database=CSGOAT;uid=postgres;password=postgres;"
}
```

3. Appliquez les migrations :
```bash
cd S5_01_App_CS_GOAT
dotnet ef database update
```

### Configuration de l'API

1. Naviguez vers le projet API :
```bash
cd S5_01_App_CS_GOAT
```

2. Restaurez les dépendances et exécutez :
```bash
dotnet restore
dotnet run
```

L'API sera disponible à : `https://localhost:7070` (par défaut)

### Configuration du Frontend Blazor

1. Naviguez vers le projet Blazor :
```bash
cd S5_01_Blazor_CS_GOAT
```

2. Restaurez et exécutez :
```bash
dotnet restore
dotnet run
```

Le frontend sera disponible à : `https://localhost:7030`

### Configuration du Backend ML Python

1. Naviguez vers le projet Flask :
```bash
cd S5_01_Flask_CS_GOAT
```

2. Créez un environnement virtuel :
```bash
python -m venv venv
venv\Scripts\activate  # Windows
source venv/bin/activate  # Linux/Mac
```

3. Installez les dépendances :
```bash
pip install -r requirements.txt
```

4. Exécutez le serveur :
```bash
python runserver.py
```

## 🔐 Configuration

### Variables d'Environnement (appsettings.Development.json)

**Base de données :**
```json
"ConnectionStrings": {
    "LocalConnectionString": "...",
    "RemoteConnectionString": "..."
}
```

**JWT :**
```json
"Jwt": {
    "Secret": "votre-clé-secrète",
    "Issuer": "CSGOAT_API",
    "Audience": "CSGOAT_Client"
}
```

**Paiements (Stripe) :**
```json
"Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": "whsec_..."
}
```

**Email (Mailtrap) :**
```json
"Mail": {
    "Url": "https://send.api.mailtrap.io/api/send",
    "Auth": "Bearer {token}"
}
```

## 📦 Structure du Projet

```
S5_01_CS_GOAT/
├── S5_01_App_CS_GOAT/              # API ASP.NET Core
│   ├── Controllers/                # Points d'accès API (25+ contrôleurs)
│   ├── Models/                     # Modèles BD et entités
│   ├── Services/                   # Logique métier
│   ├── Mapper/                     # Profils AutoMapper
│   ├── Migrations/                 # Migrations EF Core
│   └── Program.cs                  # Démarrage application
│
├── S5_01_Blazor_CS_GOAT/           # Frontend Blazor WebAssembly
│   ├── Pages/                      # Pages/composants Razor
│   ├── Components/                 # Composants réutilisables
│   ├── Models/                     # ViewModels
│   ├── Service/                    # Services client API
│   ├── Layout/                     # Composants de layout
│   └── wwwroot/                    # Ressources statiques
│
├── S5_01_Flask_CS_GOAT/            # Microservice ML Python
│   ├── S5_01_Flask_CS_GOAT/        # Application Flask
│   ├── requirements.txt            # Dépendances Python
│   └── runserver.py                # Point d'entrée
│
├── S5_01_App_CS_GOATTests/         # Tests API
│   ├── Fixtures/                   # Données de test
│   └── Mocks/                      # Objets mock
│
├── S5_01_Blazor_CS_GOATTests/      # Tests UI
│   └── appsettings.json            # Configuration de test
│
└── Shared/                         # Bibliothèque partagée
    ├── DTO/                        # Objets de transfert de données
    ├── Enum/                       # Énumérations
    ├── Interfaces/                 # Interfaces communes
    └── Exceptions/                 # Exceptions personnalisées
```

## 🔌 Résumé des Points d'Accès API

### Ressources Principales
| Ressource | Points d'Accès |
|-----------|------------------|
| **Coffres** | GET/POST coffres, ouvrir coffre, obtenir détails |
| **Skins** | GET skins, filtrer par usure, rareté, collection |
| **Utilisateurs** | GET/POST/PUT utilisateur, profil, statistiques |
| **Inventaire** | GET inventaire utilisateur, articles |
| **Transactions** | GET historique transactions, échanges d'articles/argent |
| **Usure** | GET modèles 3D, détails d'usure |

### Paiements et Authentification
| Ressource | Points d'Accès |
|-----------|------------------|
| **Connexion Steam** | OAuth via Steam |
| **Stripe** | Intentions de paiement, webhooks |
| **PayPal** | Traitement paiements |
| **Notifications** | Mises à jour en temps réel, préférences |

## 🧪 Tests

### Exécuter les Tests API
```bash
cd S5_01_App_CS_GOATTests
dotnet test
```

### Exécuter les Tests UI
```bash
cd S5_01_Blazor_CS_GOATTests
dotnet test
```

## 🤝 Fonctionnalités Clés

### Système d'Échange
- Parcourir les skins et coffres disponibles
- Créer et accepter des offres d'échange
- Historique des transactions et règlement
- Tarification équitable basée sur les données du marché

### Ouverture de Coffres
- Ouverture de coffre équitable avec preuve
- Sélection aléatoire en temps réel
- Suivi des gains et statistiques

### Intégration des Paiements
- Support Stripe
- Intégration PayPal
- Gestion de portefeuille
- Limites de transaction

### Gestion des Utilisateurs
- Authentification Steam
- Personnalisation de profil
- Préférences de notifications
- Système de bannissement pour les mauvais acteurs

### ML/Analyse
- Prédiction de prix basée sur les données historiques
- Analyse des tendances
- Prévision de la demande
- Insights du marché

## 📊 Stack Technologique

| Couche | Technologies |
|--------|-----------------|
| **Frontend** | Blazor WebAssembly, Composants Radzen, Visionneuse 3D |
| **API Backend** | ASP.NET Core 8.0, EF Core, PostgreSQL |
| **Service ML** | Flask, TensorFlow, scikit-learn, Prophet |
| **Authentification** | JWT, Steam OpenID |
| **Paiements** | Stripe, PayPal |
| **Infrastructure** | Azure App Services, PostgreSQL Azure |
| **Tests** | MSTest, xUnit |

## 🐛 Dépannage

### Problèmes de Connexion à la Base de Données
- Vérifiez que PostgreSQL est en cours d'exécution
- Vérifiez la chaîne de connexion dans `appsettings.Development.json`
- Assurez-vous que la base de données existe
- Exécutez les migrations avec `dotnet ef database update`

### Erreurs CORS
- Vérifiez que l'URL du frontend Blazor est dans la politique CORS de l'API
- Vérifiez les en-têtes d'origine dans les requêtes
- Assurez-vous que les informations d'identification sont correctement configurées

### Problèmes d'Authentification
- Vérifiez que le secret JWT est configuré
- Vérifiez le temps d'expiration du token
- Assurez-vous que les identifiants OAuth Steam sont valides

## 📝 Contribution

1. Créez une branche de fonctionnalité
2. Effectuez des modifications et ajoutez des tests
3. Validez avec des messages clairs
4. Poussez et créez une Pull Request

## 📄 Licence

[Ajoutez les informations de licence ici]

## 📧 Contact

Pour toute question ou support, veuillez contacter l'équipe de développement.

---

**Dernière mise à jour :** Janvier 2026
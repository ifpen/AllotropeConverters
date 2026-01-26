# GUIDE DE DÉVELOPPEMENT

## Build et Dépendances
* **Cible :** `win-x86`. Le SDK Chromeleon ne fonctionne qu'en 32-bit.
* **Résolution du SDK :** Le `.csproj` cherche `Thermo.Chromeleon.Sdk.dll` d'abord dans le GAC (Global Assembly Cache), puis dans un dossier `..\Dependencies\`. 

## Tests Unitaires
Les tests sont situés dans le projet `.Tests`. 
* **Frameworks :** `xUnit`, `Moq`, `FluentAssertions`.
* **Pattern :** Utilisez les Builders du dossier `TestHelpers` (ex: `new InjectionBuilder().Build()`) pour préparer les tests unitaires sans le vrai SDK.

## Tests d'Intégration et le Piège du 32-bit (CorFlags)

Le projet contient des tests d'intégration complets qui utilisent le vrai SDK Chromeleon, mais **ils ne peuvent pas être exécutés comme des tests normaux**.

### L'Architecture du Projet de Test d'Intégration
Le projet `IFPEN.AllotropeConverters.Chromeleon.IntegrationTests` est particulier :
1. C'est un **Exécutable** (`<OutputType>Exe</OutputType>`), pas une librairie (`Library`).
2. Il désactive l'hôte de test standard (`<UseTestHost>false</UseTestHost>`).

### Le "Pourquoi" (Le Problème Technique)
Le SDK Chromeleon vérifie les en-têtes PE (CorFlags) et exige le flag `32BitsRequired`. L'hôte de test standard (`testhost.x86.exe`) n'a que le flag `ILOnly`. Cela fait crasher le SDK. En compilant les tests comme un exécutable autonome avec `<Prefer32Bit>true</Prefer32Bit>`, l'exécutable généré possède le bon flag.

### Exécution (À réparer - Tâche Actuelle)
Actuellement, le runner xUnit peine à s'exécuter correctement dans cette configuration "Console App". Le correctif de cette exécution est la priorité absolue du développement actuel.
* **Validation des JSON :** Ce projet utilise `Newtonsoft.Json.Schema` pour valider que la sortie du convertisseur respecte bien les schémas officiels Allotrope (inclus dans le dossier `Schemas\`).
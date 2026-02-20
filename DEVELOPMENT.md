# GUIDE DE DÉVELOPPEMENT

> [!CAUTION]
> **Prérequis OBLIGATOIRE : Chromeleon SDK**
> 
> Le SDK Chromeleon (`Thermo.Chromeleon.Sdk.dll`) est **propriétaire** et **ne peut pas être commité** dans le dépôt.
> 
> **Avant de compiler**, vous DEVEZ :
> 1. Vérifier que le fichier `Dependencies\Thermo.Chromeleon.Sdk.dll` existe
> 2. Si absent, récupérer le SDK depuis l'installation Chromeleon ou auprès d'un développeur autorisé
> 3. Placer `Thermo.Chromeleon.Sdk.dll` dans le dossier `Dependencies\` à la racine du projet
> 
> Sans ce fichier, la compilation échouera avec ~57 erreurs (types SDK introuvables).

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

### Exécution
Les tests peuvent maintenant être démarrés avec la commande suivante :

```bash
cmd /c "C:\Users\viscontm\.nuget\packages\xunit.runner.console\2.9.2\tools\net481\xunit.console.x86.exe" "IFPEN.AllotropeConverters.Chromeleon.IntegrationTests\bin\Debug\win-x86\IFPEN.AllotropeConverters.Chromeleon.IntegrationTests.exe" -nologo
```

* **Validation des JSON :** Ce projet utilise `Newtonsoft.Json.Schema` pour valider que la sortie du convertisseur respecte bien les schémas officiels Allotrope (inclus dans le dossier `Schemas\`).



# ARCHITECTURE LOGICIELLE

Le projet suit les principes SOLID et l'Injection de Dépendances (DI).

## 1. Modèle de Conversion (Mappers)
La classe principale (`ChromeleonToAllotropeConverter`) agit comme un chef d'orchestre. Elle ne fait pas le mapping elle-même. Elle délègue à des "Mappers" spécialisés injectés dans son constructeur :
* `IDeviceSystemMapper`
* `IChromatographyColumnMapper`
* `ISampleMapper`
* `IDataCubeMapper`
* `IProcessedDataMapper`

**Contrainte Architecturale :** Les nouveaux champs de conversion doivent être implémentés dans la classe spécialisée du Mapper concerné, maintenant le `ChromeleonToAllotropeConverter` uniquement comme une couche d'orchestration.

## 2. Gestion des Ressources (IDisposable)
Le SDK Chromeleon utilise des ressources COM non managées. 
* Les classes qui ouvrent un scope SDK (`CmSdkScope`) doivent implémenter `IDisposable`.
* Suivre le pattern standard C# pour `Dispose(bool disposing)`.

## 3. Modèles de Constructeurs
Les convertisseurs fournissent plusieurs constructeurs pour différents cas d'utilisation :
1. **Standalone** : Gère son propre `CmSdkScope` pour une invocation directe.
2. **Injecté (DI)** : Accepte des dépendances pour les tests unitaires et l'intégration dans des systèmes plus larges.

## 4. Peak Name Mapping (Strategy Pattern)
Le `ProcessedDataMapper` utilise un pattern strategy pour la transformation des noms de pics :
* **Interface** : `IPeakNameMappingStrategy` (méthode `MapName`).
* **Comportement par défaut** : Par défaut, aucun mapping n'est effectué (passthrough via `DefaultPeakNameStrategy`).
* **Stratégies optionnelles** : `WikidataFrenchNameStrategy` (traduction via Wikidata SPARQL).
* **Décorateurs** : `MemoryCachePeakNameStrategy` (Cache en mémoire par session).
* **Composition** : `Core Strategy (Default or Wikidata) → Memory Cache`.
* **Configuration** : Fichier `peakname-config.json` dans le répertoire d'exécution. Exemple :
  ```json
  {
    "enableWikidata": true,
    "useMemoryCache": true
  }
  ```

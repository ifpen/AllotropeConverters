# ARCHITECTURE LOGICIELLE

Le projet suit les principes SOLID et l'Injection de Dépendances (DI).

## 1. Modèle de Conversion (Mappers)
La classe principale (`ChromeleonToAllotropeConverter`) agit comme un chef d'orchestre. Elle ne fait pas le mapping elle-même. Elle délègue à des "Mappers" spécialisés injectés dans son constructeur :
* `IDeviceSystemMapper`
* `IChromatographyColumnMapper`
* `ISampleMapper`
* `IDataCubeMapper`
* `IProcessedDataMapper`

**Règle pour l'agent :** Pour ajouter un nouveau champ à convertir, il faut modifier le Mapper correspondant, pas le convertisseur principal.

## 2. Gestion des Ressources (IDisposable)
Le SDK Chromeleon utilise des ressources COM non managées. 
* Les classes qui ouvrent un scope SDK (`CmSdkScope`) doivent implémenter `IDisposable`.
* Suivre le pattern standard C# pour `Dispose(bool disposing)`.

## 3. Surcharge de Constructeurs
Les convertisseurs ont généralement plusieurs constructeurs :
1. Un constructeur "Standalone" qui gère son propre `CmSdkScope` (pour une utilisation directe).
2. Un constructeur avec injection (`DI`) pour les tests unitaires et l'intégration dans des apps plus larges.
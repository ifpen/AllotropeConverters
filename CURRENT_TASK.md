# MISSION ACTUELLE

## Tâche en cours : Faire fonctionner les tests d'intégration

**Statut :** 🔴 Non fonctionnel (Bloquant)

**Contexte technique :**
Le projet `IFPEN.AllotropeConverters.Chromeleon.IntegrationTests` est configuré en `<OutputType>Exe</OutputType>` et `<UseTestHost>false</UseTestHost>` pour obtenir le flag `32BitsRequired` (nécessaire au SDK Chromeleon). 

**Le Problème :**
L'exécution des tests via xUnit échoue (problème de configuration du runner dans une Console App).

**Objectif pour l'agent :**
- [ ] Analyser pourquoi le runner xUnit ne se lance pas ou ne trouve pas les tests.
- [ ] Corriger le `csproj` ou le `Program.cs`.
- [ ] Réussir à exécuter un test qui instancie un `CmSdkScope` réel.
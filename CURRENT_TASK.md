# MISSION ACTUELLE

## Tâche en cours : Faire fonctionner les tests d'intégration

**Statut :** 🔴 Non fonctionnel (Bloquant)

**Contexte technique :**
Le projet `IFPEN.AllotropeConverters.Chromeleon.IntegrationTests` est configuré en `<OutputType>Exe</OutputType>` et `<UseTestHost>false</UseTestHost>` pour obtenir le flag `32BitsRequired` (nécessaire au SDK Chromeleon). 

**Le Problème :**
Le test ne passe pas.
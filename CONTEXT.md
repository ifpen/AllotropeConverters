# CONTEXTE METIER : Convertisseurs Allotrope

## L'Objectif
Ce projet (développé par l'IFPEN) vise à convertir des données brutes issues d'instruments de laboratoire (Chromatographie) vers un format standard mondial : **Allotrope Simple Model (ASM)**. 

## Domaine : Chromatographie et Chromeleon
* **Chromeleon** est un logiciel de Thermo Fisher qui pilote des instruments (GC, HPLC) et stocke les données brutes. Son SDK est en C# (.NET Framework 4.8.1, x86).
* Les concepts clés à manipuler sont :
  * `IInjection` : Une analyse (un échantillon).
  * `ISignal` : Un chromatogramme (la courbe de données).
  * `ISymbol` : Des métadonnées ou variables instrumentales.

## La Sortie : Allotrope (ASM)
Le résultat est un objet JSON/C# fortement typé (via le package `IFPEN.AllotropeConverters.AllotropeModels`) qui respecte le manifest officiel Allotrope, notamment le modèle *Gas Chromatography*.
Référence : [Allotrope Simple Models C#](https://ifpen.github.io/AllotropeSimpleModelsClassGeneration/csharp/README.md).
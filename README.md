# Chromeleon To Allotrope Converter

[![License CeCILL 2.1][license-badge]][cecill-2.1]
[![NuGet][nuget-badge]][nuget-package]

A .NET library to convert Thermo Chromeleon proprietary data (via SDK) to Allotrope's ASM data (Gas Chromatography).

A project from [IFP Energies Nouvelles][ifpen], a public research, innovation and
training organization in the fields of energy, transport and the environment.

## Prerequisites

- **.NET Framework 4.8**
- **Thermo Chromeleon 7.2+ Client** installed locally (The library requires `Thermo.Chromeleon.Sdk.dll`).
- Internet access to **nuget.org** to retrieve dependencies.

## Installation

1. Clone the repository.
2. Edit the `ChromeleonConverter.csproj` file to point to your local Chromeleon SDK installation:
   ```xml
   <Reference Include="Thermo.Chromeleon.Sdk">
      <HintPath>C:\Program Files (x86)\Thermo\Chromeleon\bin\Thermo.Chromeleon.Sdk.dll</HintPath>
   </Reference>
   ```
3. Restore the public NuGet packages and build the solution:
   ```powershell
   dotnet restore
   dotnet build --configuration Release
   ```

## Usage

The library provides two initialization modes depending on your running environment (Standalone application vs Chromeleon Add-in).

### 1. Standalone Application
Use this mode if you are building an external tool or console application. The converter handles the SDK Scope creation and User Logon.

```csharp
using Ifpen.AllotropeConverters.Chromeleon;
using Thermo.Chromeleon.Sdk.Common;
using Thermo.Chromeleon.Sdk.Interfaces.Data;

// The 'using' block ensures proper SDK scope disposal and logoff
using (var converter = new ChromeleonToAllotropeConverter())
{
    // Retrieve injection using standard Chromeleon SDK factories
    IItemFactory itemFactory = CmSdk.GetItemFactory();
    Uri injectionUrl = new Uri("chrom://localhost/MySequence/MyInjection");

    if (itemFactory.TryGetItem(injectionUrl, out IInjection injection))
    {
        // Convert to Allotrope Model
        var allotropeModel = converter.Convert(injection);
        
        // Use the model (e.g. serialize to JSON)
        // ...
    }
}
```

### 2. Chromeleon Add-in (Existing Scope)
Use this mode if your code is running inside a process where the Chromeleon SDK is already initialized and authenticated (e.g., a `.NET` Add-in or Script).

```csharp
using Ifpen.AllotropeConverters.Chromeleon;
using Thermo.Chromeleon.Sdk.Common;

// 'activeScope' is your existing CmSdkScope
public void ProcessInjection(CmSdkScope activeScope, IInjection myInjection)
{
    // Initialize converter with the existing scope
    // The converter will NOT dispose the scope
    var converter = new ChromeleonToAllotropeConverter(activeScope);

    var allotropeModel = converter.Convert(myInjection);
    
    // ...
}
```

## Architecture

This library follows **Clean Architecture** principles to ensure maintainability and testability:
- **Domain**: Contains agnostic Data Transfer Objects (DTOs) representing physical data (e.g., `PeakData`). No dependency on Chromeleon SDK.
- **Chromeleon**: Contains the specific implementation.
    - `Infrastructure`: Handles the complexity of the proprietary SDK (Reflection, Reporting Engine Formulas).
    - `Mappers`: Maps the infrastructure data to the Allotrope format using the Domain DTOs.

## Roadmap

- Support for Agilent OpenLab data (using the shared Domain kernel).

## License

The code is available under the [CeCILL 2.1][cecill-2.1] license, 
which is **compatible with GNU GPL**, GNU Affero GPL and EUPL.

The [ASM JSON schemas][asm] are available under [CC-BY-NC 4.0][cc-by-nc-4.0] terms.

[//]: # (@formatter:off)

[license-badge]: https://img.shields.io/badge/License-CeCILL_2.1-green
[nuget-badge]: https://img.shields.io/nuget/v/IFPEN.AllotropeConverters.Chromeleon

[cecill-2.1]: https://opensource.org/license/cecill-2-1
[ifpen]: https://www.ifpenergiesnouvelles.com/
[asm]: https://www.allotrope.org/asm
[cc-by-nc-4.0]: https://creativecommons.org/licenses/by-nc/4.0/
[nuget-package]: https://www.nuget.org/packages/IFPEN.AllotropeConverters.Chromeleon

using IFPEN.AllotropeConverters.AllotropeModels;
using Ifpen.AllotropeConverters.Chromeleon.Abstractions;
using Thermo.Chromeleon.Sdk.Interfaces.RawData;
using Thermo.Chromeleon.Sdk.Interfaces.RawData.Collections;
using Thermo.Chromeleon.Sdk.Interfaces.Data;
using System.Collections.Generic;

namespace Ifpen.AllotropeConverters.Chromeleon.Mappers
{
    /// <summary>
    /// Maps raw signal data points to the Allotrope Chromatogram Data Cube.
    /// </summary>
    public class DataCubeMapper : IDataCubeMapper
    {
        /// <inheritdoc />
        public ChromatogramDataCube Map(ISignal signal)
        {
            var timeConversionFactor = 1.0;
            var signalUnit = signal.Metadata.TimeAxis.Unit;

            if (signalUnit != null)
            {
                if (signalUnit.Equals("min", System.StringComparison.OrdinalIgnoreCase))
                {
                    timeConversionFactor = 60.0;
                }
                else if (signalUnit.Equals("hr", System.StringComparison.OrdinalIgnoreCase))
                {
                    timeConversionFactor = 3600.0;
                }
            }

            IDataPointList points = signal.DataPoints;
            var xList = new List<double>(points.Count);
            var yList = new List<double>(points.Count);

            foreach (IDataPoint p in points)
            {
                xList.Add(p.X * timeConversionFactor);
                yList.Add(p.Y);
            }

            return new ChromatogramDataCube(
                label: signal.Name,
                datacubeStructure: new ChromatogramDatacubeStructure(
                    dimensions: new List<ChromatogramDimension> {
                        new ChromatogramDimension(
                            concept: ChromatogramDimension.ConceptEnum.RetentionTime,
                            unit: ChromatogramDimension.UnitEnum.S)
                    },
                    measures: new List<ChromatogramMeasure> {
                        new ChromatogramMeasure(
                            componentDataType: ChromatogramMeasure.ComponentDataTypeEnum.DOUBLE,
                            unit: signal.Metadata?.SignalAxis?.Unit ?? "arb")
                    }
                ),
                datacubeData: new DatacubeData(
                    dimensions: new List<List<double>> { xList },
                    measures: new List<List<double>> { yList }
                )
            );
        }
    }
}

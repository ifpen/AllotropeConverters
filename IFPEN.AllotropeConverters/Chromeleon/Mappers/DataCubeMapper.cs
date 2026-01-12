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

            return new ChromatogramDataCube
            {
                Label = signal.Name,
                DatacubeStructure = new ChromatogramDatacubeStructure
                {
                    Dimensions = new List<ChromatogramDimension> {
                        new ChromatogramDimension { 
                            Concept = ChromatogramDimension.ConceptEnum.RetentionTime, 
                            Unit = ChromatogramDimension.UnitEnum.S }
                    },
                    Measures = new List<ChromatogramMeasure> {
                        new ChromatogramMeasure { 
                            ComponentDataType = ChromatogramMeasure.ComponentDataTypeEnum.DOUBLE,
                            Unit = signal.Metadata?.SignalAxis?.Unit ?? "arb" }
                    }
                },
                DatacubeData = new DatacubeData
                {
                    Dimensions = new List<List<double>> { xList },
                    Measures = new List<List<double>> { yList }
                }
            };
        }
    }
}

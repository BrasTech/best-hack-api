using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrovoAPI.NeuralNetwork.Models
{
    public class ModelInput
    {
        [ColumnName("col0"), LoadColumn(0)]
        public float Col0 { get; set; }


        [ColumnName("col1"), LoadColumn(1)]
        public string Col1 { get; set; }
    }
}

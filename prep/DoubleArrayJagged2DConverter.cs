using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

//CHATGPT LOL

public class DoubleArrayJagged2DConverter : JsonConverter<double[][,]>
{
    public override double[][,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var list = JsonSerializer.Deserialize<List<List<List<double>>>>(ref reader, options);
        return ToArray(list!);
    }

    public override void Write(Utf8JsonWriter writer, double[][,] array, JsonSerializerOptions options)
    {
        var list = ToList(array);
        JsonSerializer.Serialize(writer, list, options);
    }

    private static List<List<List<double>>> ToList(double[][,] array)
    {
        var list = new List<List<List<double>>>();
        foreach (var matrix in array)
        {
            if (matrix == null)
            {
                list.Add(null!);
                continue;
            }

            var matrixList = new List<List<double>>();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                var row = new List<double>();
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    row.Add(matrix[i, j]);
                }
                matrixList.Add(row);
            }
            list.Add(matrixList);
        }
        return list;
    }

    private static double[][,] ToArray(List<List<List<double>>> list)
    {
        var array = new double[list.Count][,];
        for (int k = 0; k < list.Count; k++)
        {
            if (list[k] == null)
            {
                array[k] = null!;
                continue;
            }

            int rows = list[k].Count;
            int cols = list[k][0].Count;
            var matrix = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    matrix[i, j] = list[k][i][j];
                }
            }
            array[k] = matrix;
        }
        return array;
    }
}
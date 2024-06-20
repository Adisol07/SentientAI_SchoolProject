namespace prep;

public class NeuralNetwork
{
    public int[] Layers;
    public Random RNG;
    public double[][,] Weights;
    public double[][] Biases;

    public NeuralNetwork(params int[] layers)
    {
        Layers = layers;
        RNG = new Random();

        Weights = new double[layers.Length][,];
        Biases = new double[layers.Length][];

        InitializeWeights();
        InitializeBiases();
    }

    private void InitializeWeights()
    {
        for (int layer = 0; layer < Layers.Length - 1; layer++)
        {
            int currentLayer = Layers[layer];
            int nextLayer = Layers[layer + 1];

            Weights[layer] = new double[currentLayer, nextLayer];
            for (int i = 0; i < currentLayer; i++)
            {
                for (int j = 0; j < nextLayer; j++)
                {
                    Weights[layer][i, j] = RNG.NextDouble() * 2 - 1;
                }
            }
        }
    }
    private void InitializeBiases()
    {
        for (int layer = 0; layer < Layers.Length; layer++)
        {
            Biases[layer] = new double[Layers[layer]];
            for (int i = 0; i < Biases[layer].Length; i++)
            {
                //Biases[layer][i] = RNG.NextDouble() * 2;
                Biases[layer][i] = RNG.NextDouble() * 2 - 1;
            }
        }
    }

    public double[] FeedForward(double[] inputs)
    {
        double[] output = inputs;

        for (int layer = 0; layer < Layers.Length - 1; layer++)
        {
            int currentLayer = Layers[layer];
            int nextLayer = Layers[layer + 1];

            double[] layerOutput = new double[nextLayer];
            for (int j = 0; j < nextLayer; j++)
            {
                double sum = 0;
                for (int i = 0; i < currentLayer; i++)
                {
                    sum += output[i] * Weights[layer][i, j];
                }
                sum += Biases[layer + 1][j];
                layerOutput[j] = Sigmoid(sum);
            }

            output = layerOutput;
        }

        return output;
    }

    public void Backpropagate(double[] inputs, double[] targets, double learningRate)
    {
        //FeedForward for all layers
        double[][] outputs = new double[Layers.Length][];
        double[] feedForwardInputs = inputs;
        for (int layer = 0; layer < Layers.Length - 1; layer++)
        {
            int currentLayer = Layers[layer];
            int nextLayer = Layers[layer + 1];

            outputs[layer] = new double[Layers[1]];
            for (int j = 0; j < nextLayer; j++)
            {
                outputs[layer][j] = 0;
                for (int i = 0; i < currentLayer; i++)
                {
                    outputs[layer][j] += feedForwardInputs[i] * Weights[layer][i, j];
                }
                outputs[layer][j] += Biases[layer + 1][j];
                outputs[layer][j] = Sigmoid(outputs[layer][j]);
            }

            feedForwardInputs = outputs[layer];
        }

        double[] outputError = new double[Layers.Last()];
        for (int i = 0; i < Layers.Last(); i++)
        {
            outputError[i] = (targets[i] - FeedForward(inputs)[i]) * SigmoidDerivative(FeedForward(inputs)[i]);
        }

        double[] error = outputError;
        double[] pastError = null!;
        for (int layer = Layers.Length - 2; layer >= 0; layer--)
        {
            int currentLayer = Layers[layer];
            int pastLayer = Layers[layer + 1];

            pastError = error;
            error = new double[currentLayer];

            //Calculate error
            for (int i = 0; i < currentLayer; i++)
            {
                for (int j = 0; j < pastLayer; j++)
                {
                    error[i] += pastError[j] * Weights[layer][i, j];
                }
                error[i] *= SigmoidDerivative(outputs[layer][i]);
            }

            //Update weights and biases
            for (int i = 0; i < currentLayer; i++)
            {
                for (int j = 0; j < pastLayer; j++)
                {
                    Weights[layer][i, j] += learningRate * error[i] * outputs[layer][i]; //
                }
            }

            for (int i = 0; i < pastLayer; i++)
            {
                Biases[layer + 1][i] += learningRate * pastError[i]; //
            }
        }
    }

    public static double Sigmoid(double x)
    {
        return 1.0 / (1.0 + Math.Exp(-x));
    }

    public static double SigmoidDerivative(double x)
    {
        double sigmoid = Sigmoid(x);
        return sigmoid * (1 - sigmoid);
    }
}
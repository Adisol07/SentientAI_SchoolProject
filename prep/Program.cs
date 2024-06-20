using System.Text.Json;

namespace prep;

class Program
{
    public static NeuralNetwork? Network;
    public static List<string> Dictionary = new List<string>();
    public static string[] TrainingData = [
        "../data/trainData1.json",
        "../data/trainData2.json",
        "../data/trainData3.json",
        "../data/trainData4.json",
        "../data/trainData5.json",
        "../data/trainData6.json",
        "../data/trainData7.json",
        "../data/trainData8.json",
        "../data/trainData9.json",
        "../data/trainData10.json",
    ];
    public static JsonSerializerOptions options = new JsonSerializerOptions();
    public static int Epochs = 3;
    public static double LearningRate = 0.0001;

    public static void Main(string[] args)
    {
        options.WriteIndented = true;
        options.NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.Converters.Add(new DoubleArrayJagged2DConverter());
        bool foundNetwork = false;
        bool foundDictionary = false;
        if (args.Length > 0)
        {
            foreach (string arg in args)
            {
                if (arg.StartsWith("-d=") || arg.StartsWith("-d:"))
                {
                    string path = arg.Remove(0, 3);
                    Dictionary = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(path))!;
                    foundDictionary = true;
                }
                if (arg.StartsWith("-n=") || arg.StartsWith("-n:"))
                {
                    string path = arg.Remove(0, 3);
                    string fileContent = File.ReadAllText(path);
                    string[] sectors = fileContent.Split("\\\\\\\\\\\\\\\\\\\\\\");

                    Network = new NeuralNetwork(JsonSerializer.Deserialize<int[]>(sectors[0])!);
                    Network.Weights = JsonSerializer.Deserialize<double[][,]>(sectors[1], options)!;
                    Network.Biases = JsonSerializer.Deserialize<double[][]>(sectors[2])!;

                    foundNetwork = true;
                }
            }
        }
        if (!foundNetwork)
        {
            WriteDictionary();
        }
        if (!foundDictionary)
        {
            Train();
        }

        while (true)
        {
            Console.Write("Sentence: ");
            string sentence = Console.ReadLine()!;
            string[] words = Purify(sentence).Split(' ');

            double[] input = new double[Dictionary.Count];
            for (int i = 0; i < Dictionary.Count; i++)
            {
                if (words.Contains(Dictionary[i]))
                {
                    input[i] = 1;
                }
                else
                {
                    input[i] = 0;
                }
            }

            double[] output = Network!.FeedForward(input);
            Console.Write("Score: ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            foreach (double o in output)
                Console.WriteLine(o * 100);
            Console.ResetColor();
        }
    }

    public static void Train()
    {
        Console.WriteLine("Training..");
        //Network = new NeuralNetwork(Dictionary.Count, Dictionary.Count * 10, Dictionary.Count * 5, Dictionary.Count * 2, Dictionary.Count, Dictionary.Count / 2, 15, 1);
        Network = new NeuralNetwork(Dictionary.Count, 
                                Dictionary.Count, Dictionary.Count, Dictionary.Count, 
                                Dictionary.Count / 2, Dictionary.Count / 2, Dictionary.Count / 2,
                                Dictionary.Count / 5, Dictionary.Count / 5,
                                    1);

        foreach (string trainData in TrainingData)
        {
            Console.WriteLine("Training: " + trainData);
            Data[] datas = JsonSerializer.Deserialize<Data[]>(File.ReadAllText(trainData))!;
            Console.Write("Epoch (" + datas.Length + "x" + Epochs + "): ");
            int x = Console.GetCursorPosition().Left;
            int y = Console.GetCursorPosition().Top;
            Console.Write("0 / " + datas.Length * Epochs);
            int finishedEpochs = 0;
            foreach (Data data in datas)
            {
                double[] input = new double[Dictionary.Count];

                string[] words = Purify(data.comment!).Split(' ');
                for (int i = 0; i < Dictionary.Count; i++)
                {
                    if (words.Contains(Dictionary[i]))
                    {
                        input[i] = 1;
                    }
                    else
                    {
                        input[i] = 0;
                    }
                }

                for (int e = 0; e < Epochs; e++)
                {
                    Network.Backpropagate(input, [data.score / 100], LearningRate);
                }
                finishedEpochs += Epochs;
                Console.SetCursorPosition(x, y);
                Console.Write(finishedEpochs + " / " + datas.Length * Epochs);
            }
            Console.Write('\n');
        }
        SaveNetwork();
    }

    public static void WriteDictionary()
    {
        Console.WriteLine("Writing to dictionary..");
        foreach (string trainData in TrainingData)
        {
            Data[] datas = JsonSerializer.Deserialize<Data[]>(File.ReadAllText(trainData))!;

            foreach (Data data in datas)
            {
                string[] words = Purify(data.comment!).Split(' ');
                foreach (string word in words)
                {
                    if (!Dictionary.Contains(word))
                    {
                        Dictionary.Add(word);
                    }
                }
            }
        }
        SaveDictionary();
    }

    public static void SaveNetwork()
    {
        string fileContent = "";
        fileContent += JsonSerializer.Serialize(Network!.Layers);
        fileContent += "\\\\\\\\\\\\\\\\\\\\\\";
        fileContent += JsonSerializer.Serialize(JsonSerializer.Serialize(Network!.Weights, options));
        fileContent += "\\\\\\\\\\\\\\\\\\\\\\";
        fileContent += JsonSerializer.Serialize(Network!.Biases, options);
        File.WriteAllText("./network_" + DateTime.Now.Ticks + ".txt", fileContent);
    }
    public static void SaveDictionary()
    {
        File.WriteAllText("./dictionary_" + DateTime.Now.Ticks + ".json", JsonSerializer.Serialize(Dictionary, options));
    }

    public static string Purify(string str)
    {
        string r = str;
        r = r.ToLower();
        r = r.Trim();
        r = r.Replace(".", "");
        r = r.Replace(",", "");
        r = r.Replace("!", "");
        r = r.Replace("?", "");
        r = r.Replace("_", "");
        r = r.Replace("-", "");
        r = r.Replace("(", "");
        r = r.Replace(")", "");
        r = r.Replace("/", "");
        r = r.Replace("\"", "");
        r = r.Replace("'", "");
        r = r.Replace("’", "");
        return r;
    }
}
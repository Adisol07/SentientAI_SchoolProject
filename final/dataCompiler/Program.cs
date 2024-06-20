using System;
using System.Text.Json;

namespace dataCompiler;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Preparing..");
        string[] files = Directory.GetFiles("../../data", "*.json");
        foreach (string file in files)
        {
            Console.WriteLine("Compiling " + file + "..");
            Data[] datas = JsonSerializer.Deserialize<Data[]>(File.ReadAllText(file))!;

            string content = "";
            foreach (Data data in datas)
            {
                string comment = Purify(data.comment!);
                int score = data.score;

                Console.WriteLine("  > \"" + comment + "\" -> " + score);

                content += comment + ";" + score + "\n";
            }
            File.WriteAllText("../../data/compiled/" + Path.GetFileNameWithoutExtension(file) + ".txt", content.Remove(content.Length-1, 1));
            Console.WriteLine("Compiled " + file + "..");
        }
    }

    public static string Purify(string str)
    {
        string r = str;

        r = r.ToLower();
        r = r.Replace(".", "");
        r = r.Replace(",", "");
        r = r.Replace("?", "");
        r = r.Replace("!", "");
        r = r.Replace("\"", "");
        r = r.Replace("'", "");

        string cp = r;
        r = "";
        foreach (char c in cp)
        {
            if (char.IsAsciiLetterOrDigit(c) || c == ' ') r += c;
        }

        return r;
    }
}
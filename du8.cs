using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace abeceda
{
    class Program
    {
        static string[] NactiPoleStringu()
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "python";
            start.Arguments = "table_generator_upgrade.py";
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            string[] poleStringu;
            using (Process process = Process.Start(start)!)
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    // Přečte úplně všechno, co Python vypsal
                    string celyVystup = reader.ReadToEnd(); 
                    
                    // Rozseká to na jednotlivé položky podle konců řádků
                    poleStringu = celyVystup.Split(
                        new[] { '\r', '\n' }, 
                        StringSplitOptions.RemoveEmptyEntries
                    );
                }
            }
            return poleStringu;
        }

        static char[,] PrevedStringNaTabulku(string stringTabulka, int vyska, int sirka)
        {
            char[,] tabulka = new char [vyska, sirka];
            int k = 0;
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    tabulka[i,j] = stringTabulka[k];
                    k++;
                }
            }
            return tabulka;
        }

        static char[][,] VytvorPoleTabulek(int vyska, int sirka, int pocetTabulek) 
        {
            string[] poleStringu = NactiPoleStringu();
            char[][,] poleTabulek = new char[pocetTabulek][,];

            int m = 0;
            foreach (string stringTabulka in poleStringu)
            {
                var tabulka = PrevedStringNaTabulku(stringTabulka, vyska, sirka);
                poleTabulek[m] = tabulka;
                m++;
                if (m >= pocetTabulek) break;
            }
            return poleTabulek;
        }

        static string PrevedTabulkuNaString(char[,] tabulka, int vyska, int sirka)
        {
            var sb = new StringBuilder(vyska * sirka);
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    sb.Append(tabulka[i, j]);
                }
            }
            return sb.ToString();
        }

        static int Vyres(char[,] tabulka, int vyska, int sirka, string text)
        {
            var poziceZnaku = new Dictionary<char, List<(int r, int s)>>();
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    char znak = tabulka[i, j];
                    if (!poziceZnaku.ContainsKey(znak))
                        poziceZnaku[znak] = new List<(int r, int s)>();
                    poziceZnaku[znak].Add((i, j));
                }
            }

            var stavy = new Dictionary<(int r, int s), int>();
            stavy[(0, 0)] = 0;

            foreach (char cil in text)
            {
                if (!poziceZnaku.ContainsKey(cil)) continue;
                var noveStavy = new Dictionary<(int r, int s), int>();
                var cilovePozice = poziceZnaku[cil];

                foreach (var cilPozice in cilovePozice)
                {
                    int minVzdalenost = int.MaxValue;
                    foreach (var start in stavy)
                    {
                        int dist = Math.Abs(start.Key.r - cilPozice.r) + Math.Abs(start.Key.s - cilPozice.s) + 1;
                        minVzdalenost = Math.Min(minVzdalenost, start.Value + dist);
                    }
                    noveStavy[cilPozice] = minVzdalenost;
                }
                stavy = noveStavy;
            }
            int vysledek = stavy.Values.Min();
            return vysledek;
        }

        static void Main()
        {
            int vyska = 8; int sirka = 8; int pocetTabulek = 1000;
            var poleTabulek = VytvorPoleTabulek(vyska, sirka, pocetTabulek);
            string text = File.ReadAllText("du8-text.txt").Trim();
            int vysledek = int.MaxValue;
            char [,] nejlepsiTabulka = new char [vyska, sirka];

            foreach (char[,] tabulka in poleTabulek)
            {
                int novyVysledek = Vyres(tabulka, vyska, sirka, text);
                if (novyVysledek < vysledek)
                {
                    vysledek = novyVysledek;
                    nejlepsiTabulka = tabulka;
                }
            }
            string nejlepsiTabulkaString = PrevedTabulkuNaString(nejlepsiTabulka, vyska, sirka);

            Console.WriteLine(" "); 
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    Console.Write($"{nejlepsiTabulka[i, j], 2}");
                }
                Console.WriteLine();
            }
            Console.WriteLine(" "); 
            Console.WriteLine($"Počet stisků: {vysledek}"); 
            Console.WriteLine(" "); 
            Console.WriteLine($"Tabulka jako string: \"{nejlepsiTabulkaString}\""); 
            Console.WriteLine(" "); 
        }
    }
}
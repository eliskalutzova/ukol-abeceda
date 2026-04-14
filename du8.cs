using System;
using System.Diagnostics;
using System.IO;

namespace abeceda
{
    class Program
    {
        // static (char[,], string, int, int) NactiVstup()
        // {
        //     int sirka = int.Parse(Console.ReadLine());
        //     int vyska = int.Parse(Console.ReadLine());
        //     string obsahTabulky = Console.ReadLine();
        //     string text = Console.ReadLine().Trim();
        //     char[,] tabulka = new char [vyska, sirka];
        //     int k = 0;
        //     for (int i = 0; i < vyska; i++)
        //     {
        //          for (int j = 0; j < sirka; j++)
        //         {
        //             tabulka[i,j] = obsahTabulky[k];
        //             k++;
        //         }
        //     }
        //     return (tabulka, text, sirka, vyska);
        // }

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

        static char[][,] VytvorPoleTabulek(int vyska, int sirka, int pocetTabulek) 
        {
            string[] poleStringu = NactiPoleStringu();
            char[][,] poleTabulek = new char[pocetTabulek][,];

            int m = 0;
            foreach (string stringTabulka in poleStringu)
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
                poleTabulek[m] = tabulka;
                m++;
                if (m >= pocetTabulek) break;
            }
            return poleTabulek;
        }



        static void Vyresetuj2DPole(int[,] pole, int vyska, int sirka, int hodnota)
        {
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    pole[i, j] = hodnota;
                }
            }
        }

        static List<(int r, int s, int pocetK)> ZiskejInformaceAktualnichZnaku(Queue<(int r, int s, int pocetK)> fronta, char[,] tabulka, int sirka, int vyska, char cil, int[,] vzdalenosti)
        {
            (int r, int s)[] smery = [(-1, 0), (1, 0), (0, 1), (0, -1)];
            while (fronta.Count > 0)
            {
                var (aktualniR, aktualniS, pocetK) = fronta.Dequeue();
                foreach (var (smerR, smerS) in smery)
                {
                    int nR = aktualniR + smerR;
                    int nS = aktualniS + smerS;

                    if (nR >= 0 && nR < vyska && nS >= 0 && nS < sirka && pocetK + 1 < vzdalenosti[nR, nS])
                    {
                        fronta.Enqueue((nR, nS, pocetK + 1));
                        vzdalenosti[nR, nS] = pocetK + 1;
                    }
                }
            } 

            List<(int r, int s, int pocetK)> informaceAktualnichZnaku = [];
            for (int i = 0; i < vyska; i++)
            {
                for (int j = 0; j < sirka; j++)
                {
                    if (tabulka[i, j] == cil && vzdalenosti[i, j] != int.MaxValue)
                    {
                        informaceAktualnichZnaku.Add((i, j, vzdalenosti[i, j] + 1));
                    }
                }
            }
            return informaceAktualnichZnaku;
        }

        static int Vyres(char[,] tabulka, int sirka, int vyska, string text)
        {
            int[,] vzdalenosti = new int[vyska, sirka];
            Vyresetuj2DPole(vzdalenosti, vyska, sirka, int.MaxValue);
            vzdalenosti[0, 0] = 0;

            int idx = 0;
            char cil = text[idx];
            int posledni_idx = text.Length - 1;

            Queue<(int r, int s, int pocetK)> fronta = new();
            fronta.Enqueue((0, 0, 0));
            List<(int r, int s, int pocetK)> informaceAktualnichZnaku = [(0, 0, 0)];
            int vysledek = int.MaxValue;

            while (fronta.Count > 0)
            {
                List<(int r, int s, int pocetK)> noveInformaceAktualnichZnaku = ZiskejInformaceAktualnichZnaku(fronta, tabulka, sirka, vyska, cil, vzdalenosti);
                if (noveInformaceAktualnichZnaku.Count != 0)
                {
                    informaceAktualnichZnaku = noveInformaceAktualnichZnaku;
                }

                if (idx == posledni_idx)
                {
                    foreach (var znak in informaceAktualnichZnaku)
                    {
                        if (znak.pocetK < vysledek)
                        {
                            vysledek = znak.pocetK;
                        }
                    }
                    return vysledek;
                }
                idx++;
                cil = text[idx];
                Vyresetuj2DPole(vzdalenosti, vyska, sirka, int.MaxValue);
                //pridej vsechny aktualni startovni znaky do fronty a do vzdalenosti
                foreach (var (r, s, pocetK) in informaceAktualnichZnaku)
                {
                    fronta.Enqueue((r, s, pocetK));
                    vzdalenosti[r, s] = pocetK;
                }
            }
            return vysledek;
        }

        static void Main()
        {
            int vyska = 8; int sirka = 8; int pocetTabulek = 1000;
            var poleTabulek = VytvorPoleTabulek(vyska, sirka, pocetTabulek);
            string text = File.ReadAllText("du8-text.txt").Trim();
            int vysledek = int.MaxValue;

            foreach (char[,] tabulka in poleTabulek)
            {
                int novyVysledek = Vyres(tabulka, vyska, sirka, text);
                if (novyVysledek < vysledek)
                    vysledek = novyVysledek;
            }

            Console.WriteLine(vysledek); 
        }
    }
}
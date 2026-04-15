// Zjistí unikátní znaky v textu a jejich četnost


string text = File.ReadAllText("du8-text.txt").Trim();

var unikatniZnaky = new Dictionary<char, int>();
foreach (char znak in text)
{
    if (!unikatniZnaky.ContainsKey(znak))
        unikatniZnaky[znak] = 0;
    unikatniZnaky[znak] += 1;
}

foreach (var (klic, hodnota) in unikatniZnaky)
{
    Console.WriteLine($"Znak: {klic}, Počet: {hodnota}");
}

Console.WriteLine($"Celkový počet unikátních znaků: {unikatniZnaky.Count}");
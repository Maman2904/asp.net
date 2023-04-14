using System;

namespace vending_machine
{
    class Program
    {
        static void Main(string[] args)
        {
            String kalimat;
            Console.Write("Input Kalimat : ");
            kalimat = Console.ReadLine();
            Console.WriteLine();
            Console.Write("Huruf Vocal    = ");
            for (int i = 0; i < kalimat.Length; i++)
            {
                if (kalimat[i] == 'A' || kalimat[i] == 'a' || kalimat[i] == 'E' || kalimat[i] == 'e' || kalimat[i] == 'I' || kalimat[i] == 'i' || kalimat[i] == 'U' || kalimat[i] == 'u' || kalimat[i] == 'O' || kalimat[i] == 'o')
                {

                    Console.Write(kalimat[i]);

                }
            }
            Console.WriteLine();
            Console.Write("Huruf Konsonan = ");
            for (int i = 0; i < kalimat.Length; i++)
            {
                if (kalimat[i] == 'A' || kalimat[i] == 'a' || kalimat[i] == 'E' || kalimat[i] == 'e' || kalimat[i] == 'I' || kalimat[i] == 'i' || kalimat[i] == 'U' || kalimat[i] == 'u' || kalimat[i] == 'O' || kalimat[i] == 'o')
                {
                    continue;
                }
                else
                {
                    if (char.IsWhiteSpace(kalimat[i]))
                    {
                        continue;
                    }
                    else
                    {
                        Console.Write(kalimat[i] + " ");
                    }

                }
            }
            Console.ReadKey();
        }
    }
}
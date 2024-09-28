namespace ConsoleApp3;

public static class ReaderBinary
{
    public static void Read(string filePath, int numbCount)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"The contents of the file {filePath}:");
        Console.ResetColor();
        Console.WriteLine();
        int count = 0;

        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
        while (reader.BaseStream.Position < reader.BaseStream.Length && count < numbCount)
        {
            int number = reader.ReadInt32();
            Console.Write($"{number,13} ");
            
            if ((count + 1) % 8 == 0)
            {
                Console.WriteLine();
            }
            count++;
        }
        Console.WriteLine();
        Console.WriteLine();
    }
}
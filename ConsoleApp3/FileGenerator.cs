namespace ConsoleApp3;

public static class FileGenerator
{
    public static void Generate(int size, string pathFile, int sizeInMb)
    {
        Console.WriteLine($"The generation of the {sizeInMb} MB file has begun.");
        Random rand = new Random();

        using BinaryWriter writer = new BinaryWriter(File.Open(pathFile, FileMode.Create));

        for (int i = 0; i < size; i++)
        {
            int number = rand.Next(int.MinValue, int.MaxValue);
            writer.Write(number);
        }

        Console.WriteLine($"File with {size} unsorted integers generated and written in {pathFile}");
    }
}
using System.Diagnostics;

namespace ConsoleApp3;

static class Program
{
    static void Main()
    {

        string unsortedFile = "source_file.dat";
        string seriesFile = "series_nums.txt";
        
        int bufferSizeUnlimited = 1024 * 450;
        int bufferSizeLimited = 1024 * 1024 * 65;
        int bufferSize = bufferSizeUnlimited;
        
        int selectedOption = 0;
        string[] options =
        [
            "Sort with buffer for unlimited memory",
            "Sort with buffer for memory limited to 512 MB"
        ];

        int sizeInMb;
        int[] fileSizesInMb = [10, 100, 512, 1024];
        int selectedFileSizeOption = 3;
        
        Console.Clear();
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Choose file size:");

            for (int i = 0; i < fileSizesInMb.Length; i++)
            {
                if (i == selectedFileSizeOption)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine($"> {fileSizesInMb[i]} MB");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {fileSizesInMb[i]} MB");
                }
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedFileSizeOption = (selectedFileSizeOption == 0) ? fileSizesInMb.Length - 1 : selectedFileSizeOption - 1;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedFileSizeOption = (selectedFileSizeOption == fileSizesInMb.Length - 1) ? 0 : selectedFileSizeOption + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                sizeInMb = fileSizesInMb[selectedFileSizeOption];
                break; 
            }
            
            Console.SetCursorPosition(0, 1);
        }
        
        Console.Clear();
        
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Choose sorting type:");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedOption)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }
            
            ConsoleKeyInfo keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedOption = (selectedOption == 0) ? options.Length - 1 : selectedOption - 1;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedOption = (selectedOption == options.Length - 1) ? 0 : selectedOption + 1;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                if (selectedOption == 0)
                {
                    bufferSize = bufferSizeUnlimited;
                    Console.Clear();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Buffer for unlimited memory selected.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else if (selectedOption == 1)
                {
                    bufferSize = bufferSizeLimited;
                    Console.Clear();
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Buffer for memory limited to 512 MB selected.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                
                break; 
            }
            
            Console.SetCursorPosition(0, 1);
        }
        
        int integersNumber = 1024 * 1024 * sizeInMb / sizeof(int);
        int numbersToRead = 250;
        
        FileGenerator.Generate(integersNumber, unsortedFile, sizeInMb);
        ReaderBinary.Read(unsortedFile, numbersToRead);
        
        SortSeriesMod.ModSeriesSplit(unsortedFile, sizeInMb / 8, bufferSize);
        SortSeriesMod.WriteSeriesNumToFile(seriesFile);

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Sorting have started!");
        Console.ResetColor();
        Console.WriteLine();
        Stopwatch time = new Stopwatch();
        time.Start();
        NaturalMergeSort.ExternalSort(unsortedFile, seriesFile, integersNumber, bufferSize);
        time.Stop();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Sorting took {time.ElapsedMilliseconds} ms.");
        Console.ResetColor();
        
        ReaderBinary.Read(unsortedFile, numbersToRead);
        
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
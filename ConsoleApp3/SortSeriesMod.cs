using System.IO.MemoryMappedFiles;

namespace ConsoleApp3;

public static class SortSeriesMod
{
    private static readonly List<int> SeriesNum = new();

    public static void WriteSeriesNumToFile(string filePath)
    {
        using BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
        foreach (int series in SeriesNum)
        {
            writer.Write(series);
        }
    }

    public static void ModSeriesSplit(string fileToSplit, int seriesSize, int sizeOfBuffer)
    {
        int elsInSeries = 1024 * 1024 * seriesSize / sizeof(int);

        using var mmf = MemoryMappedFile.CreateFromFile(fileToSplit, FileMode.Open, null, 0,
            MemoryMappedFileAccess.Read);
        using var viewStream = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);
        using var readerA = new BinaryReader(new BufferedStream(viewStream, sizeOfBuffer));

        using var writerB = new BinaryWriter(new BufferedStream(new FileStream("file_B.dat",
            FileMode.Create), sizeOfBuffer));
        using var writerC = new BinaryWriter(new BufferedStream(new FileStream("file_C.dat",
            FileMode.Create), sizeOfBuffer));

        bool fileTurn = true;
        List<int> numbers = new List<int>();
        int counter = 0;

        int? currentNumber = readerA.BaseStream.Position < readerA.BaseStream.Length
            ? readerA.ReadInt32()
            : null;

        while (currentNumber != null)
        {
            bool tempTurn = fileTurn;
            if (numbers.Count < elsInSeries)
            {
                counter++;
                numbers.Add(currentNumber.Value);
            }
            else
            {
                SeriesNum.Add(counter + 1);
                counter = 0;
                numbers.Sort();
                foreach (int number in numbers)
                {
                    if (fileTurn)
                    {
                        writerB.Write(number);
                    }
                    else
                    {
                        writerC.Write(number);
                    }
                }
                numbers.Clear();
                tempTurn = !tempTurn;
            }

            currentNumber = readerA.BaseStream.Position < readerA.BaseStream.Length
                ? readerA.ReadInt32()
                : null;
            fileTurn = tempTurn;
        }

        if (numbers.Count > 0)
        {
            SeriesNum.Add(counter + 1);
            numbers.Sort();
            foreach (int number in numbers)
            {
                if (fileTurn)
                {
                    writerB.Write(number);
                }
                else
                {
                    writerC.Write(number);
                }
            }
        }
    }
}
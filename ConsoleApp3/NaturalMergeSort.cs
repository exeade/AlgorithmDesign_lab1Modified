using System.IO.MemoryMappedFiles;

namespace ConsoleApp3;

public static class NaturalMergeSort
{
    private static readonly List<int> SeriesNum = new();

    private static void ReadSeriesNumFromFile(string filePath)
    {
        using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open));
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            SeriesNum.Add(reader.ReadInt32());
        }
    }

    public static void ExternalSort(string unsortedFile, string startSeries, int maxEls, int bufferSize)
    {
        ReadSeriesNumFromFile(startSeries);
        do
        {
            MergeSeries(unsortedFile, maxEls, bufferSize);
            SeriesNum.Clear();
            SeriesSplit(unsortedFile, bufferSize);
        } while (SeriesNum.Count > 1);
    }

    private static void SeriesSplit(string fileToSplit, int sizeOfBuffer)
    {
        using var mmf = MemoryMappedFile.CreateFromFile(fileToSplit, FileMode.Open, null, 0,
            MemoryMappedFileAccess.Read);
        using var viewStream = mmf.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);
        using var readerA = new BinaryReader(new BufferedStream(viewStream, sizeOfBuffer));

        using var writerB = new BinaryWriter(new BufferedStream(new FileStream("file_B.dat", FileMode.Create),
            sizeOfBuffer));
        using var writerC = new BinaryWriter(new BufferedStream(new FileStream("file_C.dat", FileMode.Create),
            sizeOfBuffer));

        int counter = 0;
        bool fileTurn = true;

        int? prevNumber = readerA.BaseStream.Position < readerA.BaseStream.Length
            ? readerA.ReadInt32()
            : null;
        int? currNumber = readerA.BaseStream.Position < readerA.BaseStream.Length
            ? readerA.ReadInt32()
            : null;

        while (prevNumber != null)
        {
            bool tempTurn = fileTurn;
            if (currNumber != null)
            {
                if (prevNumber <= currNumber)
                {
                    counter++;
                }
                else
                {
                    SeriesNum.Add(counter + 1);
                    counter = 0;
                    tempTurn = !tempTurn;
                }
            }

            if (fileTurn)
            {
                writerB.Write(prevNumber.Value);
            }
            else
            {
                writerC.Write(prevNumber.Value);
            }

            prevNumber = currNumber;
            currNumber = readerA.BaseStream.Position < readerA.BaseStream.Length
                ? readerA.ReadInt32()
                : null;
            fileTurn = tempTurn;
        }

        SeriesNum.Add(counter + 1);
    }

    private static void MergeSeries(string mergeOutputFile, int maxEls, int sizeOfBuffer)
    {
        using var mmfB = MemoryMappedFile.CreateFromFile("file_B.dat", FileMode.Open, null, 0,
            MemoryMappedFileAccess.Read);
        using var readerB = new BinaryReader(new BufferedStream(mmfB.CreateViewStream(0, 0,
            MemoryMappedFileAccess.Read), sizeOfBuffer));

        using var mmfC = MemoryMappedFile.CreateFromFile("file_C.dat", FileMode.Open, null, 0,
            MemoryMappedFileAccess.Read);
        using var readerC = new BinaryReader(new BufferedStream(mmfC.CreateViewStream(0, 0,
            MemoryMappedFileAccess.Read), sizeOfBuffer));

        using var writerA = new BinaryWriter(new BufferedStream(new FileStream(mergeOutputFile, FileMode.Create,
            FileAccess.Write), sizeOfBuffer));

        int counterB = 0;
        int counterC = 0;

        int indexB = 0;
        int indexC = 1;

        int elCounter = 0;

        int? numberB = readerB.BaseStream.Position < readerB.BaseStream.Length
            ? readerB.ReadInt32()
            : null;
        int? numberC = readerC.BaseStream.Position < readerC.BaseStream.Length
            ? readerC.ReadInt32()
            : null;

        while ((numberB != null || numberC != null) && elCounter < maxEls)
        {
            elCounter++;
            if (indexB < SeriesNum.Count && indexC < SeriesNum.Count)
            {
                if (SeriesNum[indexB] == counterB && SeriesNum[indexC] == counterC)
                {
                    indexB += 2;
                    indexC += 2;
                    counterC = 0;
                    counterB = 0;
                    continue;
                }
            }

            if ((SeriesNum.Count <= indexB || SeriesNum[indexB] == counterB) && numberC != null)
            {
                writerA.Write(numberC.Value);
                numberC = readerC.BaseStream.Position < readerC.BaseStream.Length
                    ? readerC.ReadInt32()
                    : null;
                counterC++;
                continue;
            }

            if ((SeriesNum.Count <= indexC || SeriesNum[indexC] == counterC) && numberB != null)
            {
                writerA.Write(numberB.Value);
                numberB = readerB.BaseStream.Position < readerB.BaseStream.Length
                    ? readerB.ReadInt32()
                    : null;
                counterB++;
                continue;
            }

            if (numberB <= numberC)
            {
                writerA.Write(numberB.Value);
                numberB = readerB.BaseStream.Position < readerB.BaseStream.Length
                    ? readerB.ReadInt32()
                    : null;
                counterB++;
            }
            else if (numberC != null)
            {
                writerA.Write(numberC.Value);
                numberC = readerC.BaseStream.Position < readerC.BaseStream.Length
                    ? readerC.ReadInt32()
                    : null;
                counterC++;
            }
        }
    }
}
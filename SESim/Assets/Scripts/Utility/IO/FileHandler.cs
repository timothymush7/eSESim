using System.Collections.Generic;
using System.IO;

/// <summary>
/// Helper class for writing and reading input vectors to and from
/// a text file.
/// </summary>
public static class FileHandler
{
    public static string DELIMITER_COMMA = ",";                     // Delimiter for data fields
    public static string DEFAULT_FILEPATH = "Assets/Resources/";    // String which describes the default filepath for input vector files

    /// <summary>
    /// Utility method for writing an array of input vectors to a text file.
    /// </summary>
    /// <param name="inputVectors">Input vectors to be written to a text file.</param>
    /// <param name="filepath">Filepath of the text file which will store the input vectors.</param>
    /// <param name="useHeaders">Boolean which indicates if column headers are appended at the top of the text file.</param>
    /// <param name="appendData">Boolean which indicates if data is appended (or simply overwrite data in text file).</param>
    public static void WriteInputVectorsToFile(InputVector[] inputVectors, string filepath, bool useHeaders, bool appendData)
    {
        if (inputVectors != null)
        {
            if (inputVectors.Length > 0)
            {
                StreamWriter writer = new StreamWriter(filepath, appendData);

                if (useHeaders)
                    writer.WriteLine(inputVectors[0].GetHeadersFromInputVectorTuples());

                for (int i = 0; i < inputVectors.Length; i++)
                    writer.WriteLine(inputVectors[i].GetDataFromInputVectorTuples());

                writer.Close();
            }
        }
    }

    /// <summary>
    /// Utility method for writing a list of input vector arrays to a text file.
    /// </summary>
    /// <param name="inputVectorsList">List of input vector arrays to be written to a text file.</param>
    /// <param name="filepath">Filepath of the text file which will store the input vectors.</param>
    /// <param name="useHeaders">Boolean which indicates if column headers are appended at the top of the text file.</param>
    /// <param name="appendData">Boolean which indicates if data is appended (or simply overwrite data in text file).</param>
    public static void WriteInputVectorsListToFile(List<InputVector[]> inputVectorsList, string filepath, bool useHeaders, bool appendData)
    {
        if (inputVectorsList != null)
        {
            if (inputVectorsList.Count > 0)
            {
                StreamWriter writer = new StreamWriter(filepath, appendData);

                if (useHeaders)
                    writer.WriteLine(inputVectorsList[0][0].GetHeadersFromInputVectorTuples());

                foreach (InputVector[] inputVectors in inputVectorsList)
                    for (int i = 0; i < inputVectors.Length; i++)
                        writer.WriteLine(inputVectors[i].GetDataFromInputVectorTuples());

                writer.Close();
            }
        }
    }
}

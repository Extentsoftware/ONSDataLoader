using System.Collections.Generic;

namespace ONSLoader.Console.Csv
{
    public interface ICsvLine
    {
        /// <summary>
        /// Gets the headers from the csv file.
        /// </summary>
        List<string> Headers { get; }

        List<string> Line { get; }

        /// <summary>
        /// Gets the original raw content of the line.
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// Gets the 1-based index for the line inside the file.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the number of columns of the line.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Gets the data for the specified named header.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        string this[string name] { get; }

        /// <summary>
        /// Gets the data for the specified indexed header.
        /// </summary>
        /// <param name="index">The index of the header.</param>
        string this[int index] { get; }
    }

}

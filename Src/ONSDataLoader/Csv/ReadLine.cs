using System;
using System.Collections.Generic;

namespace ONSLoader.Console.Csv
{
    public static partial class CsvReader
    {
        private sealed class ReadLine : ICsvLine
        {
            private readonly Dictionary<string, int> headerLookup;
            private readonly CsvOptions options;
            private List<string> parsedLine;

            public ReadLine(List<string> headers, Dictionary<string, int> headerLookup, int index, string raw, CsvOptions options)
            {
                this.headerLookup = headerLookup;
                this.options = options;
                Headers = headers;
                Raw = raw;
                Index = index;
            }

            public List<string> Headers { get; }

            public string Raw { get; }

            public int Index { get; }

            public int ColumnCount => Line.Count;

            public List<string> Line
            {
                get
                {
                    if (parsedLine == null)
                    {
                        lock (headerLookup)
                        {
                            if (parsedLine == null)
                                parsedLine = SplitLine(Raw, options);

                            if (options.ValidateColumnCount && parsedLine.Count != headerLookup.Count)
                                throw new InvalidOperationException($"Expected {headerLookup.Count}, only got {parsedLine.Count} columns.");
                        }
                    }
                    return parsedLine;
                }
            }

            string ICsvLine.this[string name]
            {
                get
                {
                    if (!headerLookup.TryGetValue(name, out int index))
                        throw new ArgumentOutOfRangeException(nameof(name), name, $"Header '{name}' does not exist. Expected one of {string.Join("; ", headerLookup.Keys)}");

                    try
                    {
                        return Line[index];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException($"Invalid row, missing {name} header, expected {headerLookup.Count} columns, only got {Line.Count}");
                    }
                }
            }

            string ICsvLine.this[int index] => Line[index];

            public override string ToString()
            {
                return Raw;
            }
        }
    }

}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ONSLoader.Console.Csv
{
    public static partial class CsvReader
    {
        private static readonly Dictionary<char, Regex> splitterCache = new Dictionary<char, Regex>();
        private static readonly object syncRoot = new object();

        /// <summary>
        /// Reads the lines from the reader.
        /// </summary>
        /// <param name="reader">The text reader to read the data from.</param>
        /// <param name="options">The optional options to use when reading.</param>
        public static IEnumerable<ICsvLine> Read(TextReader reader, CsvOptions options = null)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return ReadImpl(reader, options);
        }

        /// <summary>
        /// Reads the lines from the stream.
        /// </summary>
        /// <param name="stream">The stream to read the data from.</param>
        /// <param name="options">The optional options to use when reading.</param>
        public static IEnumerable<ICsvLine> ReadFromStream(Stream stream, CsvOptions options = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return ReadFromStreamImpl(stream, options);
        }

        /// <summary>
        /// Reads the lines from the csv string.
        /// </summary>
        /// <param name="csv">The csv string to read the data from.</param>
        /// <param name="options">The optional options to use when reading.</param>
        public static IEnumerable<ICsvLine> ReadFromText(string csv, CsvOptions options = null)
        {
            if (csv == null)
                throw new ArgumentNullException(nameof(csv));

            return ReadFromTextImpl(csv, options);
        }

        private static IEnumerable<ICsvLine> ReadFromStreamImpl(Stream stream, CsvOptions options)
        {
            using var reader = new StreamReader(stream);
            foreach (var line in ReadImpl(reader, options))
                yield return line;
        }

        private static IEnumerable<ICsvLine> ReadFromTextImpl(string csv, CsvOptions options)
        {
            using var reader = new StringReader(csv);
            foreach (var line in ReadImpl(reader, options))
                yield return line;
        }

        private static IEnumerable<ICsvLine> ReadImpl(TextReader reader, CsvOptions options)
        {
            if (options == null)
                options = new CsvOptions();

            string line;
            var index = 0;
            List< string> headers = null;
            Dictionary<string, int> headerLookup = null;
            var initalized = false;
            while ((line = reader.ReadLine()) != null)
            {
                index++;
                if (index <= options.RowsToSkip || options.SkipRow?.Invoke(line, index) == true)
                    continue;

                if (!initalized)
                {
                    InitalizeOptions(line, options);
                    var skipInitialLine = options.HeaderMode == HeaderMode.HeaderPresent;

                    headers = skipInitialLine ? GetHeaders(line, options) : CreateDefaultHeaders(line, options);

                    headerLookup = headers.Select((h, idx) => Tuple.Create(h, idx)).ToDictionary(h => h.Item1, h => h.Item2, options.Comparer);

                    initalized = true;

                    if (skipInitialLine)
                        continue;
                }

                yield return new ReadLine(headers, headerLookup, index, line, options);
            }
        }

        private static char AutoDetectSeparator(string sampleLine)
        {
            // NOTE: Try simple 'detection' of possible separator
            if (sampleLine.Contains(";"))
                return ';';
            if (sampleLine.Contains("\t"))
                return '\t';
            return ',';
        }

        private static List<string> CreateDefaultHeaders(string line, CsvOptions options)
        {
            var columnCount = options.Splitter.Matches(line);

            var headers = new string[columnCount.Count];

            for (var i = 0; i < headers.Length; i++)
                headers[i] = $"Column{i + 1}";

            return headers.ToList();
        }

        private static List<string> GetHeaders(string line, CsvOptions options)
        {
            return SplitLine(line, options);
        }

        private static void InitalizeOptions(string line, CsvOptions options)
        {
            if (options.Separator == '\0')
                options.Separator = AutoDetectSeparator(line);


            Regex splitter;
            lock (syncRoot)
            {
                if (!splitterCache.TryGetValue(options.Separator, out splitter))
                    splitterCache[options.Separator] = splitter = new Regex(string.Format(@"(?>(?(IQ)(?(ESC).(?<-ESC>)|\\(?<ESC>))|(?!))|(?(IQ)\k<QUOTE>(?<-IQ>)|(?<QUOTE>"")(?<IQ>))|(?(IQ).|[^{0}]))+|^(?={0})|(?<={0})(?={0})|(?<={0})$", Regex.Escape(options.Separator.ToString())), (RegexOptions)8);
            }

            options.Splitter = splitter;
        }

        private static List<string> SplitLine(string line, CsvOptions options)
        {
            var matches = options.Splitter.Matches(line);
            var parts = new string[matches.Count];
            for (var i = 0; i < matches.Count; i++)
            {
                var str = matches[i].Value;
                if (options.TrimData)
                    str = str.Trim();

                if (str.StartsWith("\"") && str.EndsWith("\""))
                    str = str[1..^1];

                parts[i] = str;
            }
            return parts.ToList();
        }
    }
}

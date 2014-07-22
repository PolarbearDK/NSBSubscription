using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Miracle.NSBSubscription
{
    /// <summary>
    /// Format object list into column table (like powershell)
    /// </summary>
    public static class ColumnFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        public static void WriteColumns<T>(TextWriter writer, IEnumerable<T> items, IEnumerable<Tuple<string,Func<T, object>>> columns)
        {
            var width = new int[columns.Count()];
            var leftAligned = new bool[columns.Count()];
            int col;

            // Measure column width and calculate column alignment.
            col = 0;
            foreach (var column in columns)
            {
                width[col++] = column.Item1.Length;
            }

            foreach (var item in items)
            {
                col = 0;
                foreach (var column in columns)
                {
                    var content = column.Item2(item);

                    if (leftAligned[col] == false && !IsNumericType(content))
                        leftAligned[col] = true;

                    if (content != null)
                        width[col] = Math.Max(width[col], content.ToString().Length);
                    col++;
                }
            }

            writer.WriteLine(); // Looks better with an empty line before a column table

            // Write Header    
            WriteColumns(writer, width, leftAligned, columns.Select(x=>x.Item1));
            // Write dashes (-) under header
            WriteColumns(writer, width, leftAligned, columns.Select(x=> new string('-', x.Item1.Length)));
            // Write item data
            foreach (var command in items)
            {
                WriteColumns(writer, width, leftAligned, columns.Select(x => x.Item2(command).ToString()));
            }
        }

        private static void WriteColumns(TextWriter writer, int[] width, bool[] leftAlighed, IEnumerable<string> columns)
        {
            int col = 0;
            int spaces = 0;
            foreach (var column in columns)
            {
                if (!string.IsNullOrWhiteSpace(column))
                {
                    while (spaces > 0)
                    {
                        writer.Write(' ');
                        spaces--;
                    }

                    spaces = width[col] - column.Length + 1;
                    if(!leftAlighed[col])
                        while (spaces > 1)
                        {
                            writer.Write(' ');
                            spaces--;
                        }

                    writer.Write(column);
                }
                else
                    spaces += width[col] + 1;

                col++;
            }
            writer.WriteLine();
        }

        private static readonly Type[] NumericTypes =
            {
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),
                typeof (byte),
                typeof (sbyte),
                typeof (short),
                typeof (ushort),
                typeof (decimal),
                typeof (float),
                typeof (double),
            };

        private static bool IsNumericType(object content)
        {
            Type type = content.GetType();
            return NumericTypes.Contains(type);
        }
    }
}
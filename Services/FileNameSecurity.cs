using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace college_events_desktop.Services
{
    public static class FileNameSecurity
    {
        private static readonly char[] InvalidChars = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Формирует безопасное имя файла из произвольных частей.
        /// </summary>
        /// <param name="extension">Расширение с точкой, например ".xlsx".</param>
        /// <param name="maxLength">Максимальная длина имени без расширения (по умолчанию 200).</param>
        /// <param name="parts">Части имени (между ними ставится '_'). null/пустые пропускаются.</param>
        public static string Build(string extension, int maxLength = 200, params string[] parts)
        {
            var cleaned = parts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(Clean)
                .Where(p => p.Length > 0)
                .ToList();

            var name = string.Join("_", cleaned);

            if (name.Length == 0)
                name = "file";

            if (name.Length > maxLength)
            {
                name = name.Substring(0, maxLength).TrimEnd('_', ' ', '.');
            }

            if (!extension.StartsWith(".", StringComparison.Ordinal))
            {
                extension = "." + extension;
            }

            return name + extension;
        }

        private static string Clean(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            var sb = new StringBuilder(input.Length);
            foreach (var ch in input)
            {
                if (Array.IndexOf(InvalidChars, ch) >= 0 || char.IsControl(ch))
                    sb.Append(' ');
                else
                    sb.Append(ch);
            }
            var result = string.Join(" ",
                sb.ToString().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            return result.Trim(' ', '.');
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Damco.Common
{
    /// <summary>
    /// Define the block of string which further will be used for other string operations.
    /// </summary>
    public class StringBlock
    {
        public StringBlock(string start, string end, params StringBlock[] children)
        {
            this.Start = start;
            this.Ends = new string[] { end };
            if (children != null)
                this.AddChildren(children);
        }

        public StringBlock(string start, int length, params StringBlock[] children)
        {
            this.Start = start;
            this.Length = length;
            if (children != null)
                this.AddChildren(children);
        }

        public StringBlock(string start, StringFindOptions startFindOptions, string end, StringFindOptions endFindOptions, params StringBlock[] children) : this(start, end, children)
        {
            this.StartFindOptions = startFindOptions;
            this.EndFindOptions = endFindOptions;
        }

        public StringBlock(string start, StringFindOptions startFindOptions, int length, params StringBlock[] children) : this(start, length, children)
        {
            this.StartFindOptions = startFindOptions;
        }

        public StringBlock(string start, StringFindOptions startFindOptions, string[] ends, StringFindOptions endFindOptions, params StringBlock[] children)
        {
            this.Start = start;
            this.Ends = ends;
            if (children != null)
                this.AddChildren(children);
        }

        public void AddChildren(params StringBlock[] children)
        {
            this.Children.AddRange(children);
        }

        public string Start { get; set; }
        public string[] Ends { get; set; } = null;
        public int? Length { get; set; }
        public List<StringBlock> Children { get; } = new List<StringBlock>();
        public StringFindOptions StartFindOptions { get; set; } = StringFindOptions.None;
        public StringFindOptions EndFindOptions { get; set; } = StringFindOptions.None;

        public override string ToString()
        {
            if (this.Start != null && this.Ends != null)
                return this.Start + "..." + this.Ends.JoinStrings(",");
            else if (this.Start != null && this.Length != null)
                return this.Start + new string('.', this.Length.Value - this.Start.Length);
            else if (this.Start != null)
                return this.Start;
            else
                return base.ToString();
        }
    }

    [Serializable]
    [Flags()]
    public enum StringFindOptions
    {
        None = 0,
        SearchFromStart = 1,
        SearchFromEnd = 2,
        FullTextIfNotFound = 4,
        CaseInsensitive = 8,
        NullIfNotFound = 16
    }


    /// <summary>
    /// Extension methods on String.
    /// </summary>

    public static class StringExtensions
    {
        public static bool CoalescedFuzzyEquals(this string text, string value)
        {
            if (string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(value))
                return true;
            else if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(value))
                return false;
            else if (!string.IsNullOrWhiteSpace(text) && string.IsNullOrWhiteSpace(value))
                return false;
            else //No nulls
                return string.Compare(
                    new string(text.RemoveDiacritics().Where(c => char.IsLetterOrDigit(c)).ToArray()),
                    new string(value.RemoveDiacritics().Where(c => char.IsLetterOrDigit(c)).ToArray()),
                    StringComparison.OrdinalIgnoreCase
                ) == 0;
        }

        public static string RemoveDiacritics(this string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static string ReplaceBlocks(this string source, Func<StringBlock, string, string> replacementFactory, params StringBlock[] blocks)
        {
            if (source.Length == 0)
                return source;
            StringBuilder rootBuilder = new StringBuilder();
            Stack<Tuple<StringBlock, StringBuilder>> ancestorBlocks = new Stack<Tuple<StringBlock, StringBuilder>>();
            var currentBlock = Tuple.Create(default(StringBlock), new StringBuilder()); //Root
            int i = 0;
            while (i < source.Length)
            {
                int endCurrentBlock = -1;
                if (currentBlock.Item1 != null)
                    endCurrentBlock = currentBlock.Item1.Ends.Min(e => source.IndexOf(e, currentBlock.Item1.EndFindOptions, i));
                var firstChildBlock =
                    ((IEnumerable<StringBlock>)currentBlock.Item1?.Children ?? (IEnumerable<StringBlock>)blocks)
                    .Select(b => Tuple.Create(b, source.IndexOf(b.Start, b.StartFindOptions, i)))
                    .Where(t => t.Item2 != -1)
                    .OrderBy(t => t.Item2).ThenByDescending(t => t.Item1.Start.Length)
                    .FirstOrDefault();

                //Child block found?
                if (firstChildBlock != null && (endCurrentBlock == -1 || firstChildBlock.Item2 <= endCurrentBlock)) //Start of a child block before end of this block
                {
                    currentBlock.Item2.Append(source.Substring(i, firstChildBlock.Item2 - i)); //All text until the new block is part of the current block
                    //string blockStart = source.Substring(firstChildBlock.Item2, firstChildBlock.Item1.Start.Length); 
                    if (firstChildBlock.Item1.Ends == null) //Fixed-length block
                    {
                        if (firstChildBlock.Item1.Children.Count > 0)
                            throw new InvalidOperationException("Fixed-length string blocks cannot have child blocks");
                        string blockText = source.Substring(firstChildBlock.Item2, Math.Min(firstChildBlock.Item1.Length.Value, source.Length - firstChildBlock.Item2));
                        blockText = replacementFactory(firstChildBlock.Item1, blockText);
                        currentBlock.Item2.Append(blockText);
                        i = firstChildBlock.Item2 + firstChildBlock.Item1.Length.Value;
                    }
                    else
                    {
                        ancestorBlocks.Push(currentBlock);
                        currentBlock = Tuple.Create(firstChildBlock.Item1, new StringBuilder());
                        currentBlock.Item2.Append(source.Substring(firstChildBlock.Item2, firstChildBlock.Item1.Start.Length)); //Getting start from "source" because we want the real casing in case of case insensitive search.
                        i = firstChildBlock.Item2 + firstChildBlock.Item1.Start.Length;
                    }
                }
                else if (endCurrentBlock > -1) //Found the end of the current block 
                {
                    //Find the correct (first string that matches)
                    string end = currentBlock.Item1.Ends
                        .First(e => string.Compare(source.Substring(endCurrentBlock, e.Length), e, GetComparison(currentBlock.Item1.EndFindOptions)) == 0);
                    currentBlock.Item2.Append(source.Substring(i, endCurrentBlock - i + end.Length)); //All text until the end is part of the current block
                    string blockText = currentBlock.Item2.ToString();
                    blockText = replacementFactory(currentBlock.Item1, blockText);
                    i = endCurrentBlock + (end?.Length).GetValueOrDefault(1);
                    currentBlock = ancestorBlocks.Pop();
                    currentBlock.Item2.Append(blockText);
                }
                else //End of the text, we're done
                {
                    currentBlock.Item2.Append(source.Substring(i)); //All text until the end is part of the current block
                    break;
                }
            }
            {
                string blockText = currentBlock.Item2.ToString();
                blockText = replacementFactory(currentBlock.Item1, blockText);
                //Add the text to all ancestors
                while (ancestorBlocks.Count > 0)
                {
                    currentBlock = ancestorBlocks.Pop();
                    currentBlock.Item2.Append(blockText);
                    blockText = currentBlock.Item2.ToString();
                    blockText = replacementFactory(currentBlock.Item1, blockText);
                }
                return currentBlock.Item2.ToString(); //Root
            }
        }

        private static void ValidateOptions(StringFindOptions options)
        {
            if ((options & StringFindOptions.SearchFromStart) != 0 && (options & StringFindOptions.SearchFromEnd) != 0)
                throw new ArgumentException($"'{nameof(options)}' cannot have both {nameof(StringFindOptions.SearchFromStart)} and {nameof(StringFindOptions.SearchFromEnd)}");
            if ((options & StringFindOptions.FullTextIfNotFound) != 0 && (options & StringFindOptions.NullIfNotFound) != 0)
                throw new ArgumentException($"'{nameof(options)}' cannot have both {nameof(StringFindOptions.FullTextIfNotFound)} and {nameof(StringFindOptions.NullIfNotFound)}");
        }

        private static StringComparison GetComparison(StringFindOptions options)
        {
            return ((options & StringFindOptions.CaseInsensitive) != 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }


        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// <para>Use specified search option and starting position, while searching for the occurence. </para>
        /// </summary>
        /// <param name="value">Source string.</param>
        /// <param name="valueToFind">Value to be searched.</param>
        /// <param name="options">Type of Search.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>The zero-based index position of value found, or -1 if it is not.</returns>
        private static int IndexOf(this string value, string valueToFind, StringFindOptions options, int startIndex)
        {
            //TODO: Handle escape chars
            ValidateOptions(options);
            if ((options & StringFindOptions.SearchFromEnd) != 0)
                return value.LastIndexOf(valueToFind, startIndex, GetComparison(options));
            else
                return value.IndexOf(valueToFind, startIndex, GetComparison(options));
        }
        /// <summary>
        /// Retrieves a substring from this instance. The substring is in between the start and end character specified.
        /// </summary>
        /// <param name="value">Current instance of the string.</param>
        /// <param name="start">Starting character after which a substring will start.</param>
        /// <param name="end">Ending character before which asubstring will end.</param>
        /// <returns>A string that is equivalent to the substring of length which depends upon start and end character position.></returns>
        public static string Substring(this string value, string start, string end)
        {
            return value.Substring(start, StringFindOptions.None, end, StringFindOptions.None);
        }

        /// <summary>
        ///  Retrieves a substrings from this instance. 
        ///  <para> No of substring will be equal to the no.of occurrence of specified start and end character in the source string. </para>
        /// </summary>
        /// <param name="value">Current instance of the string.</param>
        /// <param name="start">Starting character after which a substring will start.</param>
        /// <param name="end">Ending character before which asubstring will end.</param>
        /// <returns>List of string.</returns>
        public static IEnumerable<string> Substrings(this string value, string start, string end)
        {
            return value.Substrings(start, StringFindOptions.None, end, StringFindOptions.None);
        }

        public static IEnumerable<string> Substrings(this string value, string start, StringFindOptions startOptions, string end, StringFindOptions endOptions)
        {
            ValidateOptions(startOptions);
            ValidateOptions(endOptions);
            if ((startOptions & StringFindOptions.FullTextIfNotFound) == 0 && (startOptions & StringFindOptions.NullIfNotFound) == 0)
                startOptions |= StringFindOptions.NullIfNotFound;
            if ((endOptions & StringFindOptions.FullTextIfNotFound) == 0 && (startOptions & StringFindOptions.NullIfNotFound) == 0)
                startOptions |= StringFindOptions.NullIfNotFound;
            while (true)
            {
                int? startIndex = GetStartIndex(value, start, startOptions);
                if (startIndex == null)
                    yield break;
                int? endIndex = GetEndIndex(value, startIndex.Value, end, endOptions);
                if (endIndex == null)
                    yield break;
                yield return value.Substring(startIndex.Value, endIndex.Value - startIndex.Value + 1);
                value = value.Substring(endIndex.Value + end.Length);
            }
        }

        private static int? GetStartIndex(string value, string start, StringFindOptions options)
        {
            if (start == null)
                return 0;
            else
            {
                int startIndex;
                if ((options & StringFindOptions.SearchFromEnd) != 0)
                    startIndex = value.Length;
                else
                    startIndex = 0;
                startIndex = value.IndexOf(start, options, startIndex);
                if (startIndex == -1)
                {
                    if ((options & StringFindOptions.FullTextIfNotFound) != 0)
                        startIndex = 0;
                    else if ((options & StringFindOptions.NullIfNotFound) != 0)
                        return null;
                    else
                        throw new ArgumentException("'start' not found", "start");
                }
                startIndex += start.Length;
                return startIndex;
            }
        }

        private static int? GetEndIndex(this string value, int startIndex, string end, StringFindOptions options)
        {
            int endIndex;
            if (end == null)
                return value.Length - 1;
            else
            {
                if ((options & StringFindOptions.SearchFromEnd) != 0)
                    endIndex = value.Length;
                else
                    endIndex = startIndex;
                endIndex = value.IndexOf(end, options, endIndex);
                if (endIndex == -1)
                {
                    if ((options & StringFindOptions.FullTextIfNotFound) != 0)
                        endIndex = value.Length;
                    else if ((options & StringFindOptions.NullIfNotFound) != 0)
                        return null;
                    else
                        throw new ArgumentException("'end' not found", "ends");
                }
                endIndex--; //We wan't the last character of the found text
                return endIndex;
            }
        }
        public static string Substring(this string value, string start, StringFindOptions startOptions, string end, StringFindOptions endOptions)
        {
            int? startIndex = GetStartIndex(value, start, startOptions);
            if (startIndex == null)
                return null;
            int? endIndex = GetEndIndex(value, startIndex.Value, end, endOptions);
            if (endIndex == null)
                return null;
            return value.Substring(startIndex.Value, endIndex.Value - startIndex.Value + 1);
        }

        public static string SubstringAfter(this string value, string text)
        {
            return value.SubstringAfter(text, StringFindOptions.None);
        }
        public static string SubstringAfter(this string value, string text, StringFindOptions options)
        {
            ValidateOptions(options);
            int? startIndex = GetStartIndex(value, text, options);
            if (startIndex == null)
                return null;
            return value.Substring(startIndex.Value);
        }
        public static string SubstringBefore(this string value, string text)
        {
            return value.SubstringBefore(text, StringFindOptions.None);
        }
        public static string SubstringBefore(this string value, string text, StringFindOptions options)
        {
            ValidateOptions(options);
            int? endIndex = GetEndIndex(value, 0, text, options);
            if (endIndex == null)
                return null;
            return value.Substring(0, endIndex.Value + 1);
        }

        public static string LimitLength(this string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;
            else
                return value.Substring(0, maxLength);
        }

        public static string LimitLength(this string value, int maxLength, string partialIndicator)
        {
            if (value.Length <= maxLength)
                return value;
            else
                return string.Concat(value.Substring(0, maxLength - partialIndicator.Length), partialIndicator);
        }

        //Sentence can be e.g.: There [is][are] {0} item[s]
        public static string ToSentence(this int count, string sentence)
        {
            if (count == 1)
                return string.Format(GetOneText(sentence), count);
            else
                return string.Format(GetMultipleText(sentence), count);
        }

        //Sentence can be e.g.: Item[s] [is][are]: {0}
        public static string ToSentence(this IEnumerable<string> list, string sentence, string separator)
        {
            if (list.Count() == 1)
                return string.Format(GetOneText(sentence), list.JoinStrings(separator));
            else
                return string.Format(GetMultipleText(sentence), list.JoinStrings(separator));
        }

        static Regex _findMultiOptionText = new Regex(@"\[(?<one>[^]]*)\]\[(?<multi>[^]]*)\]"); //There [is][are] ...
        static Regex _findBracketedText = new Regex(@"\[[^]]*\]"); //{0} rate[s]
        private static string GetOneText(string multipleTextWithBracketsAroundMultipleParts)
        {
            return
                _findBracketedText.Replace(
                    _findMultiOptionText.Replace(multipleTextWithBracketsAroundMultipleParts, m => m.Groups["one"].Value) //use "one" text.
                , m => "");
        }
        private static string GetMultipleText(string multipleTextWithBracketsAroundMultipleParts)
        {
            return
                _findBracketedText.Replace(
                    _findMultiOptionText.Replace(multipleTextWithBracketsAroundMultipleParts, m => m.Groups["multi"].Value)
                , m => m.Value.Substring("[".Length, m.Value.Length - "[]".Length));
        }

        /// <summary>
        /// Join multiple strings without separator.
        /// </summary>
        /// <param name="input">List of string to be joined.</param>
        /// <returns>Single string  without any separator.</returns>
        public static string JoinStrings(this IEnumerable<string> input)
        {
            return input.JoinStrings("");
        }

        /// <summary>
        /// Join multiple strings with the specified separator.
        /// </summary>
        /// <param name="input">List of string to be joined.</param>
        /// <param name="separator">separator between two string join.</param>
        /// <returns>Single string with separator inbetween the two consecutive strings.</returns>
        public static string JoinStrings(this IEnumerable<string> input, string separator)
        {
            var result = new StringBuilder();
            var oneDone = false;
            foreach (var item in input)
                if (item != null)
                {
                    if (oneDone)
                        result.Append(separator);
                    result.Append(item);
                    oneDone = true;
                }
            if (!oneDone)
                return null;
            else
                return result.ToString();
        }

    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tf9ik
{
    public class Leksem
    {
        public Leksem(int index, LeksemType type)
        {
            Index = index;
            Type = type;
        }
        public int Index { get; set; }
        public LeksemType Type { get; set; }
    }
    public enum LeksemType
    {
        Printf,
        QuotationMarks,
        Word,
        Delimiter,
        Specifier,
        Id,
        Number,
        Unknown,
        OpeningBracket,
        ClosingBracket,
        Comma,
        Semicolon
    }
    static internal class Scaner
    {
        private static string str;
        public static bool isPrintf(int index)
        {
            if (str.Length - index < 6)
                return false;
            if (str.Substring(index, 6) == "printf")
                return true;
            return false;
        }

        public static List<Leksem> GetLeksem(string s)
        {
            bool afterComma = false;
            str = s;
            var listLeksem = new List<Leksem>();
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsWhiteSpace(str[i]))
                {
                    listLeksem.Add(new Leksem(i, LeksemType.Delimiter));
                    continue;
                }

                if (isPrintf(i))
                {
                    listLeksem.Add(new Leksem(i, LeksemType.Printf));
                    i += 5;
                    continue;
                }

                if (str[i] == '"')
                {
                    listLeksem.Add(new Leksem(i, LeksemType.QuotationMarks));
                    continue;
                }

                if (str[i] == '%')
                {
                    if (str.Length > i + 1 && (str[i + 1] == 'c' || str[i + 1] == 'd' | str[i + 1] == 'i'))
                    {
                        listLeksem.Add(new Leksem(i, LeksemType.Specifier));
                        i++;
                        continue;
                    }
                    listLeksem.Add(new Leksem(i, LeksemType.Unknown));
                }

                if (str[i] == '%')
                {
                    if (str.Length > i + 1 && (str[i + 1] == 'c' || str[i + 1] == 'd' | str[i + 1] == 'i'))
                    {
                        listLeksem.Add(new Leksem(i, LeksemType.QuotationMarks));
                        i++;
                        continue;
                    }
                    listLeksem.Add(new Leksem(i, LeksemType.Unknown));
                }

                if (Char.IsLetterOrDigit(str[i]) && !afterComma)
                {
                    var startIndex = i;
                    while (i + 1 < str.Length && Char.IsLetterOrDigit(str[i + 1]))
                    {
                        i++;
                    }
                    listLeksem.Add(new Leksem(startIndex, LeksemType.Word));
                    continue;
                }

                if (Char.IsLetter(str[i]) && afterComma)
                {
                    var startIndex = i;
                    while (i + 1 < str.Length && Char.IsLetterOrDigit(str[i + 1]))
                    {
                        i++;
                    }
                    listLeksem.Add(new Leksem(startIndex, LeksemType.Id));
                    continue;
                }

                if (Char.IsNumber(str[i]))
                {
                    var startIndex = i;
                    while (i + 1 < str.Length && Char.IsNumber(str[i + 1]))
                    {
                        i++;
                    }
                    listLeksem.Add(new Leksem(startIndex, LeksemType.Number));
                    continue;
                }

                if (str[i] == '(')
                {
                    listLeksem.Add(new Leksem(i, LeksemType.OpeningBracket));
                    continue;
                }

                if (str[i] == ')' )
                {
                    listLeksem.Add(new Leksem(i, LeksemType.ClosingBracket));
                    continue;
                }

                if (str[i] == ',' )
                {
                    listLeksem.Add(new Leksem(i, LeksemType.Comma));
                    afterComma = true;
                    continue;
                }

                if (str[i] == ';' )
                {
                    listLeksem.Add(new Leksem(i, LeksemType.Semicolon));
                    continue;
                }

                listLeksem.Add(new Leksem(i, LeksemType.Unknown));
            }
            return listLeksem; ;
        }
    }
}


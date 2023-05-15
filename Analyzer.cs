using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tf9ik
{
    public static class Analyzer
    {
        public class Error
        {
            public int Position;
            public string Message;
            public Error(int _position, string _message)
            {
                Position = _position;
                Message = _message;
            }
        }

        private static List<Error> errors;
        private static StringBuilder result;
        private static int index;
        private static int lastIndex;
        private static List<Leksem> leksems;

        private static void SkipDelimiter()
        {
            while (index < leksems.Count && leksems[index].Type == LeksemType.Delimiter)
                index++;
        }

        private static void SkipDelimiterAndUnknown()
        {
            while (index < leksems.Count && (leksems[index].Type == LeksemType.Delimiter || leksems[index].Type == LeksemType.Unknown))
                index++;
        }

        private static bool SkipWhile(LeksemType type)
        {
            int count = 0;
            var tmpIndex = index;
            while (tmpIndex < leksems.Count && leksems[tmpIndex].Type != type)
            {
                if (leksems[tmpIndex].Type != LeksemType.Delimiter)
                    count++;
                tmpIndex++;
            }

            if (count == 0 && tmpIndex != leksems.Count)
                return true;

            if (tmpIndex < leksems.Count && leksems[tmpIndex].Type == type)
                index = tmpIndex;

            return false;
        }

        private static bool SkipWhile2(LeksemType type1, LeksemType type2)
        {
            int count = 0;

            var tmpIndex = index;
            while (tmpIndex < leksems.Count && leksems[tmpIndex].Type != type1 && leksems[tmpIndex].Type != type2)
            {
                if (leksems[tmpIndex].Type != LeksemType.Delimiter)
                    count++;
                tmpIndex++;
            }

            if (count == 0 && tmpIndex != leksems.Count)
                return true;

            if (tmpIndex < leksems.Count && tmpIndex < leksems.Count)
                index = tmpIndex;

            return false;
        }

        public static string GetResult(List<Leksem> _leksems)
        {
            index = 0;
            leksems = _leksems;
            if (leksems.Count == 0)
                lastIndex = 0;
            else
                lastIndex = leksems[leksems.Count - 1].Index;
            errors = new List<Error>();

            Operator();

            if (errors.Count == 0)
                return "Ошибок нет";

            result = new StringBuilder();
            foreach (var error in errors)
            {
                result.AppendLine($"Ошибка. Позиция {error.Position + 1}: {error.Message}");
            }
            return result.ToString();
        }

        private static void Operator()
        {
            var previousIndex = 0;
            while (index < leksems.Count && leksems[index].Type == LeksemType.Delimiter) index++;

            if (index >= leksems.Count || leksems[index].Type != LeksemType.Printf)
            {
                errors.Add(new Error(index < leksems.Count ? leksems[index].Index : lastIndex, "Отсутствует printf"));
            }

            index++;

            previousIndex = index;
            if (!SkipWhile(LeksemType.OpeningBracket) && index < leksems.Count && leksems[index].Type == LeksemType.OpeningBracket)
            {
                errors.Add(new Error(previousIndex < leksems.Count ? leksems[previousIndex].Index : lastIndex, "Встречен символ, отличный от открывающей скобки"));
                index++;
            }
            else if (index >= leksems.Count || leksems[index].Type != LeksemType.OpeningBracket)
            {
                errors.Add(new Error(previousIndex < leksems.Count ? leksems[previousIndex].Index : lastIndex, "Не найдена открывающая скобка"));
            }
            else
            {
                index++;
            }

            String();

            Argument();


            previousIndex = index;

            if (index < leksems.Count && !SkipWhile(LeksemType.ClosingBracket) && index != previousIndex )
                errors.Add(new Error(previousIndex < leksems.Count ? leksems[previousIndex].Index : lastIndex, "Неверный формат аргумента"));

            if (index >= leksems.Count || leksems[index].Type != LeksemType.ClosingBracket)
            {
                errors.Add(new Error(index < leksems.Count ? leksems[index].Index : lastIndex, "Не найдена закрывающая скобка"));
            }
            else
            {
                index++;
            }

            SkipWhile(LeksemType.Semicolon);

            if (index >= leksems.Count || leksems[index].Type != LeksemType.Semicolon)
            {
                errors.Add(new Error(index < leksems.Count ? leksems[index].Index : lastIndex, "Не найдена точка с запятой"));
                index++;
            }
        }

        private static void String()
        {
            SkipDelimiterAndUnknown();
            if (index >= leksems.Count || leksems[index].Type != LeksemType.QuotationMarks)
            {
                errors.Add(new Error(index < leksems.Count ? leksems[index].Index : lastIndex, "Не найдены открывающие кавычки"));
            }
            else
            {
                index++;
            }

            while (index < leksems.Count && (leksems[index].Type == LeksemType.Word ||
                leksems[index].Type == LeksemType.Delimiter || leksems[index].Type == LeksemType.Specifier))
            {
                index++;
            }

            var previousIndex = index;
            if (!SkipWhile(LeksemType.QuotationMarks) && leksems[index].Type == LeksemType.QuotationMarks)
            {
                errors.Add(new Error(index, "Неверный формат строки"));
            }
            if (index >= leksems.Count || leksems[index].Type != LeksemType.QuotationMarks)
            {
                errors.Add(new Error(previousIndex < leksems.Count ? leksems[previousIndex].Index : lastIndex, "Не найдены закрывающие кавычки"));
            }
            else
            {
                index++;
            }
        }

        private static void Argument()
        {
            SkipDelimiter();
            while (index < leksems.Count && leksems[index].Type == LeksemType.Comma)
            {
                index++;
                SkipDelimiter();
                var previousIndex = index;
                if (index < leksems.Count)
                {
                    if (!SkipWhile2(LeksemType.Id, LeksemType.Number))
                    {
                        errors.Add(new Error(previousIndex < leksems.Count ? leksems[previousIndex].Index : lastIndex, "Неверный формат аргумента"));
                        if (index < leksems.Count && (leksems[index].Type == LeksemType.Id || leksems[index].Type == LeksemType.Number))
                            index++;
                    }
                    else
                    {
                        index++;
                    }
                }
                SkipDelimiter();
            }
        }
    }
}

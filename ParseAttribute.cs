using System;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ParseAttribute : Attribute
    {
        private readonly string _text;

        public ParseAttribute(string text)
        {
            _text = text;
        }

        public Regex CreateRegex()
        {
            var text = "^" + _text + "$";
            return new Regex(text, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
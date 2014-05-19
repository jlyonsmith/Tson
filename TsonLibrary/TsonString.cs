using System;
using System.Collections.Generic;

namespace TsonLibrary
{
    public class TsonString
	{
        public TsonString(string s, bool isQuoted = false)
        {
            this.Value = s;
            this.IsQuoted = isQuoted;
        }

        public string Value { get; set; }
        public bool IsQuoted { get; set; }

        public static implicit operator string(TsonString qs)
        {
            return qs.Value;
        }

        public static implicit operator TsonString(string s)
        {
            return new TsonString(s);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var qs = obj as TsonString;

            if (qs == null)
                return false;

            return qs.Value == Value && qs.IsQuoted == IsQuoted;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode() ^ IsQuoted.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[TsonString: Value=\"{0}\", IsQuoted={1}]", Value, IsQuoted);
        }
	}
}


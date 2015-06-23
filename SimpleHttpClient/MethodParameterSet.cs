using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpClient
{
    public class MethodParameterSet : SortedSet<IMethodParameter>
    {
        private class MethodParameterComparer : IComparer<IMethodParameter>
        {
            public int Compare(IMethodParameter x, IMethodParameter y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        public MethodParameterSet()
            : base(new MethodParameterComparer())
        { }

        public MethodParameterSet(IEnumerable<IMethodParameter> collection)
            : base(collection, new MethodParameterComparer())
        { }

        internal MethodParameterSet(string formUrlEncodedValue)
            : base(new MethodParameterComparer())
        {
            if (formUrlEncodedValue == null)
                throw new ArgumentNullException("formUrlEncodedValue");

            if (formUrlEncodedValue.Contains("&") || formUrlEncodedValue.Contains("="))
            {
                var pairs = formUrlEncodedValue.Split('&');
                foreach (var pair in pairs)
                {
                    var nameValue = pair.Split('=');
                    if (nameValue.Length < 2)
                        continue;

                    this.Add(nameValue[0], System.Net.WebUtility.UrlDecode(nameValue[1]));
                }
            }
        }

        public void Add(string name, long value, long? defaultValue = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            if (!defaultValue.HasValue || defaultValue.Value != value)
                Add(new StringMethodParameter(name, value));
        }

        public void Add(string name, double value, double? defaultValue = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            if (!defaultValue.HasValue || defaultValue.Value != value)
                Add(new StringMethodParameter(name, value));
        }

        public void Add(string name, int value, int? defaultValue = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            if (!defaultValue.HasValue || defaultValue.Value != value)
                Add(new StringMethodParameter(name, value));
        }

        public void Add(string name, bool value, bool? defaultValue = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            if (!defaultValue.HasValue || defaultValue.Value != value)
                Add(new StringMethodParameter(name, value));
        }

        public void Add(string name, string value, string defaultValue = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            if (defaultValue != value)
                Add(new StringMethodParameter(name, value));
        }
        internal string ToFormUrlEncoded()
        {
            return String.Join("&", this.Where(c => c is StringMethodParameter).Select(c => String.Format("{0}={1}", c.Name, Encode(c))));
        }

        internal string ToAuthorizationHeader()
        {
            return String.Join(",", this.Select(c => String.Format("{0}=\"{1}\"", c.Name, Encode(c))));
        }

        private string Encode(IMethodParameter p)
        {
            return UrlEncoder.Encode(((StringMethodParameter)p).Value);
        }
    }
}
    internal static class UrlEncoder
    {
        private const string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789~.-_";

        public static string Encode(string value)
        {
            if (value == null)
                return String.Empty;

            var bytes = System.Text.Encoding.UTF8.GetBytes(value);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in bytes)
            {
                char c = (char)b;
                if (unreservedChars.IndexOf(c) >= 0)
                    sb.Append(c);
                else
                    sb.Append(String.Format("%{0:X2}", b));
            }

            return sb.ToString();
        }

        public static string Decode(string value)
        {
            if (value == null)
                return String.Empty;

            char[] chars = value.ToCharArray();

            List<byte> buffer = new List<byte>(chars.Length);
            for (int i = 0; i < chars.Length; i++)
            {
                if (value[i] == '%')
                {
                    byte decodedChar = (byte)Convert.ToInt32(new string(chars, i + 1, 2), 16);
                    buffer.Add(decodedChar);

                    i += 2;
                }
                else
                {
                    buffer.Add((byte)value[i]);
                }
            }

            return System.Text.Encoding.UTF8.GetString(buffer.ToArray(), 0, buffer.Count);
        }
    }
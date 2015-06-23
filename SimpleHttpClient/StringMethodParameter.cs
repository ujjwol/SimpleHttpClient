﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpClient
{
    internal class StringMethodParameter : IMethodParameter
    {
        private readonly string _name;
        private readonly string _value;

        public StringMethodParameter(string name, long value)
            : this(name, value.ToString())
        { }

        public StringMethodParameter(string name, double value)
            : this(name, value.ToString())
        { }

        public StringMethodParameter(string name, int value)
            : this(name, value.ToString())
        { }

        public StringMethodParameter(string name, bool value)
            : this(name, value.ToString().ToLowerInvariant())
        { }

        public StringMethodParameter(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Parameter name cannot be empty or whitespace.", "name");

            this._name = name;
            this._value = value;
        }

        public string Name { get { return _name; } }

        public string Value { get { return _value; } }

        public HttpContent AsHttpContent()
        {
            var content = new StringContent(_value, Encoding.UTF8);

            content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data");
            content.Headers.ContentDisposition.Name = this.Name;
            content.Headers.ContentLength = null;
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            content.Headers.ContentEncoding.Add("UTF-8");

            return content;
        }

        public bool Equals(IMethodParameter other)
        {
            var p = other as StringMethodParameter;
            if (p == null)
                return false;

            return (this.Name == other.Name && this._value == p._value);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this._value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringMethodParameter);
        }

        public override string ToString()
        {
            return String.Format("{0}={1}", Name, Value);
        }
    }
}

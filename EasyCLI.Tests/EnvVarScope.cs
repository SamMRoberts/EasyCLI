using System;
using System.Collections.Generic;

namespace EasyCLI.Tests
{
    internal sealed class EnvVarScope : IDisposable
    {
        private readonly Dictionary<string, string?> _originals = new();
        private readonly string[] _keys;

        public EnvVarScope(params (string key, string? value)[] vars)
        {
            _keys = new string[vars.Length];
            for (int i = 0; i < vars.Length; i++)
            {
                var (key, value) = vars[i];
                _keys[i] = key;
                _originals[key] = Environment.GetEnvironmentVariable(key);
                Environment.SetEnvironmentVariable(key, value);
            }
        }

        public void Dispose()
        {
            foreach (var key in _keys)
            {
                Environment.SetEnvironmentVariable(key, _originals[key]);
            }
        }
    }
}

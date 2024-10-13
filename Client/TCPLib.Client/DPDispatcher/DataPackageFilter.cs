using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCPLib.Client.DPDispatcher
{
    public class DataPackageFilter
    {
        private string _pattern;
        private ConditionType _conditionType;

        #region builder
        public static DataPackageFilter Equals(string pattern)
        {
            if(string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
           return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.Equals };
        }
        public static DataPackageFilter NotEquals(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.NotEquals };
        }
        public static DataPackageFilter Contains(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.Contains };
        }
        public static DataPackageFilter NotContains(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.NotContains };
        }
        public static DataPackageFilter MatchesRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.MatchesRegex };
        }
        public static DataPackageFilter NotMatchesRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DataPackageFilter() { _pattern = pattern, _conditionType = ConditionType.NotMatchesRegex };
        }
        #endregion
        internal bool Check(string input)
        {
            switch (_conditionType)
            {
                case ConditionType.Equals:
                    return input == _pattern;
                case ConditionType.NotEquals:
                    return input != _pattern;
                case ConditionType.Contains:
                    return input.Contains(_pattern);
                case ConditionType.NotContains:
                    return !input.Contains(_pattern);
                case ConditionType.MatchesRegex:
                    return new Regex(_pattern).IsMatch(input);
                case ConditionType.NotMatchesRegex:
                    return !new Regex(_pattern).IsMatch(input);
            }

            throw new InvalidOperationException("Unknown condition type.");
        }

        private DataPackageFilter() { }
    }
    internal enum ConditionType
    {
        Equals,
        NotEquals,
        Contains,
        NotContains,
        MatchesRegex,
        NotMatchesRegex
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCPLib.Server.DPDispatcher
{
    public class DPFilter
    {
        private string _pattern;
        private ConditionType _conditionType;

        #region builder
        public static DPFilter Equals(string pattern)
        {
            if(string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
           return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.Equals };
        }
        public static DPFilter NotEquals(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.NotEquals };
        }
        public static DPFilter Contains(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.Contains };
        }
        public static DPFilter NotContains(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.NotContains };
        }
        public static DPFilter MatchesRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.MatchesRegex };
        }
        public static DPFilter NotMatchesRegex(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentNullException(nameof(pattern));
            return new DPFilter() { _pattern = pattern, _conditionType = ConditionType.NotMatchesRegex };
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
        #region Operators and overrided methods
        public override bool Equals(object obj)
        {
            return obj is DPFilter filter &&
                   _pattern == filter._pattern &&
                   _conditionType == filter._conditionType;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + (_pattern?.GetHashCode() ?? 0);
            hash = hash * 23 + _conditionType.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            switch (_conditionType)
            {
                case ConditionType.Equals:
                    return $"if type equals {_pattern}";
                case ConditionType.NotEquals:
                    return $"if type not equals {_pattern}";
                case ConditionType.Contains:
                    return $"if type contains {_pattern}";
                case ConditionType.NotContains:
                    return $"if type not contains {_pattern}";
                case ConditionType.MatchesRegex:
                    return $"if type matches regex pattern: {_pattern}";
                case ConditionType.NotMatchesRegex:
                    return $"if type not matches regex pattern: {_pattern}";
            }

            throw new InvalidOperationException("Unknown condition type.");
        }

        private DPFilter() { }
        public static bool operator ==(DPFilter value1, DPFilter value2)
            => value1._pattern == value2._pattern && value1._conditionType == value2._conditionType;
        public static bool operator !=(DPFilter value1, DPFilter value2)
            => (value1 == value2) == false;
        #endregion
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

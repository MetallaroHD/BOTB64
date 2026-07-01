using System.Numerics;

namespace BOTB64.Runtime
{
    public enum ParameterType
    {
        Boolean = 0,
        Integer = 1,
        Float = 2,
        String = 3,
        Vector = 4
    }

    public class Parameter
    {
        public string Name { get; }
        public ParameterType Type { get; private set; }

        private bool _bVal = false;
        private int _iVal = 0;
        private float _fVal = 0f;
        private string _sVal = "";
        private Vector3 _vVal = Vector3.Zero;

        private Parameter(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        public static Parameter FromBool(string name, bool value) => new(name) { Type = ParameterType.Boolean, _bVal = value };
        public static Parameter FromInt(string name, int value) => new(name) { Type = ParameterType.Integer, _iVal = value };
        public static Parameter FromFloat(string name, float value) => new(name) { Type = ParameterType.Float, _fVal = value };
        public static Parameter FromString(string name, string value) => new(name) { Type = ParameterType.String, _sVal = value ?? "" };
        public static Parameter FromVector(string name, Vector3 value) => new(name) { Type = ParameterType.Vector, _vVal = value };

        public bool AsBool() => Expect(ParameterType.Boolean)._bVal;
        public int AsInt() => Expect(ParameterType.Integer)._iVal;
        public float AsFloat() => Expect(ParameterType.Float)._fVal;
        public string AsString() => Expect(ParameterType.String)._sVal;
        public Vector3 AsVector() => Expect(ParameterType.Vector)._vVal;

        public bool GetBool(bool fallback = false) => Type == ParameterType.Boolean ? _bVal : fallback;
        public int GetInt(int fallback = 0) => Type == ParameterType.Integer ? _iVal : fallback;
        public float GetFloat(float fallback = 0f) => Type == ParameterType.Float ? _fVal : fallback;
        public string GetString(string fallback = "") => Type == ParameterType.String ? _sVal : fallback;
        public Vector3 GetVector(Vector3 fallback = default) => Type == ParameterType.Vector ? _vVal : fallback;

        public void Set(bool value) { Type = ParameterType.Boolean; _bVal = value; }
        public void Set(int value) { Type = ParameterType.Integer; _iVal = value; }
        public void Set(float value) { Type = ParameterType.Float; _fVal = value; }
        public void Set(string value) { Type = ParameterType.String; _sVal = value ?? ""; }
        public void Set(Vector3 value) { Type = ParameterType.Vector; _vVal = value; }

        public float ToNumeric() => Type switch
        {
            ParameterType.Integer => _iVal,
            ParameterType.Float => _fVal,
            ParameterType.Boolean => _bVal ? 1f : 0f,
            _ => throw new InvalidCastException($"Parameter '{Name}' ({Type}) cannot be coerced to a number.")
        };

        public string Stringify() => Type switch
        {
            ParameterType.Boolean => _bVal.ToString().ToLower(),
            ParameterType.Integer => _iVal.ToString(),
            ParameterType.Float => _fVal.ToString("G"),
            ParameterType.String => _sVal,
            ParameterType.Vector => $"{_vVal.X},{_vVal.Y},{_vVal.Z}",
            _ => ""
        };

        public void Parse(string raw)
        {
            switch (Type)
            {
                case ParameterType.Boolean:
                    _bVal = raw.Trim().ToLower() is "true" or "1" or "yes";
                    break;
                case ParameterType.Integer:
                    _iVal = int.Parse(raw);
                    break;
                case ParameterType.Float:
                    _fVal = float.Parse(raw, System.Globalization.CultureInfo.InvariantCulture);
                    break;
                case ParameterType.String:
                    _sVal = raw;
                    break;
                case ParameterType.Vector:
                    var parts = raw.Split(',');
                    _vVal = new Vector3(
                        float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                        float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                        float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
                    break;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Parameter other || Type != other.Type) return false;
            return Type switch
            {
                ParameterType.Boolean => _bVal == other._bVal,
                ParameterType.Integer => _iVal == other._iVal,
                ParameterType.Float => MathF.Abs(_fVal - other._fVal) < float.Epsilon,
                ParameterType.String => _sVal == other._sVal,
                ParameterType.Vector => _vVal == other._vVal,
                _ => false
            };
        }

        public override int GetHashCode() => HashCode.Combine(Type, Stringify());

        public override string ToString() => $"[{Name} : {Type}] = {Stringify()}";

        private Parameter Expect(ParameterType expected)
        {
            if (Type != expected)
                throw new InvalidCastException(
                    $"Parameter '{Name}' is {Type}, not {expected}.");
            return this;
        }
    }
}

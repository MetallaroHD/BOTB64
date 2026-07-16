using System.Globalization;
using BOTB64.Editor.ViewModels;
using BOTB64.Runtime;

namespace BOTB64.Editor.Models
{
    // Editable stand-in for BOTB64.Runtime.Parameter. Keeps one value box per
    // type (instead of a discriminated union) so all editors can stay bound
    // at once and only the active one is shown/serialized.
    public class ParameterModel : ObservableObject
    {
        private string _name = "NewParam";
        private ParameterType _type = ParameterType.Integer;
        private bool _boolValue;
        private int _intValue;
        private float _floatValue;
        private string _stringValue = "";
        private float _vecX, _vecY, _vecZ;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public ParameterType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public bool BoolValue
        {
            get => _boolValue;
            set => Set(ref _boolValue, value);
        }

        public int IntValue
        {
            get => _intValue;
            set => Set(ref _intValue, value);
        }

        public float FloatValue
        {
            get => _floatValue;
            set => Set(ref _floatValue, value);
        }

        public string StringValue
        {
            get => _stringValue;
            set => Set(ref _stringValue, value ?? "");
        }

        public float VecX
        {
            get => _vecX;
            set => Set(ref _vecX, value);
        }

        public float VecY
        {
            get => _vecY;
            set => Set(ref _vecY, value);
        }

        public float VecZ
        {
            get => _vecZ;
            set => Set(ref _vecZ, value);
        }

        // Matches Parameter.Stringify() in BOTB64.Runtime
        public string ToRaw()
        {
            return Type switch
            {
                ParameterType.Boolean => BoolValue ? "true" : "false",
                ParameterType.Integer => IntValue.ToString(CultureInfo.InvariantCulture),
                ParameterType.Float => FloatValue.ToString("G", CultureInfo.InvariantCulture),
                ParameterType.String => StringValue,
                ParameterType.Vector => $"{VecX.ToString(CultureInfo.InvariantCulture)},{VecY.ToString(CultureInfo.InvariantCulture)},{VecZ.ToString(CultureInfo.InvariantCulture)}",
                _ => ""
            };
        }

        // Matches Parameter.Parse(raw) in BOTB64.Runtime
        public void ParseRaw(string raw)
        {
            switch (Type)
            {
                case ParameterType.Boolean:
                    BoolValue = raw.Trim().ToLowerInvariant() is "true" or "1" or "yes";
                    break;
                case ParameterType.Integer:
                    IntValue = int.Parse(raw, CultureInfo.InvariantCulture);
                    break;
                case ParameterType.Float:
                    FloatValue = float.Parse(raw, CultureInfo.InvariantCulture);
                    break;
                case ParameterType.String:
                    StringValue = raw;
                    break;
                case ParameterType.Vector:
                    var parts = raw.Split(',');
                    VecX = float.Parse(parts[0], CultureInfo.InvariantCulture);
                    VecY = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    VecZ = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    break;
            }
        }

        public static ParameterModel FromRaw(string name, ParameterType type, string raw)
        {
            var p = new ParameterModel { Name = name, Type = type };
            p.ParseRaw(raw);
            return p;
        }
    }
}

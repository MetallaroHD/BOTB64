using BOTB64.Editor.ViewModels;
using BOTB64.Entities;

namespace BOTB64.Editor.Models
{
    public class EffectModel : ObservableObject
    {
        private string _script = "";
        private EffectTrigger _trigger = EffectTrigger.Direct;
        private EffectSourceType _source = EffectSourceType.Unknown;
        private EffectDamageType _type = EffectDamageType.None;
        private EffectDamageScaling _scaling = EffectDamageScaling.None;

        public string Script
        {
            get => _script;
            set => Set(ref _script, value ?? "");
        }

        public EffectTrigger Trigger
        {
            get => _trigger;
            set => Set(ref _trigger, value);
        }

        public EffectSourceType Source
        {
            get => _source;
            set => Set(ref _source, value);
        }

        public EffectDamageType Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        public EffectDamageScaling Scaling
        {
            get => _scaling;
            set => Set(ref _scaling, value);
        }

        // Matches the "script | trigger | source | type | scaling" line format
        public string ToLine()
        {
            return $"{Script} | {(int)Trigger} | {(int)Source} | {(int)Type} | {(int)Scaling}";
        }

        public static EffectModel FromParts(string script, int trigger, int source, int type, int scaling)
        {
            return new EffectModel
            {
                Script = script,
                Trigger = (EffectTrigger)trigger,
                Source = (EffectSourceType)source,
                Type = (EffectDamageType)type,
                Scaling = (EffectDamageScaling)scaling
            };
        }
    }
}

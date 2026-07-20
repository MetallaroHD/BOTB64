using BOTB64.Runtime;

namespace BOTB64.Entities
{
    public class CharacterStat
    {
        private float BaseVal = 0f;
        private float Mod_Add = 0f;
        private float Mod_Mul = 1f;

        public CharacterStat(float val)
        {
            BaseVal = val;
        }

        public void Set(float val)
        {
            BaseVal = val;
        }

        public float GetF()
        {
            return (BaseVal + Mod_Add) * Mod_Mul;
        }

        public int GetI()
        {
            return (int)GetF();
        }

        public void Add(float val)
        {
            if (val > 0)
                Mod_Add += val;
            else
                Mod_Add = Math.Max(0, Mod_Add + val);
        }

        public void Mul(float val)
        {
            if (val > 0)
                Mod_Mul += val;
            else
                Mod_Mul = Math.Max(0, Mod_Add + val);
        }

        public void Reset()
        {
            Mod_Add = 0f;
            Mod_Mul = 1f;
        }
    }
}

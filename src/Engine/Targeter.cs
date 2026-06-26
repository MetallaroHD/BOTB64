using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine
{
    public enum TargetingType
    {
        None = 0,
        Direct = 1,
        BeamNoLos = 2, //requires source
        Area = 3, //requires radius
    }

    public class TargetingData
    {
        public TargetingType Type;
        public Hex? Source;
        public int Radius;
        public bool Secret;
    }

    public static class Targeter
    {
        public static TargetingData Data { get; set; } = new TargetingData { Type = TargetingType.None, Source = null, Radius = 0 };
        public static Board? Board { get; set; } = null;
        public static List<Tile> Targeted = new();

        public static void SetBoard(Board board)
        {
            Board = board;
        }

        public static void SetTargetingData(TargetingData data)
        {
            Data = data;
        }

        private static void SetHighlightStatus(bool enabled)
        {
            foreach (Tile tile in Targeted)
                tile.Highlighted = enabled;
        }

        public static void Reset()
        {
            Targeted.Clear();
        }

        public static void UpdateTarget(Hex pickedPoint)
        {
            SetHighlightStatus(false);
            Reset();
            switch (Data.Type)
            {
                case TargetingType.None:
                    break;
                case TargetingType.Direct:
                    Targeted.Add(Board?.GetTile(pickedPoint) ?? throw new InvalidOperationException("Board is not set."));
                    break;
                case TargetingType.BeamNoLos:
                    TargetBeam(pickedPoint, false);
                    break;

                default:
                    break;
            }

            if(!Data.Secret)
                SetHighlightStatus(true);
        }

        public static void TargetBeam(Hex dst, bool lineOfSight)
        {
            if (Data.Source == null || Board == null)
                return;

            var line = HexAlgo.Beam(Data.Source.Value, dst);

            if (!lineOfSight)
            {
                Targeted = Board.GetTiles(line);
                return;
            }

            foreach (var h in line)
            {
                if (!Board.IsPassable(h))
                    break;

                Tile? tile = Board.GetTile(h);
                if (tile == null)
                    break;
                Targeted.Add(tile);
            }
        }
    }
}

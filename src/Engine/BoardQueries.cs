using BOTB64.Entities;
using BOTB64.Runtime;

namespace BOTB64.Engine
{
    public static class BoardQueries
    {
        public static List<Tile> Beam(Board board, Hex src, Hex dst, bool lineOfSight)
        {
            var line = HexAlgo.Beam(src,dst);

            if (!lineOfSight)
                return board.GetTiles(line);

            List<Tile> result = new();

            foreach (var h in line)
            {
                if(!board.IsPassable(h)) 
                    break;

                Tile? tile = board.GetTile(h);
                if (tile == null)
                    break;
                result.Add(tile);
            }

            return result;
        }

    }
}

using BOTB64.Entities;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace BOTB64.Runtime
{
    public class LevelDataFile : BOTBDatafile<Level>
    {
        protected new const string HeaderTag = ":BOTB64LEVEL";

        public override Level Read(DataFile df)
        {
            string[] lines = df.ReadLines();
            int i = SkipBlankAndComments(lines, 0);

            if (i >= lines.Length || !TryParseHeader(lines[i]))
                throw new FormatException($"'{(string)df}' is missing a valid header.");
            i++;

            var board = new Board();
            var level = new Level
            {
                LevelBoard = board
            };

            while (i < lines.Length)
            {
                i = SkipBlankAndComments(lines, i);

                if (i >= lines.Length) 
                    break;

                string line = lines[i].Trim();

                if (line.StartsWith(":CENTER"))
                {
                    board.Center = ParseCenter(line);
                    i++;
                }
                else if (line.StartsWith(":MAP"))
                {
                    i++;
                    i = ParseMap(lines, i, board);
                }
                else if (line.StartsWith(":BLUESPAWNS"))
                {
                    i++;
                    i = ParseSpawns(lines, i, board, SpawnType.Blue);
                }
                else if (line.StartsWith(":REDSPAWNS"))
                {
                    i++;
                    i = ParseSpawns(lines, i, board, SpawnType.Red);
                }
                else
                {
                    i++;
                }
            }

            return level;
        }

        public override bool TryRead(DataFile df, out Level data)
        {
            data = null;

            if (!df.Exists())
                return false;

            try 
            {
                data = Read(df);
                return true;
            }
            catch(FormatException)
            {
                return false;
            }
        }

        // at some point I may make an editor
        public override void Write(DataFile file, Level data)
        {
            Board board = data.LevelBoard;
            var sb = new StringBuilder();

            sb.Append(HeaderTag).Append(' ').Append(data.Name).Append('\n');
            sb.Append('\n');

            sb.Append(":CENTER ")
              .Append(board.Center.X.ToString(CultureInfo.InvariantCulture)).Append(' ')
              .Append(board.Center.Y.ToString(CultureInfo.InvariantCulture))
              .Append('\n');

            sb.Append('\n');
            sb.Append(":MAP\n");
            foreach (List<Tile> row in board.Tiles)
            {
                foreach (Tile tile in row)
                    sb.Append((int)tile.Type);
                sb.Append('\n');
            }

            File.WriteAllText((string)file, sb.ToString());
        }

        private static Vector2 ParseCenter(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return new Vector2(x, y);
        }

        private static int ParseSpawns(string[] lines, int i, Board board, SpawnType type)
        {
            var rawRows = new List<string>();
            int start = i;

            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                string[] parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int q = int.Parse(parts[0], CultureInfo.InvariantCulture);
                int r = int.Parse(parts[1], CultureInfo.InvariantCulture);
                Hex position = new Hex(q, r);
                if(type == SpawnType.Blue)
                    board.BlueSpawns.Add(new SpawnPoint { Type = type, Position = position });
                else if(type == SpawnType.Red)
                    board.RedSpawns.Add(new SpawnPoint { Type = type, Position = position });
                i++;
            }
            return i;
        }

        private static int ParseMap(string[] lines, int i, Board board)
        {
            var rawRows = new List<string>();
            int start = i;

            while (i < lines.Length)
            {
                string trimmed = lines[i].Trim();

                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    i++;
                    continue;
                }

                if (trimmed.StartsWith(":", StringComparison.Ordinal))
                    break;

                rawRows.Add(trimmed);
                i++;
            }

            int totalRows = rawRows.Count;
            int totalCols = rawRows.Count > 0 ? rawRows[0].Length : 0;

            int rOffset = totalRows / 2;
            int qOffset = totalCols / 2;

            for (int ri = 0; ri < rawRows.Count; ri++)
            {
                string trimmed = rawRows[ri];
                int r = ri - rOffset;

                var tileRow = new List<Tile>(trimmed.Length);
                for (int qi = 0; qi < trimmed.Length; qi++)
                {
                    int q = qi - qOffset;
                    var type = (TileType)(trimmed[qi] - '0');
                    tileRow.Add(board.CreateTile(new Hex(q, r), type));
                }

                board.Tiles.Add(tileRow);
            }

            return i;
        }
    }
}

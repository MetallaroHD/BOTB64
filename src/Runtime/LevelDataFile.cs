using BOTB64.Entities;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace BOTB64.Runtime
{
    public class LevelDataFile : IDataFileRW<Level>
    {
        private const string HeaderTag = ":BOTB64LEVEL";

        public Level Read(DataFile df)
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
                else
                {
                    i++;
                }
            }

            return level;
        }

        public bool TryRead(DataFile df, out Level data)
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

        public void Write(DataFile file, Level data)
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

        private static bool TryParseHeader(string line)
        {
            if (!line.StartsWith(HeaderTag, StringComparison.Ordinal))
                return false;

            return true;
        }

        private static int SkipBlankAndComments(string[] lines, int i)
        {
            while (i < lines.Length)
            {
                string t = lines[i].Trim();
                if (t.Length > 0 && !t.StartsWith("#", StringComparison.Ordinal))
                    break;
                i++;
            }

            return i;
        }

        private static Vector2 ParseCenter(string line)
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
            return new Vector2(x, y);
        }

        private static int ParseMap(string[] lines, int i, Board board)
        {
            // First pass: collect raw rows
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

            // Second pass: build tiles with centered coordinates
            int totalRows = rawRows.Count;
            int totalCols = rawRows.Count > 0 ? rawRows[0].Length : 0;

            int rOffset = totalRows / 2;  // e.g. 29 for a 59-row matrix
            int qOffset = totalCols / 2;  // e.g. 14 for a 29-col matrix

            for (int ri = 0; ri < rawRows.Count; ri++)
            {
                string trimmed = rawRows[ri];
                int r = ri - rOffset;

                var tileRow = new List<Tile>(trimmed.Length);
                for (int qi = 0; qi < trimmed.Length; qi++)
                {
                    int q = qi - qOffset;
                    var type = (TileType)(trimmed[qi] - '0');
                    tileRow.Add(board.CreateTile(q, r, type));
                }

                board.Tiles.Add(tileRow);
            }

            return i;
        }
    }
}

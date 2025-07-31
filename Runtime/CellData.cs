using UnityEngine;

namespace XIV.GridSystems
{
    public struct CellData
    {
        public static readonly CellData InvalidCell = new CellData(-1, -1, -1, Vector3.zero, Vector3.zero);

        public int index;
        public int x;
        public int y;
        public Vector3 worldPos;
        public Vector3 cellSize;

        public CellData(int index, int x, int y, Vector3 worldPos, Vector3 cellSize)
        {
            this.index = index;
            this.x = x;
            this.y = y;
            this.worldPos = worldPos;
            this.cellSize = cellSize;
        }
    }
}
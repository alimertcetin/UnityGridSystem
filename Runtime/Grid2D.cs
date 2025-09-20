using System.Collections.Generic;
using UnityEngine;

namespace XIV.GridSystems
{
    public class Grid2D : IGrid
    {
        public Vector3 GridCenter
        {
            get => gridCenter;
            set
            {
                isDirty = gridCenter != value || isDirty;
                gridCenter = value;
            }
        }
        
        public Vector2 AreaSize
        {
            get => areaSize;
            set
            {
                isDirty = areaSize != value || isDirty;
                areaSize = value;
            }
        }
        
        public Vector2Int CellCount
        {
            get => cellCount;
            set
            {
                isDirty = cellCount != value || isDirty;
                cellCount = value;
            }
        }
        
        public Quaternion Orientation
        {
            get => orientation;
            set
            {
                isDirty = orientation != value || isDirty;
                orientation = value;
            }
        }
        
        public bool IsDirty => isDirty;

        Vector3 gridCenter;
        Vector2 areaSize;
        Vector2Int cellCount;
        Quaternion orientation;
        bool isDirty;

        readonly List<CellData> cellDatas;
        readonly List<IGridListener> gridListeners;
        Vector2 CellSize => new Vector2(areaSize.x / cellCount.x, areaSize.y / cellCount.y);
        static readonly List<int> neighbourIndicesBuffer = new List<int>(8);

        public Grid2D(Vector3 gridCenter, Vector2 areaSize, Vector2Int cellCount, Quaternion orientation)
        {
            this.gridCenter = gridCenter;
            this.areaSize = areaSize;
            this.cellCount = cellCount;
            this.orientation = orientation;
            int length = cellCount.x * cellCount.y;
            this.cellDatas = new List<CellData>(length);
            this.gridListeners = new List<IGridListener>(2);
            CreateCells();
        }

        void CreateCells()
        {
            CreateCellsNonAlloc(gridCenter, areaSize, cellCount, orientation, cellDatas);
        }

        public void RebuildIfDirty()
        {
            if (isDirty == false) return;

            CreateCells();
            InformListeners();
            isDirty = false;
        }

        void InformListeners()
        {
            var count = gridListeners.Count;
            for (int i = 0; i < count; i++)
            {
                gridListeners[i].OnGridChanged(this);
            }
        }

        public void AddListener(IGridListener listener)
        {
            if (gridListeners.Contains(listener)) return;
            gridListeners.Add(listener);
        }

        public void RemoveListener(IGridListener listener)
        {
            gridListeners.Remove(listener);
        }

        public int GetIndexByWorldPos(Vector3 worldPos)
        {
            var localPos = Quaternion.Inverse(orientation) * (worldPos - gridCenter);

            localPos.x = Mathf.Clamp(localPos.x, -areaSize.x / 2f + 0.01f, areaSize.x / 2f - 0.01f);
            localPos.y = Mathf.Clamp(localPos.y, -areaSize.y / 2f + 0.01f, areaSize.y / 2f - 0.01f);

            float normalizedX = (localPos.x + areaSize.x * 0.5f) / areaSize.x;
            float normalizedY = (localPos.y + areaSize.y * 0.5f) / areaSize.y;

            int x = Mathf.FloorToInt(normalizedX * cellCount.x);
            int y = Mathf.FloorToInt(normalizedY * cellCount.y);

            int index = x * cellCount.y + y;
            return Mathf.Clamp(index, 0, cellDatas.Count - 1);
        }
        
        public IReadOnlyList<int> GetNeighbourIndices(int centerIndex)
        {
            GetNeighbourIndicesNonAlloc(centerIndex, cellCount, neighbourIndicesBuffer);
            return neighbourIndicesBuffer.AsReadOnly();
        }

        public static void GetNeighbourIndicesNonAlloc(int centerIndex, Vector2Int cellCount, IList<int> neighbourIndicesBuffer)
        {
            neighbourIndicesBuffer.Clear();
            int x = centerIndex / cellCount.y;
            int y = centerIndex % cellCount.y;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    int neighbourX = x + i;
                    int neighbourY = y + j;

                    if (neighbourX < 0 || neighbourX >= cellCount.x || neighbourY < 0 || neighbourY >= cellCount.y) continue;

                    int neighborIndex = neighbourX * cellCount.y + neighbourY;
                    neighbourIndicesBuffer.Add(neighborIndex);
                }
            }
        }

        public IReadOnlyList<int> GetNeighbourIndices(Vector3 worldPos)
        {
            int centerIndex = GetIndexByWorldPos(worldPos);
            return GetNeighbourIndices(centerIndex);
        }

        public IList<CellData> GetCells() => cellDatas;

        public static void CreateCellsNonAlloc(Vector3 gridCenter, Vector2 areaSize, Vector2Int cellCount, Quaternion orientation, List<CellData> buffer)
        {
            int count = buffer.Count;
            int length = cellCount.y * cellCount.x;
            int diff = length - count;
            for (int i = 0; i < diff; i++)
            {
                buffer.Add(default);
            }

            if (diff < 0)
            {
                diff = -diff;
                buffer.RemoveRange(count - diff - 1, diff);
            }
            var cellSize = new Vector3(areaSize.x / cellCount.x, areaSize.y / cellCount.y);
            var halfArea = new Vector2(areaSize.x * 0.5f, areaSize.y * 0.5f);
            var originOffset = new Vector3(-halfArea.x + cellSize.x * 0.5f, -halfArea.y + cellSize.y * 0.5f, 0f);

            for (int x = 0; x < cellCount.x; x++)
            {
                for (int y = 0; y < cellCount.y; y++)
                {
                    var localPos = new Vector3(cellSize.x * x, cellSize.y * y, 0f) + originOffset;
                    var worldPos = gridCenter + orientation * localPos;
                    int index = x * cellCount.y + y;
                    buffer[index] = new CellData(index, x, y, worldPos, cellSize);
                }
            }
        }
    }
}
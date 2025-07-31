using System.Collections.Generic;
using UnityEngine;

namespace XIV.GridSystems
{
    public interface IGrid
    {
        Vector3 GridCenter { get; }
        Vector2 AreaSize { get; }
        Vector2Int CellCount { get; }

        IList<CellData> GetCells();
    }
}
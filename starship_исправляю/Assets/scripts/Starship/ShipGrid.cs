using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ShipGrid : NetworkTransform
{
    public float cellSize = 1f; // Размер одной клетки в метрах Unity

    [Header("Стартовые настройки")]
    public ShipModule corePrefab; // Префаб ядра корабля

    // Хранилище всех модулей. Ключ — координата клетки, значение — модуль в ней.
    private Dictionary<Vector2Int, ShipModule> occupiedCells = new Dictionary<Vector2Int, ShipModule>();

    private void Start()
    {
        // При старте игры автоматически создаем ядро в центре сетки (0, 0)
        if (corePrefab != null)
        {
            PlaceModule(new Vector2Int(0, 0), corePrefab, corePrefab.size, 0f);
        }
        else
        {
            Debug.LogError("Вы забыли назначить Core Prefab в скрипте ShipGrid!");
        }
    }

    // Перевод мировых координат мыши в локальные координаты сетки корабля
    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);

        int x = Mathf.FloorToInt((localPos.x + cellSize / 2f) / cellSize);
        int y = Mathf.FloorToInt((localPos.y + cellSize / 2f) / cellSize);

        return new Vector2Int(x, y);
    }

    // «Умный» перевод координат сетки в локальные координаты Unity с учетом четности блока
    public Vector3 GridToLocal(Vector2Int gridPos, Vector2Int blockSize)
    {
        float posX = gridPos.x * cellSize;
        float posY = gridPos.y * cellSize;

        // Сдвиг по X: если ширина четная (2, 4), добавляем половину ячейки, чтобы Pivot не ломал сетку
        if (blockSize.x % 2 == 0) posX += cellSize / 2f;

        // Сдвиг по Y: если высота четная, добавляем половину ячейки
        if (blockSize.y % 2 == 0) posY += cellSize / 2f;

        return new Vector3(posX, posY, 0f);
    }

    // Проверка: можно ли установить модуль определенного размера в эти координаты?
    public bool CanPlaceModule(Vector2Int startPos, Vector2Int size)
    {
        // 1. Проверяем, свободны ли все нужные клетки
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int cellToCheck = startPos + new Vector2Int(x, y);
                if (occupiedCells.ContainsKey(cellToCheck))
                {
                    return false; // Клетка уже занята!
                }
            }
        }

        // 2. Новый блок обязан касаться старых блоков (вокруг ядра)
        if (occupiedCells.Count > 0)
        {
            if (!HasNeighbor(startPos, size)) return false;
        }

        return true;
    }

    // Проверка соседства (касается ли новый блок хотя бы одного старого)
    private bool HasNeighbor(Vector2Int startPos, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int currentCell = startPos + new Vector2Int(x, y);

                Vector2Int[] neighbors = {
                    currentCell + Vector2Int.up,
                    currentCell + Vector2Int.down,
                    currentCell + Vector2Int.left,
                    currentCell + Vector2Int.right
                };

                foreach (var neighbor in neighbors)
                {
                    if (occupiedCells.ContainsKey(neighbor)) return true;
                }
            }
        }
        return false;
    }

    // Сам процесс установки модуля в сетку (с учетом его разворота и реального размера)
    public void PlaceModule(Vector2Int startPos, ShipModule modulePrefab, Vector2Int actualSize, float rotationAngle)
    {
        if (!CanPlaceModule(startPos, actualSize)) return;

        // Создаем модуль как дочерний объект корабля
        ShipModule spawnedModule = Instantiate(modulePrefab, transform);

        // ТУТ ИСПРАВЛЕНО (строка 120): передаем actualSize, чтобы Unity знала, как сдвинуть четный блок
        spawnedModule.transform.localPosition = GridToLocal(startPos, actualSize);

        // Применяем угол поворота
        spawnedModule.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
        spawnedModule.gridPosition = startPos;

        // Записываем этот модуль во все клетки словаря, которые он занял по факту разворота
        for (int x = 0; x < actualSize.x; x++)
        {
            for (int y = 0; y < actualSize.y; y++)
            {
                Vector2Int cell = startPos + new Vector2Int(x, y);
                occupiedCells.Add(cell, spawnedModule);
            }
        }
    }
}

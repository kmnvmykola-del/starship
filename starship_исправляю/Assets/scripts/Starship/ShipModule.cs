using Unity.Netcode.Components;
using UnityEngine;

public class ShipModule : NetworkTransform
{
    [Header("Настройки модуля")]
    public string moduleName;
    public int cost = 100;
    public int health = 100; public int maxHealth = 100;
    // Сколько клеток занимает модуль по X и Y (для больших пушек 2x2 или 3x2)
    public Vector2Int size = new Vector2Int(2, 2);

    // Координата левого нижнего угла модуля на сетке корабля
    [HideInInspector] public Vector2Int gridPosition;
}

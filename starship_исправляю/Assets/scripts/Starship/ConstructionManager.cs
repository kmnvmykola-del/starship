using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem; // Используем новый Input System

public class ConstructionManager : NetworkTransform
{
    [Header("Ссылки")]
    public ShipGrid targetShip;          // На какой корабль строим
    public ShipModule selectedPrefab;    // Какой модуль сейчас выбран для постройки
    public SpriteRenderer previewRender; // Визуальный "призрак" модуля на курсоре

    private int currentRotation = 0;     // 0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°

    void Start()
    {
        if (selectedPrefab == null) previewRender.gameObject.SetActive(false);
    }

    void Update()
    {
        if(!IsOwner) return;
        if (selectedPrefab == null) return;

        // 1. Поворот блока при нажатии на клавишу R
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            currentRotation = (currentRotation + 1) % 4;

            float angle = currentRotation * -90f; // Поворот по часовой стрелке
            previewRender.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        // 2. Получаем позицию мыши в мире
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
        mouseWorldPos.z = 0f;

        // 3. Переводим координаты мыши в сетку корабля
        Vector2Int currentGridPos = targetShip.WorldToGrid(mouseWorldPos);

        // 4. Двигаем полупрозрачное превью (призрака) с учетом сдвига четности
        previewRender.gameObject.SetActive(true);
        Vector2Int currentSize = GetRotatedSize();

        // ТУТ ИСПРАВЛЕНО (строка 29): передаем currentSize в метод GridToLocal
        Vector3 localGridPos = targetShip.GridToLocal(currentGridPos, currentSize);

        previewRender.transform.position = targetShip.transform.TransformPoint(localGridPos);

        // 5. Проверяем правила и подсвечиваем превью цветом
        if (targetShip.CanPlaceModule(currentGridPos, currentSize))
        {
            previewRender.color = new Color(0f, 1f, 0f, 0.5f); // Зеленый — можно строить

            // Если игрок кликнул ЛКМ — строим блок, передавая угол и размер
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                float finalAngle = currentRotation * -90f;
                targetShip.PlaceModule(currentGridPos, selectedPrefab, currentSize, finalAngle);
            }
        }
        else
        {
            previewRender.color = new Color(1f, 0f, 0f, 0.5f); // Красный — строить нельзя
        }
    }

    // Метод, который высчитывает математический размер блока с учетом его текущего поворота
    public Vector2Int GetRotatedSize()
    {
        if (selectedPrefab == null) return Vector2Int.one;

        // Если блок повернут боком (на 90 или 270 градусов), меняем X и Y местами в математике
        if (currentRotation == 1 || currentRotation == 3)
        {
            return new Vector2Int(selectedPrefab.size.y, selectedPrefab.size.x);
        }

        return selectedPrefab.size;
    }

    // Метод для кнопок интерфейса: выбрать другой блок для строительства
    public void ChangeSelectedModule(ShipModule newModule)
    {
        selectedPrefab = newModule;
        currentRotation = 0; // Сбрасываем поворот при выборе нового блока
        previewRender.transform.localRotation = Quaternion.identity;

        previewRender.GetComponent<SpriteRenderer>().sprite = newModule.GetComponent<SpriteRenderer>().sprite;
        previewRender.transform.localScale = newModule.transform.localScale;
    }
}

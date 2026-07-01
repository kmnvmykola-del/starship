using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2D : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;

    private void Update()
    {
        // ГЛАВНЫЙ ФИЛЬТР: Если этот персонаж принадлежит не мне — выходим.
        // Без этого условия вы бы управляли всеми игроками на сцене одновременно!
        if (!IsOwner) return;

        // Проверяем наличие клавиатуры
        if (Keyboard.current == null) return;

        float forwardInput = 0f;
        float rotationInput = 0f;

        // 1. Сбор ввода с клавиатуры
        if (Keyboard.current.wKey.isPressed) forwardInput += 1f;
        if (Keyboard.current.sKey.isPressed) forwardInput -= 1f;
        if (Keyboard.current.aKey.isPressed) rotationInput += 1f;
        if (Keyboard.current.dKey.isPressed) rotationInput -= 1f;

        // 2. Локальный поворот (выполняется прямо на ПК игрока)
        if (rotationInput != 0f)
        {
            float rotationAmount = rotationInput * rotationSpeed * Time.deltaTime;
            transform.Rotate(0f, 0f, rotationAmount);
        }

        // 3. Локальное перемещение вперед/назад относительно текущего поворота
        if (forwardInput != 0f)
        {
            Vector3 movement = transform.up * forwardInput * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }
}
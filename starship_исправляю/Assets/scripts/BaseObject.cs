using UnityEngine;

// Мы подписали контракт с IInteractable, значит ОБЯЗАНЫ иметь метод Interact
public class BaseObject : MonoBehaviour, IInteractable
{
    // У кожної бази в інспекторі тут буде свій унікальний Canvas
    public Canvas myCanvas;

    // ИСПРАВЛЕНО: Переименовали метод из OpenInterface в Interact
    public void Interact(DistanceCheck player)
    {
        if (myCanvas != null)
        {
            myCanvas.gameObject.SetActive(true);
            Debug.Log($"Інтерфейс бази {gameObject.name} відкрито!");

            // Отключаем корабль напрямую через переданную ссылку
            if (player != null)
            {
                player.DeactivateStarship();
            }
        }
        else
        {
            Debug.LogError($"Канвас не назначен в инспекторе на объекте {gameObject.name}!");
        }
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class DistanceCheck : MonoBehaviour
{
    // LayerMask успешно удален!
    public float distancedetection = 5f;

    public void Update()
    {
        CheckIfObjectSelected();
    }

    private void CheckIfObjectSelected()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Теперь проверяем абсолютно все 2D коллайдеры в этой точке
            Collider2D hit = Physics2D.OverlapPoint(mousePosition);

            if (hit != null)
            {
                // Просто ищем интерфейс. Если он есть — работаем, если нет — игнорируем объект
                IInteractable interactableObject = hit.GetComponent<IInteractable>();

                if (interactableObject != null)
                {
                    CheckDistance(interactableObject, hit.transform);
                }
            }
        }
    }

    private void CheckDistance(IInteractable target, Transform targetTransform)
    {
        float distance = Vector3.Distance(transform.position, targetTransform.position);

        if (distance <= distancedetection)
        {
            target.Interact(this);
        }
        else
        {
            Debug.Log($"Объект занадто далеко! Відстань: {distance}м");
        }
    }

    public void DeactivateStarship() => gameObject.SetActive(false);
    public void ActivateStarship() => gameObject.SetActive(true);
}

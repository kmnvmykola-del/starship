public interface IInteractable
{
    // Любой объект, который хочет, чтобы на него кликали, должен иметь этот метод
    void Interact(DistanceCheck player);
}

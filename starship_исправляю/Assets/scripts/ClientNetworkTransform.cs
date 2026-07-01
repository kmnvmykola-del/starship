using Unity.Netcode.Components;
using UnityEngine; 

// Этот атрибут запрещает вешать два таких компонента на один объект  
[DisallowMultipleComponent]
public class ClientNetworkTransform : NetworkTransform
{
    // Это встроенный метод Unity Netcode. Он спрашивает: "Авторизован ли сервер?"  
    protected override bool OnIsServerAuthoritative()
    {
        // Возвращая false, мы говорим: "Нет, сервер не главный. Главный — клиент-владелец!"  
        return false;
    }
}
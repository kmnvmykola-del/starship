using UnityEngine;
using Unity.Netcode;

public class NetcodeFix : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}

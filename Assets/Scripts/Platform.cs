using UnityEngine;

public class Platform : MonoBehaviour 
{
    public void GetOnPlatform(Transform player)
    {
        player.SetParent(transform);
    }
    public void GetOffPlatform(Transform player)
    {
        player.SetParent(null);
    }
}

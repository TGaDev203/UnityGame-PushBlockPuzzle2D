using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private bool hasStarted;

    private void Update()
    {
        hasStarted = false;
    }

    public bool HasStarted() => hasStarted;
}

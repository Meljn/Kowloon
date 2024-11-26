using UnityEngine;

public class ParkourController : MonoBehaviour
{
    EnviromentScanner enviromentScanner;

    private void Awake()
    {
        enviromentScanner = GetComponent<EnviromentScanner>();
    }

    private void Update()
    {
        var hitData = enviromentScanner.ObstacleCheck();
        if (hitData.forwardHitFound)
        {
            Debug.Log("Obstacle found " + hitData.forwardHit.transform.name);
        }
    }
}

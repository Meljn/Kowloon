using System.Collections;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    bool inAction;

    EnviromentScanner enviromentScanner;
    Animator animator;
    Controller controller;

    private void Awake()
    {
        enviromentScanner = GetComponent<EnviromentScanner>();
        animator = GetComponent<Animator>();
        controller = GetComponent<Controller>();
    }

    private void Update()
    {
        if (!inAction)
        {
            var hitData = enviromentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                StartCoroutine(DoParkourAction());
            }
        }
    }

    IEnumerator DoParkourAction()
    {
        inAction = true;
        controller.SetControl(false);

        animator.CrossFade("StepUp", 0.2f);
     

        var animState = animator.GetNextAnimatorStateInfo(0);

        yield return new WaitForSeconds(animState.length);

        controller.SetControl(true);
        inAction = false;
    }
}

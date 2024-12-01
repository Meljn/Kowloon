using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;

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
        if (Input.GetButton("Jump") && !inAction)
        {
            var hitData = enviromentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        controller.SetControl(false);

        animator.CrossFade(action.AnimName, 0.2f);
     

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (animState.IsName(action.AnimName))
            Debug.Log("The parkour animation is wrong");

        yield return new WaitForSeconds(animState.length - 0.1f);

        controller.SetControl(true);
        inAction = false;
    }
}

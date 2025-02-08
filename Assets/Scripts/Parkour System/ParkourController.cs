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

        animator.CrossFade(action.AnimName, 0.1f);
        yield return null;
        

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.AnimName))
            Debug.Log("The parkour animation is wrong"); 


        float timer = 0f;
        while (timer <= 0.8f)
        {
            timer += Time.deltaTime;

            if (action.RotateToObstacle)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, controller.RotationSpeed * Time.deltaTime);

            if (action.EnableTargetMatching)
                MatchTarget(action);

            yield return null;
        }


        controller.SetControl(true);
        inAction = false;
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, new MatchTargetWeightMask(new Vector3(0, 1, 1), 0), action.MatchStartTime, action.MatchTargetTime);
    }
}

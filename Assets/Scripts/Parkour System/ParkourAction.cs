using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    [SerializeField] string animName;

    [SerializeField] float minHeight;
    [SerializeField] float maxHeight;

    [SerializeField] bool rotateToObstacle;

    [Header("Target Matching")]
    [SerializeField] bool enableTargetMatching = true;
    [SerializeField] AvatarTarget matchBodyPart;
    [SerializeField] float matchStartTime;
    [SerializeField] float matchTargetTime;

    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPos { get; set; }

    public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
    {
        float height = hitData.heightHit.point.y - player.position.y;
        if (height < minHeight ||  height > maxHeight)
            return false;

        if (rotateToObstacle)
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

        if (enableTargetMatching)
            MatchPos = hitData.heightHit.point;

        return true;
    }

    public string AnimName => animName;
    public bool RotateToObstacle => rotateToObstacle;

    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;

}

using UnityEngine;
using DG.Tweening;

public class Follower : MonoBehaviour
{
    public Transform TargetedBolt;
    public Sequence TargetedSequence;
    void Update()
    {
        if(TargetedSequence != null && TargetedSequence.IsPlaying())
        {
            return;
        }
        if (TargetedBolt == null)
        {
            return;
        }
        TargetedBolt.position = transform.position;
    }
}

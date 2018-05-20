using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sense
{
    Seeing,
    Hearing
}
public class Senses {

    public float visionRangeX;
    public float visionRangeY;
    public float hearingRangeX;
    public float hearingRangeY;

    public float reactionTime;

    public GameUnit owner;

	public Senses(float visionRangeXVal, float visionRangeYVal, float hearingRangeXVal, float hearingRangeYVal, float reactionTimeVal)
    {
        this.visionRangeX = visionRangeXVal;
        this.visionRangeY = visionRangeYVal;
        this.hearingRangeX = hearingRangeXVal;
        this.hearingRangeY = hearingRangeYVal;
        this.reactionTime = reactionTimeVal;
    }
    public bool CanSee(GameUnit target)
    {
        return SeeOrHear(Sense.Seeing, target);
    }
    public bool CanHear(GameUnit target)
    {
        return SeeOrHear(Sense.Hearing, target);
    }
    protected bool SeeOrHear(Sense sens, GameUnit target)
    {
        Transform selfBody = owner.body;
        Transform targetBody = target.body;

        float modifierX = visionRangeX;
        float modifierY = visionRangeY;

        if (sens == Sense.Hearing)
        {
            modifierX = hearingRangeX;
            modifierY = hearingRangeY;
        }

        if (
                (
                    (selfBody.forward.x < 0 || sens == Sense.Hearing)
                    && targetBody.transform.position.x < selfBody.transform.position.x
                    && targetBody.transform.position.x + modifierX > selfBody.position.x
                )
                ||
                (
                    (selfBody.forward.x > 0 || sens == Sense.Hearing)
                    && targetBody.transform.position.x > selfBody.transform.position.x
                    && targetBody.transform.position.x - modifierX < selfBody.position.x
                )
            )
        {
            if (
                (targetBody.transform.position.y < selfBody.transform.position.y
                    && targetBody.transform.position.y + modifierY > selfBody.position.y
                )
                ||
                (
                    targetBody.transform.position.y > selfBody.transform.position.y
                    && targetBody.transform.position.y - modifierY < selfBody.position.y
                )
            )
            {
                if (sens == Sense.Seeing)
                {
                    Vector3 fromPosition = selfBody.position;
                    Vector3 toPosition = targetBody.position;
                    Vector3 direction = toPosition - fromPosition;
                    RaycastHit hit;
                    // Casts a ray against colliders in the scene
                    if (Physics.Raycast(fromPosition, direction, out hit))
                    {
                        ColliderOwner co = hit.transform.gameObject.GetComponent<ColliderOwner>();
                        if(co != null)
                        {
                            if(co.owner == target)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }
    public Senses Clone()
    {
        return new Senses(visionRangeX, visionRangeY, hearingRangeX, hearingRangeY,reactionTime);
    }
}

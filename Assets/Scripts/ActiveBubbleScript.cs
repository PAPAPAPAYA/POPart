using UnityEngine;

public class ActiveBubbleScript : BubbleScript
{
    // Override the Start method to set the bubble as active and poppable
    protected override void Start()
    {
        base.Start(); // Call the base class Start method

        // Set the bubble as active and poppable
        active = true;
        bubbleImg.GetComponent<SpriteRenderer>().color = ogColor;
        if (BubbleMakerScript.me.inactiveBubbles.Contains(gameObject.transform.parent.gameObject))
        {
            BubbleMakerScript.me.inactiveBubbles.Remove(gameObject.transform.parent.gameObject);
        }
    }
}
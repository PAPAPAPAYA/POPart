using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PumpThoseFuckers : MonoBehaviour
{
    public float delay = 0.5f; // Delay between setting each bubble's pumping bool
    private int burstCount = 0; // Counter for burst bubbles

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetPumpingWithDelay());
    }

    private IEnumerator SetPumpingWithDelay()
    {
        // Find all bubbles in the scene
        BubbleScript[] bubbles = FindObjectsOfType<BubbleScript>();

        // Sort the bubbles by their parent's name
        System.Array.Sort(bubbles, (bubble1, bubble2) => 
        {
            string parentName1 = bubble1.transform.parent != null ? bubble1.transform.parent.name : "";
            string parentName2 = bubble2.transform.parent != null ? bubble2.transform.parent.name : "";
            return parentName1.CompareTo(parentName2);
        });

        // Iterate through each bubble and set the pumping bool with a delay
        foreach (BubbleScript bubble in bubbles)
        {
            bubble.pumping = true;
            yield return new WaitForSeconds(delay);
        }
    }

    // Method to increment the burst counter
    public void IncrementBurstCount()
    {
        burstCount++;
        Debug.Log("Burst count: " + burstCount);

        // Check if burst count equals 18 and load the main scene
        if (burstCount == 1)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.bgm);
        }
        if (burstCount == 24)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResultsLabel : MonoBehaviour
{
    public static ResultsLabel instance; // Singleton pattern.

    //public GameObject cursor;

    public Transform labelPrefab;

    [HideInInspector]
    public Transform lastLabelPlaced;

    [HideInInspector]
    public TextMesh lastLabelPlacedText;

    public GameObject CURSOR;

    private void Awake()
    {
        // allows this instance to behave like a singleton
        instance = this;
    }

    /// <summary>
    /// Instantiate a Label in the appropriate location relative to the Main Camera.
    /// </summary>
    public void CreateLabel(GameObject cursor)
    {
        Transform adjustedLabelTransfrom = cursor.transform;
        //adjustedLabelTransfrom.rotation = transform.rotation; // Make same orientation as user.
        //adjustedLabelTransfrom.position -= new Vector3(0.0f, 0.0f, 0.1f); // Bring label slightly in-front of collided surface.
        // @TODO: make above vec3 a const or variable. x and y pos may be relevant too.

        lastLabelPlaced = Instantiate(labelPrefab, CURSOR.transform.position, transform.rotation);
        //lastLabelPlaced = Instantiate(labelPrefab, adjustedLabelTransfrom.position, transform.rotation);

        lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();

        // Change the text of the label to show that has been placed
        // The final text will be set at a later stage
        lastLabelPlacedText.text = "Analysing...";
    }

    /// <summary>
    /// Set the Tags as Text of the last Label created. 
    /// </summary>
    public void SetTagsToLastLabel(Tuple<List<string>, Dictionary<string, float>> response)
    {
        lastLabelPlacedText = lastLabelPlaced.GetComponent<TextMesh>();

        // At this point we go through all the tags received and set them as text of the label
        lastLabelPlacedText.text = "I see:\n";

        // Tags.
        //lastLabelPlacedText.text += "Tags:\n";
        //Debug.Log("\n\n TAGS\n");
        //foreach (string tag in response.Item1)
        //{
        //    lastLabelPlacedText.text += tag.ToString() + "\n";
        //}

        // Description and confidence
        lastLabelPlacedText.text += "\nDescription:\n";
        Debug.Log("\n\n DESCRIPTIONS\n");
        foreach (KeyValuePair<string, float> caption in response.Item2)
        {
            lastLabelPlacedText.text += caption.Key.ToString() + ", Confidence: " + caption.Value.ToString("0.00 \n");
            break; // Only do first caption.
        }
    }
}
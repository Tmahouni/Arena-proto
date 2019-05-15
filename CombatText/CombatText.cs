using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatText : MonoBehaviour
{
    // How fast will the spawned text object rise
    public float RiseRate = 4.0f;
    // How high should the spawned text object rise
    public float RiseHeight = 10.0f;

    // Prefab with an attached TextMesh component that will be spawned when damage is taken
    public GameObject TextPrefab = null;

    private ArrayList floatingTextObjects = new ArrayList();

    // This will be the starting height of the floating numbers
    private float initialHeight = 0.0f;

    // Use this for initialization
    void Start()
    {
        // If this component is attached to a CharacterController, use the CharacterController's height attribute to set the initial height
        CharacterController charController = gameObject.GetComponent<CharacterController>();
        if (charController != null)
            initialHeight = charController.height;
    }

    // Requires the GameObject to have a method call to: SendMessage("DamageTaken", int)
   public void Report(int Amount,Color text_color)
    {
        // Create a new text object and set the starting height and text
        GameObject textInstance = (GameObject)Instantiate(TextPrefab);
        textInstance.transform.parent = gameObject.transform;
        textInstance.transform.localPosition = new Vector3(0, initialHeight, 0);

        TextMesh mesh = textInstance.GetComponent<TextMesh>();
        mesh.text = Amount.ToString();
        mesh.renderer.material.color = text_color;
        

        // Add to the list of floating text objects to update every frame
        floatingTextObjects.Add(textInstance);
    }

    // Update is called once per frame
    void Update()
    {
		// Cache all text meshes to be deleted and later delete them
        ArrayList objectsToDelete = new ArrayList();
 
        foreach (GameObject floatingTextObject in floatingTextObjects)
        {
			float riseDelta = Time.deltaTime * RiseRate;
            Vector3 newPosition = new Vector3(floatingTextObject.transform.localPosition.x, floatingTextObject.transform.localPosition.y + riseDelta, floatingTextObject.transform.localPosition.z);
            floatingTextObject.transform.localPosition = newPosition;
            floatingTextObject.transform.LookAt(floatingTextObject.transform.position + Camera.mainCamera.transform.forward);
 
			// Delete this floating text object if it exceeds our RiseHeight property
            if (floatingTextObject.transform.localPosition.y > initialHeight + RiseHeight)
            {
                objectsToDelete.Add(floatingTextObject);
            }
        }
 
        foreach (GameObject objectToDelete in objectsToDelete)
        {
            floatingTextObjects.Remove(objectToDelete);
            Destroy(objectToDelete);
        }
    }
}
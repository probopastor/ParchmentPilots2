/*
 * Bobblehead.cs
 * Author(s): Grant Frey
 * Created on: 9/30/2020
 * Description: 
 */

using UnityEngine;

public class Bobblehead : MonoBehaviour
{
    [Tooltip("Which bobblehead in the level this is(1, 2, or 3)")]
    public int bobbleHeadNumber;

    private CollectableController collectableController;

    void Start()
    {
        collectableController = FindObjectOfType<CollectableController>();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            collectableController.CollectBobblehead(bobbleHeadNumber);
            GrayOut();
        }
    }

    public void GrayOut()
    {
        Destroy(this.gameObject);
        //Gray-out bobblehead here
    }
}

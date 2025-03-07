using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PickUpItem : MonoBehaviour
{
    [SerializeField, Tooltip("The speed that this object rotates at.")]
    private float _rotationSpeed = 500;


    public static int s_objectsCollected = 0;


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        // apply rotation
        Vector3 newRotation = transform.eulerAngles;
        newRotation.y += (_rotationSpeed * Time.deltaTime);
        transform.eulerAngles = newRotation;
    }


    public void onPickedUp(GameObject whoPickedUp)
    {

 AudioManager.instance.PlaySFX(0);

        // NEW CODE
        if (GetComponent<Weapon>() != null)
        {
            PlayerController player = whoPickedUp.GetComponent<PlayerController>();
            if (player != null)
            {
                // player has picked up a weapon
                player.EquipWeapon(GetComponent<Weapon>());


                // disable this 'pickup' script
                enabled = false;
            }
            return;
        }
        // ---------


        // OLD
        // show the collection count in the console window
        s_objectsCollected++;
        Debug.Log(s_objectsCollected + " items picked up.");


        // destroy the item
        Destroy(gameObject);
    }
}
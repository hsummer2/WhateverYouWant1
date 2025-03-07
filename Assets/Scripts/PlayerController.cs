using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // the Rigid Body physics component of this object
    // since we'll be accessing it a lot, we're store it as a member
    private Rigidbody _rigidBody;

    [SerializeField, Tooltip("How much accelleration is applied to this object when directional input is received.")]
    private float _movementAcceleration = 2;

    [SerializeField, Tooltip("The maximum velocity of this object - keeps the player from moving too fast.")]
    private float _movementVelocityMax = 2;

    [SerializeField, Tooltip("How quickly we slow down when no dirction input is received.")]
    private float _movementFriction = 0.05f;

    [SerializeField, Tooltip("Upwards force applied when Jump key is pressed.")]
    private float _jumpVelocity = 20;

    [SerializeField, Tooltip("Additional gravitation pull.")]
    private float _extraGravity = 40;

    [SerializeField, Tooltip("Are we on the ground?")]
    private bool _isGrounded = false;

    [SerializeField, Tooltip("The player's main collision shape.")]
    Collider _myCollider = null;

    //public bool _canReceiveInput = true;

    // NEW
    [SerializeField, Tooltip("this player's equipped Weapon.")]
    private Weapon _weaponEquipped = null;

    [SerializeField, Tooltip("The bullet projectile to fire.")]
    private GameObject _bulletToSpawn;

    Vector3 _curFacing = new Vector3(1, 0, 0);

    bool _moveInput = false;

    Animator _myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _myCollider = GetComponent<Collider>();
        _myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // pause movement if we've reciently attacked
        if (_weaponEquipped && _weaponEquipped.IsMovementPaused())
        {
            _rigidBody.velocity = Vector3.zero;
            return;
        }

        // get the current speed from the RigidBody physics component
        // grabbing this ensures we retain the gravity speed
        Vector3 curSpeed = _rigidBody.velocity;

        // reset move input
        _moveInput = false;

        // check to see if any of the keyboard arrows are being pressed
        // if so, adjust the speed of the player
        // also store the facing based on the keys being pressed
        if (Input.GetKey(KeyCode.D))
        {
            _moveInput = true;
            curSpeed.x += (_movementAcceleration * Time.deltaTime);
            _curFacing.x = 1;
            _curFacing.z = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            _moveInput = true;
            curSpeed.x -= (_movementAcceleration * Time.deltaTime);
            _curFacing.x = -1;
            _curFacing.z = 0;
        }

        if (Input.GetKey(KeyCode.W))
        {
            _moveInput = true;
            curSpeed.z += (_movementAcceleration * Time.deltaTime);
            _curFacing.z = 1;
            _curFacing.x = 0;
        }

        if (Input.GetKey(KeyCode.S))
        {
            _moveInput = true;
            curSpeed.z -= (_movementAcceleration * Time.deltaTime);
            _curFacing.z = -1;
            _curFacing.x = 0;
        }

        // if both left and right keys are depressed or pressed, apply friction
        if (Input.GetKey(KeyCode.A) == Input.GetKey(KeyCode.D))
        {
            curSpeed.x -= (_movementFriction * curSpeed.x);
        }

        // if both up and down keys are depressed or pressed, apply friction
        if (Input.GetKey(KeyCode.W) == Input.GetKey(KeyCode.S))
        {
            curSpeed.z -= (_movementFriction * curSpeed.z);
        }

        // does the player want to jump?
        //if ( Input.GetKeyDown(KeyCode.Space) && Mathf.Abs( curSpeed.y ) < 1 )
        if (Input.GetKeyDown(KeyCode.Space) && CalcIsGrounded())
            curSpeed.y += _jumpVelocity;
        else
            curSpeed.y -= _extraGravity * Time.deltaTime;

        // fire the weapon?
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_weaponEquipped)
            {
                _weaponEquipped.onAttack(_curFacing);

                // assign animation
                if (_weaponEquipped._attackAnim != "")
                    _myAnimator.Play(_weaponEquipped._attackAnim);
            }
        }

        // set rotation based on facing
        transform.LookAt(transform.position - new Vector3(_curFacing.x, 0f, _curFacing.z));

        UpdateAnimation();

        // apply the max speed
        curSpeed.x = Mathf.Clamp(curSpeed.x, _movementVelocityMax * -1, _movementVelocityMax);
        curSpeed.z = Mathf.Clamp(curSpeed.z, _movementVelocityMax * -1, _movementVelocityMax);

        // adjust the velocity of this object's physics component
        _rigidBody.velocity = curSpeed;

    }

    void OnTriggerEnter(Collider collider)
    {
        // did we collide with a PickupItem?
        if (collider.gameObject.GetComponent<PickUpItem>())
        {
            // get component returned a valid PickupItem
            // so let that item know it was grabbed by this gameObject
            PickUpItem collisionItem = collider.gameObject.GetComponent<PickUpItem>();
            collisionItem.onPickedUp(this.gameObject);
        }
    }

    void UpdateAnimation()
    {
        if (_myAnimator == null)
            return;

        if (_moveInput)
        {
            _myAnimator.Play("Run");
        }
        else
        {
            _myAnimator.Play("Idle");
        }

    }

    /// <summary>
    /// Check below the player object. 
    /// If they're standing on a solid object, they can Jump 
    /// and perform other actions not available in mid-air.
    /// </summary>
    bool CalcIsGrounded()
    {
        float offset = 0.1f;

        Vector3 pos = _myCollider.bounds.center;
        pos.y = _myCollider.bounds.min.y - offset;

        _isGrounded = Physics.CheckSphere(pos, offset);

        return _isGrounded;
    }


    // NEW CODE
    #region *** Weapons ***

    public void EquipWeapon(Weapon weapon)
    {
        _weaponEquipped = weapon;
        weapon.SetAttachmentParent(GameObject.Find("WEAPON_LOC"), gameObject);
    }

    #endregion
}
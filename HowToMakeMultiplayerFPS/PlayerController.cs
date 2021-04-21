/*
     * handles all the inputs and stuff like how much feel is left
     * if jumping what way should be looking
     */
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerMoter))]
[RequireComponent(typeof(ConfigurableJoint))]


public class PlayerController : MonoBehaviour {
    

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return this.thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;
    

    [Header("Spring Settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 40f;

    //Component caching
    private PlayerMoter motor;
    private ConfigurableJoint joint;
    private Animator animator;

    private void Start()
    {
        motor = GetComponent<PlayerMoter>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSettings(jointSpring);
    }
    private void Update()
    {
        if (PauseMenu.IsOn)
        {
            if (Cursor.lockState != CursorLockMode.None)
                Cursor.lockState = CursorLockMode.None;

            motor.Move(Vector3.zero);
            motor.Rotate(Vector3.zero);
            motor.RotateCamera(0f);

            return;
        }

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Setting target position for spring
        //This makes the physics act right when it comes to 
        //applying gravity when flying over object
        RaycastHit _hit;
        if(Physics.Raycast(transform.position,Vector3.down,out _hit,100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0f, -_hit.point.y, 0f);
        }
        else
        {
            joint.targetPosition = new Vector3(0f, 0f, 0f);
        }

        //calculate movement, velocity, 3d vector
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _moveHorizontal = transform.right * _xMov;
        Vector3 _moveVertical = transform.forward * _zMov;

        //final Movement vector
        Vector3 _velocity = (_moveHorizontal + _moveVertical) * speed;

        //Animate movement
        animator.SetFloat("ForwardVelocity", _zMov);

        //Apply Movement
        motor.Move(_velocity);

        
        //Calculate Rotation as a 3d vector(Turning around) 
        
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity ;

        //Apply Rotation
        motor.Rotate(_rotation);

        //Calculate Camera Rotation as a 3d vector(Turning up and down) 

        float _xRot = Input.GetAxisRaw("Mouse Y");

        float _cameraRotationX = _xRot * lookSensitivity;

        //Apply Rotation
        motor.RotateCamera(_cameraRotationX);

        //Caculate the thrusterforse based on player input
        Vector3 _thrusterForce = Vector3.zero;

        if (Input.GetButton("Jump")&& thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount > 0.001f)
            {
                _thrusterForce = Vector3.up * thrusterForce;
                SetJointSettings(0f);
            }

        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;

            SetJointSettings(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);

        //Apply the Thurster force
        motor.ApplyThruster(_thrusterForce);

    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { 
                                        positionSpring = jointSpring,
                                        maximumForce = jointMaxForce
        };
    }


}

using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    #region Player Movement Variables
    [Header("Player Movement")]
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    PlayerGunSelector playerGunSelector;

    public Vector2 MovementInput;
    public float VerticalInput;
    public float HorizontalInput;
    #endregion

    #region Mouse Aim Variables
    [Header("Mouse Aim")]
    [SerializeField] private LayerMask groundMask;
    public GameObject AimTarget;
    Camera mainCamera;
    #endregion

    #region Player Hurt Variables

    EnemyHealth Health;

    bool isHurt;
    private Coroutine ReactionRoutine;

    #endregion
    void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.Player.Movement.performed += i => MovementInput = i.ReadValue<Vector2>();

            playerControls.Player.Aim.started += context =>
            {
                if (context.interaction is HoldInteraction)
                {
                    Debug.Log("HOLD");
                }
                else if(context.interaction is PressInteraction)
                {
                    Debug.Log("PRESS");
                }
            };
        }

        playerControls.Enable();
    }

    // Start is called before the first frame update
    void Awake()
    {
        Health = GetComponent<EnemyHealth>();
        animatorManager = GetComponent<AnimatorManager>();
        playerGunSelector = GetComponent<PlayerGunSelector>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleTargetPosition();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        MouseInput();
    }

    // Sabit açýlý üstten kamera ve fareyi takip eden karakter animasyon inputlarý
    private void HandleMovementInput()
    {
        VerticalInput = MovementInput.y;
        HorizontalInput = MovementInput.x;

        // niþan ile karakter arasýndaki mesafe yöne çevrilir
        Vector3 distance = new Vector3((AimTarget.transform.position.x - transform.position.x) * -1, 0, (AimTarget.transform.position.z - transform.position.z) * -1);     // karakterin yönü ters olduðu için -1
        Vector3 animInput = Vector3.zero;
        if (Mathf.Abs(distance.x) >= Mathf.Abs(distance.z))
        {
            animInput = new Vector3(distance.x * 1 / Mathf.Abs(distance.x), 0, distance.z * 1 / Mathf.Abs(distance.x));    // blend tree karesinde büyük olan köþeye yuvarlanýr, yani kuvvetlerden en az biri 1 olur ( 0.6 ve 0.4 yerine 1 ve 0.7 gibi)
        }
        else
        {
            animInput = new Vector3(distance.x * 1 / Mathf.Abs(distance.z), 0, distance.z * 1 / Mathf.Abs(distance.z));
        }
        
        //Debug.Log(animInput);
        // inputa göre yönler ayarlanýyor, karakterin döndüðü yere göre X ve Y/Z inputu yer deðiþtirebiliyor ( saða bakarken A tuþu ile geri yürüme animasyonu oynamasý gibi)
        if (MovementInput.y > 0)
            animatorManager.UpdateAnimatorValue(animInput.x * -1, animInput.z);
        else if(MovementInput.y < 0)
            animatorManager.UpdateAnimatorValue(animInput.x, animInput.z * -1);
        else if(MovementInput.x > 0)
            animatorManager.UpdateAnimatorValue(animInput.z, animInput.x * 1);
        else if(MovementInput.x < 0)
            animatorManager.UpdateAnimatorValue(animInput.z * -1, animInput.x * -1);
        else
            animatorManager.UpdateAnimatorValue(0, 0);
    }

    private void HandleTargetPosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 dif = new Vector3((hitInfo.point.x - transform.position.x) * -1, 0, (hitInfo.point.z - transform.position.z) * -1);     // -1 because character has reverse coordinates
            float sum = Mathf.Abs(dif.x) + Mathf.Abs(dif.z);
            Vector3 dir = new Vector3(dif.x / sum * -1, 0, dif.z / sum);
            //Debug.Log(dir);

            AimTarget.transform.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
            
            //Debug.DrawLine(transform.position, AimTarget.transform.position, Color.red, 0);
        }
        else
        {
            //Debug.LogWarning("Player can't aim!, Missing ground layer!");
        }
    }


    [Header("Mouse Click")]
    public bool MouseHeld;
    Coroutine ReleaseAimingStateRoutine = null;
    // Mouse Click And Input Handler
    private void MouseInput()
    {
        #region Mouse Left Button
        if (Input.GetMouseButtonDown(0))
        {
            playerGunSelector.Shoot();

            MouseHeld = true;
            animatorManager.UpdateAimState(true);
            
            if(ReleaseAimingStateRoutine != null)
            {
                StopCoroutine(ReleaseAimingStateRoutine);
                ReleaseAimingStateRoutine = null;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseHeld = false;

            ReleaseAimingStateRoutine = StartCoroutine(ReleaseAimingState());
        }
        else if (Input.GetMouseButton(0))
        {
            if (playerGunSelector.ActivePrimaryGun.Automatic)
                playerGunSelector.Shoot();
        }
        #endregion

        #region Mouse Scroll Wheel
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            playerGunSelector.ScrollWeapon(+1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            playerGunSelector.ScrollWeapon(-1);
        }
        #endregion
    }

    private IEnumerator ReleaseAimingState()
    {
        yield return new WaitForSeconds(4);

        ReleaseAimingStateRoutine = null;

        animatorManager.UpdateAimState(false);
    }

    public void SelectGunKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            playerGunSelector.SelectWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            playerGunSelector.SelectWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            playerGunSelector.SelectWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            playerGunSelector.SelectWeapon(3);
        }
    }
}

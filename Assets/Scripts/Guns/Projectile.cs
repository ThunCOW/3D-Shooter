using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    public int Speed;
    public float DisappearTime;
    public List<AudioClip> ExplosionSounds;
    public float volume;

    public ObjectPool<GameObject> pool;

    public Vector3 StartPoint;
    public Vector3 EndPoint;

    //private Rigidbody rb;
    private GameObject rocketObject;
    private GameObject explosionObject;
    
    private AudioSource audioSource;

    public delegate void OnColliderEnterEvent(Projectile projectile, Collider other);
    public event OnColliderEnterEvent OnCollision;

    [SerializeField] private float distance;
    [SerializeField] private float remainingDistance;

    private void Awake()
    {
        //rb = transform.parent.GetComponent<Rigidbody>();
        rocketObject = transform.GetChild(0).gameObject;
        explosionObject = transform.GetChild(1).gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        distance = Vector3.Distance(StartPoint, EndPoint);
        remainingDistance = distance;
        StartCoroutine(DisappearIfNotHit());
    }

    bool canMove = true;
    private void Update()
    {
        if (canMove)
        {
            transform.parent.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance)));

            remainingDistance -= Speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //rb.velocity = Vector3.zero;
        canMove = false;
        rocketObject.SetActive(false);
        explosionObject.SetActive(true);
        audioSource.PlayOneShot(ExplosionSounds[Random.Range(0, ExplosionSounds.Count)], volume);

        OnCollision?.Invoke(this, other);
        OnCollision = null;

        StartCoroutine(BackToPool(DisappearTime));
    }

    private IEnumerator BackToPool(float Time)
    {
        yield return new WaitForSeconds(DisappearTime);

        GameObject parent = transform.parent.gameObject;

        pool.Release(parent);
        parent.SetActive(false);
    }

    private IEnumerator DisappearIfNotHit()
    {
        yield return new WaitForSeconds(15);
        canMove = false;
        rocketObject.SetActive(false);

        OnCollision = null;

        StartCoroutine(BackToPool(DisappearTime));
    }

    private void OnDisable()
    {
        canMove = false;
        explosionObject.SetActive(false);
        StopAllCoroutines();
        //rb.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        distance = Vector3.Distance(StartPoint, EndPoint);
        remainingDistance = distance;
        
        rocketObject.SetActive(true);
        canMove = true;
    }
}

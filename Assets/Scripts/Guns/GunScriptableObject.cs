using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Guns/Handgun", order = 0)]
public class GunScriptableObject : ScriptableObject, System.ICloneable
{
    public GunType ID;
    public PlayerWeapons WeaponType;
    public string Name;
    public GameObject ModelPrefab;
    public string AnimatorLayerName;
    public int CurrentAmmo;
    public bool Automatic;              // if true it can be shot while holding the mouse click
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public DamageConfigScriptableObject DamageConfig;
    public GunAmmoConfigScriptableObject AmmoConfig;
    public ShootConfigurationScriptableObject ShootConfig;
    public TrailConfigurationScriptableObject TrailConfig;
    public GunAudioConfigScriptableObject AudioConfig;

    public AudioSource ShootingAudioSource;
    
    private MonoBehaviour activeMonoBehaviour;
    private GameObject model;
    private Animator gunAnimator;
    //private ParticleSystem shootSystem;
    private GameObject shootSystem;
    private static ObjectPool<TrailRenderer> trailPool;

    public virtual void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        activeMonoBehaviour = ActiveMonoBehaviour;
        if (trailPool == null )
            trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
        
        model = Instantiate(ModelPrefab);
        model.transform.SetParent(Parent, false);
        model.transform.position = model.transform.position;
        //model.transform.localRotation = Quaternion.Euler(SpawnRotation);
        model.transform.rotation = model.transform.rotation;

        gunAnimator = model.GetComponentInChildren<Animator>();
        ShootingAudioSource = model.GetComponent<AudioSource>();

        //shootSystem = model.GetComponentInChildren<ParticleSystem>();
        //shootSystem = GameManager.Instance.Player;
        shootSystem = model;
    }

    public void Shoot()
    {
        //gunAnimator.SetTrigger("Shoot");

        AudioConfig.PlayShootingClip(ShootingAudioSource);
        //shootSystem.Play();
        Vector3 shootDirection = GameManager.Instance.Player.transform.forward
        //Vector3 shootDirection = model.transform.forward
            + new Vector3(
                Random.Range(
                    -ShootConfig.Spread.x,
                    ShootConfig.Spread.x
                    ),
                Random.Range(
                    -ShootConfig.Spread.y,
                    ShootConfig.Spread.y
                    ),
                Random.Range(
                    -ShootConfig.Spread.z,
                    ShootConfig.Spread.z)
                );
        shootDirection.Normalize();

        if(Physics.Raycast(shootSystem.transform.position, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
        {
            activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, hit.point, hit));
        }
        else
        {
            activeMonoBehaviour.StartCoroutine(PlayTrail(shootSystem.transform.position, shootSystem.transform.position + (shootDirection * TrailConfig.MissDistance), new RaycastHit()));
        }
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = StartPoint;
        yield return null; //avoid position carry-over from last frame if reused

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        while(remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint,
                EndPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance)));
            
            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        if(hit.collider != null )
        {
            // Handle Impact 
            
            if(hit.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(DamageConfig.GetDamage(distance));
            }
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }

    public object Clone()
    {
        GunScriptableObject config = CreateInstance<GunScriptableObject>();

        config.ID = ID;
        config.WeaponType = WeaponType;
        config.Name = Name;
        config.ModelPrefab = ModelPrefab;
        config.AnimatorLayerName = AnimatorLayerName;
        config.CurrentAmmo = AmmoConfig.MaxAmmo;
        config.Automatic = Automatic;
        config.SpawnPoint = SpawnPoint;
        config.SpawnRotation = SpawnRotation;

        config.DamageConfig = DamageConfig.Clone() as DamageConfigScriptableObject;
        config.AmmoConfig = AmmoConfig.Clone() as GunAmmoConfigScriptableObject;
        config.ShootConfig = ShootConfig.Clone() as ShootConfigurationScriptableObject;
        config.TrailConfig = TrailConfig.Clone() as TrailConfigurationScriptableObject;
        config.AudioConfig = AudioConfig;

        return config;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneParticleHandler : MonoBehaviour
{
    private TestFlight testFlight;
    private PlaneThrow_Handler planeThrowHandler;

    private ParticleSystem.MainModule leftMain;
    private ParticleSystem.MainModule rightMain;

    ParticleSystem.EmissionModule emissionModuleLeft;
    ParticleSystem.EmissionModule emissionModuleRight;

    public float windEmmissionRate = 10f;

    [Tooltip("The left wing's air particles.")]
    public ParticleSystem leftSystem;

    [Tooltip("The right wing's air particles.")]
    public ParticleSystem rightSystem;

    [Tooltip("The rate at which the wing particle's color and albedo change with velocity")]
    public float increaseParticleColorSwitchRate = 10.0f;

    [Tooltip("Default wing aircurrent particle color")]
    public Color particleColor;

    [Tooltip("Wing aircurrent particle color at the min velocity")]
    public Color particleColorMinSpeed;

    [Tooltip("Wing aircurrent particle color at the max velocity")]
    public Color particleColorMaxSpeed;

    [Tooltip("The rate at which the wing particle's lifespan change with velocity")]
    public float increaseParticleLifeTimeRate = 10.0f;

    [Tooltip("Wing aircurrent particle lifetime at the min velocity")]
    public float minLifetime = 0.1f;

    [Tooltip("Wing aircurrent particle lifetime at the max velocity")]
    public float maxLifetime = 0.5f;

    [Tooltip("The rate at which the wing particle's size change with velocity")]
    public float increaseParticleSizeRate = 10.0f;

    [Tooltip("Wing aircurrent particle size at the min velocity")]
    public float minSize = 0f;

    [Tooltip("Wing aircurrent particle size at the max velocity")]
    public float maxSize = 1f;

    // Start is called before the first frame update
    void Start()
    {
        testFlight = FindObjectOfType<TestFlight>();
        planeThrowHandler = FindObjectOfType<PlaneThrow_Handler>();

        leftMain = leftSystem.main;
        rightMain = rightSystem.main;

        emissionModuleLeft = leftSystem.emission;
        emissionModuleRight = rightSystem.emission;

        emissionModuleLeft.rateOverDistance = 0f;
        emissionModuleRight.rateOverDistance = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (testFlight.GetIsThrown())
        {
            emissionModuleLeft.rateOverDistance = windEmmissionRate;
            emissionModuleRight.rateOverDistance = windEmmissionRate;

            if (!testFlight.GetEffectPlayOnce())
            {
                leftSystem.Play();
                rightSystem.Play();
            }
        }
        else if (!testFlight.GetIsThrown())
        {
            leftSystem.Stop();
            rightSystem.Stop();
        }

        WindParticleHandler();
    }

    /// <summary>
    /// Alters wind particle appearance in color, size, and lifetime based on plane velocity.
    /// </summary>
    private void WindParticleHandler()
    {
        var particleColorChange = testFlight.GetPlaneVelocity() / increaseParticleColorSwitchRate;
        leftMain.startColor = new Color((Mathf.Clamp(particleColorChange, particleColorMinSpeed.r, particleColorMaxSpeed.r)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.g, particleColorMaxSpeed.g)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.b, particleColorMaxSpeed.b)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.a, particleColorMaxSpeed.a)));
        rightMain.startColor = new Color((Mathf.Clamp(particleColorChange, particleColorMinSpeed.r, particleColorMaxSpeed.r)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.g, particleColorMaxSpeed.g)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.b, particleColorMaxSpeed.b)),
            (Mathf.Clamp(particleColorChange, particleColorMinSpeed.a, particleColorMaxSpeed.a)));

        var particleLifetimeChange = testFlight.GetPlaneVelocity() / increaseParticleLifeTimeRate;
        leftMain.startLifetime = Mathf.Clamp(particleLifetimeChange, minLifetime, maxLifetime);
        rightMain.startLifetime = Mathf.Clamp(particleLifetimeChange, minLifetime, maxLifetime);

        var particleSizeChange = testFlight.GetPlaneVelocity() / increaseParticleSizeRate;
        leftMain.startSize = Mathf.Clamp(particleSizeChange, minSize, maxSize);
        rightMain.startSize = Mathf.Clamp(particleSizeChange, minSize, maxSize);
    }

    public void PauseParticles()
    {
        emissionModuleLeft.rateOverDistance = 0f;
        emissionModuleRight.rateOverDistance = 0f;

        leftSystem.Pause();
        rightSystem.Pause();
    }

    public void ResumeParticles()
    {
        emissionModuleLeft.rateOverDistance = windEmmissionRate;
        emissionModuleRight.rateOverDistance = windEmmissionRate;

        leftSystem.Play();
        rightSystem.Play();
    }

    public void PlayParticles()
    {
        leftSystem.Play();
        rightSystem.Play();
    }
}

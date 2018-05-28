using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcess : MonoBehaviour 
{
    static PostProcess Instance;
    static p_PlayerBeing PB;

    [Header("Vignette")]
    [SerializeField] float mutilateFullVignette = 0.1f;
    [SerializeField] float mutilateFullVignetteLinearity = 0.5f;
    [SerializeField] float lockVignette = 0.05f;

    [Header("Color Grading")]
    [SerializeField] float midMadnessTemperature = 10;
    [SerializeField] float euphoriaTemperature = 20;
    [SerializeField] float maxMadnessTemperature = 30;
    [SerializeField] float midMadnessSaturation = .95f;
    [SerializeField] float euphoriaSaturation = .8f;
    [SerializeField] float maxMadnessSaturation = .7f;
    [SerializeField] float midMadnessContrast = 1.05f;
    [SerializeField] float euphoriaContrast = 1.2f;
    [SerializeField] float maxMadnessContrast = 1.3f;
    [SerializeField] float midMadnessRed = 1.1f;
    [SerializeField] float euphoriaRed = 1.2f;
    [SerializeField] float maxMadnessRed = 1.4f;


    [Header("Chromatic Variations")]
    [SerializeField] float minChromaticVariation;
    [SerializeField] float maxChromaticVariation;
    [SerializeField] float timeFactor = 5f;

    [Header("Euphoria Transition")]
    [SerializeField] float toEuphoriaTime = 2f;
    [SerializeField] float toEuphoriaChromatic = 1f;
    [SerializeField] float toEuphoriaChromaticLinearity = 1.5f;
    [SerializeField] float toEuphoriaFov = 25f;
    [SerializeField] float toEuphoriaFovLinearity = 2f;

    public float MutilateVignette { get { return mutilateFullVignette * mutilateProgress; } }
    public float LockVignette { get { return lockProgress ? lockVignette : 0f; ; } }

    public static float fovOffset { get { return Instance.transitionFov; } }

    PostProcessingProfile oProfile;
    PostProcessingProfile profile;
    VignetteModel.Settings oVignetteSettings;



    static float mutilateProgress = 0f;
    static float madnessProgress = 0f, oldMadnessProgress = 0f;
    static float darkenForce = 1f;
    static bool lockProgress = false;
    static float euphoriaThreshold
    {
        get
        {
            return PB.EuphoriaThreshold;
        }
    }


    float MadnessTemperature
    {
        get
        {
            if (madnessProgress < euphoriaThreshold)
                return Utilities.Lerp(0, midMadnessTemperature, madnessProgress / euphoriaThreshold);
            else
                return Utilities.Lerp(euphoriaTemperature, maxMadnessTemperature, (madnessProgress - euphoriaThreshold) / (1 - euphoriaThreshold));
        }
    }
    float MadnessSaturation
    {
        get
        {
            if (madnessProgress < euphoriaThreshold)
                return Utilities.Lerp(0, midMadnessSaturation, madnessProgress / euphoriaThreshold);
            else
                return Utilities.Lerp(euphoriaSaturation, maxMadnessSaturation, (madnessProgress - euphoriaThreshold) / (1 - euphoriaThreshold));
        }
    }
    float MadnessContrast
    {
        get
        {
            if (madnessProgress < euphoriaThreshold)
                return Utilities.Lerp(0, midMadnessContrast, madnessProgress / euphoriaThreshold);
            else
                return Utilities.Lerp(euphoriaContrast, maxMadnessContrast, (madnessProgress - euphoriaThreshold) / (1 - euphoriaThreshold));
        }
    }
    float MadnessRed
    {
        get
        {
            if (madnessProgress < euphoriaThreshold)
                return Utilities.Lerp(0, midMadnessRed, madnessProgress / euphoriaThreshold);
            else
                return Utilities.Lerp(euphoriaRed, maxMadnessRed, (madnessProgress - euphoriaThreshold) / (1 - euphoriaThreshold));
        }
    }
    float RangedChromaticVariation
    {
        get
        {
            if (madnessProgress > euphoriaThreshold)
                return Utilities.Lerp(minChromaticVariation, maxChromaticVariation, (madnessProgress - euphoriaThreshold) / (1 - euphoriaThreshold));
            else
                return 0f;
        }
    }
    float ChromaticVariation
    {
        get
        {
            return Mathf.Sin(0.4f * Time.time * timeFactor) * Mathf.Cos(0.6f * Time.time * timeFactor) * Mathf.Sin(1.4f * Time.time * timeFactor) * RangedChromaticVariation;
        }
    }

    private void Awake()
    {
        Instance = this;
        PB = FindObjectOfType<p_PlayerBeing>();
        oProfile = GetComponent<PostProcessingBehaviour>().profile;

        profile = new PostProcessingProfile();
        profile.vignette.enabled = oProfile.vignette.enabled;
        profile.vignette.settings = oProfile.vignette.settings;
        profile.antialiasing.enabled = oProfile.antialiasing.enabled;
        profile.antialiasing.settings = oProfile.antialiasing.settings;
        profile.motionBlur.enabled = oProfile.motionBlur.enabled;
        profile.motionBlur.settings = oProfile.motionBlur.settings;
        profile.eyeAdaptation.enabled = oProfile.eyeAdaptation.enabled;
        profile.eyeAdaptation.settings = oProfile.eyeAdaptation.settings;
        profile.bloom.enabled = oProfile.bloom.enabled;
        profile.bloom.settings = oProfile.bloom.settings;
        profile.colorGrading.enabled = oProfile.colorGrading.enabled;
        profile.colorGrading.settings = oProfile.colorGrading.settings;
        profile.chromaticAberration.enabled = oProfile.chromaticAberration.enabled;
        profile.chromaticAberration.settings = oProfile.chromaticAberration.settings;
        profile.grain.enabled = oProfile.grain.enabled;
        profile.grain.settings = oProfile.grain.settings;
        profile.depthOfField.enabled = oProfile.depthOfField.enabled;
        profile.depthOfField.settings = oProfile.depthOfField.settings;

        GetComponent<PostProcessingBehaviour>().profile = profile;
    }

    VignetteModel.Settings vignetteSettings;
    ColorGradingModel.Settings colorGradingSettings;
    ChromaticAberrationModel.Settings chromaticSettings;
    DepthOfFieldModel.Settings depthOfFieldSettings;

    private void Update()
    {
        vignetteSettings = profile.vignette.settings;
        vignetteSettings.intensity = oProfile.vignette.settings.intensity + MutilateVignette + LockVignette;
        profile.vignette.settings = vignetteSettings;

        colorGradingSettings = profile.colorGrading.settings;
        colorGradingSettings.basic.temperature = oProfile.colorGrading.settings.basic.temperature + MadnessTemperature;
        colorGradingSettings.basic.temperature = oProfile.colorGrading.settings.basic.saturation + MadnessSaturation;
        colorGradingSettings.basic.temperature = oProfile.colorGrading.settings.basic.contrast + MadnessContrast;
        colorGradingSettings.channelMixer.red = new Vector3(oProfile.colorGrading.settings.channelMixer.red.x + MadnessRed,
                                                            oProfile.colorGrading.settings.channelMixer.red.y + MadnessRed,
                                                            oProfile.colorGrading.settings.channelMixer.red.z + MadnessRed);

        profile.colorGrading.settings = colorGradingSettings;

        chromaticSettings = profile.chromaticAberration.settings;
        chromaticSettings.intensity = oProfile.chromaticAberration.settings.intensity + transitionChromatic + ChromaticVariation;
        profile.chromaticAberration.settings = chromaticSettings;

        if (oldMadnessProgress <= euphoriaThreshold && madnessProgress > euphoriaThreshold)
        {
            StopCoroutine("ToEuphoriaTransition");
            StopCoroutine("ToNonEuphoriaTransition");
            StartCoroutine("ToEuphoriaTransition");
        }
        else if (oldMadnessProgress > euphoriaThreshold && madnessProgress <= euphoriaThreshold)
        {
            Debug.Log("Reverse transition effect");
        }
    }
    private void LateUpdate()
    {
        oldMadnessProgress = madnessProgress;
    }

    float transitionChromatic = 0f;
    float transitionFov = 0f;
    IEnumerator ToEuphoriaTransition()
    {
        float count = 0f, maxTime = toEuphoriaTime, progress;
        while (count < maxTime)
        {
            progress = count / maxTime;
            transitionChromatic =   Utilities.Linearity(Utilities.Lerp(1f, 0f, progress), 0, 1, toEuphoriaChromaticLinearity, true);
            transitionFov =         Utilities.Linearity(Utilities.Lerp(1f, 0f, progress), 0, 1, toEuphoriaFovLinearity, true) * toEuphoriaFov;
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime;
        }
        transitionChromatic = 0f;
    }

    public static void SetMutilatePP(float value)
    {
        mutilateProgress = Utilities.Linearity(Utilities.Limit(value, 0, 1), 0, 1, Instance.mutilateFullVignetteLinearity, true);
    }
    public static void SetLockPP(bool value)
    {
        lockProgress = value;
    }

    public static void SetMadnessPP(float value)
    {
        madnessProgress = value;
    }

    public static void BlackFade()
    {
        Instance.StartCoroutine("BlackFadeCoroutine");
    }

    public static void DarkenAndBlur(bool activate)
    {
        Instance.profile.depthOfField.enabled = activate;
        Instance.colorGradingSettings = Instance.profile.colorGrading.settings;
        Instance.colorGradingSettings.basic.postExposure = activate ? -darkenForce : 0;
        Instance.profile.colorGrading.settings = Instance.colorGradingSettings;
    }

    IEnumerator BlackFadeCoroutine()
    {
        colorGradingSettings = profile.colorGrading.settings;
        float count = 0f, maxTime = 3f;
        while (count < maxTime)
        {
            colorGradingSettings.basic.postExposure = -count / maxTime * 20f;
            profile.colorGrading.settings = colorGradingSettings;
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime;

        }
    }
}
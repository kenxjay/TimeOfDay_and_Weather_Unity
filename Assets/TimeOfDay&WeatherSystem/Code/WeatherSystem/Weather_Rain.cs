﻿using UnityEngine;
using System.Collections;

public class Weather_Rain : Weather_Base 
{
    /********** ----- VARIABLES ----- **********/

    /// <summary>
    /// Here we save the particle effect that we use for the rain effect. This is also where the sound will play from\n
    /// *Use \link GetSet_gPartRain \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private GameObject _gPartRain;

    private float _fEndParticleTimerStart;
    private float _fEndParticleTimerEnd;

    /********** ----- GETTERS AND SETTERS ----- **********/

    public GameObject GetSet_gPartRain
    {
        get { return _gPartRain; }
        set { _gPartRain = value; }
    }

    private void Start()
    {
        clWeatherController = (Weather_Controller)this.GetComponent(typeof(Weather_Controller));

        if (_bUseMorningFog == false)
            _fFogMorningAmount = _fFogAmount;

        // Make sure we fade the sound in and out
        _fSoundVolumeIn = _fSoundVolume;
        _fSoundVolumeOut = _fSoundVolume;

        // This timer makes sure that the rain stops falling and don't suddenly just disappears
        _fEndParticleTimerStart = 0.0f;
        _fEndParticleTimerEnd = 5.0f;

        if (_bUsingSound == true)
        {
            if (_adAmbientSound != null)
            {
                if (_gPartRain.GetComponent<AudioSource>() != null)
                {
                    _bGotAudioSource = true;
                    _gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
                    _gPartRain.GetComponent<AudioSource>().volume = 0.0f;
                    _gPartRain.GetComponent<AudioSource>().loop = true;
                }
                else
                {
                    _gPartRain.AddComponent<AudioSource>();
                    _gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
                    _gPartRain.GetComponent<AudioSource>().volume = 0.0f;
                    _gPartRain.GetComponent<AudioSource>().loop = true;
                    Debug.LogWarning("There was no AUDIOSOURCE on " + _gPartRain + " this is now added");

                    _bGotAudioSource = true;
                }
            }
            else
                Debug.Log("There is no AMBIENT SOUND attached to the WeatherController on type: " + clWeatherController.en_CurrWeather + " If you don't want to use Ambient sound on this weather type, set Using Ambient Sound to false!");
        }
    }

    public override void Init()
    {
        base.Init();
        TurnOnRain();

        // We turn on emission on the particle system as we always turn it off in the end
        if (_gPartRain != null)
        {
            ParticleSystem rp = _gPartRain.GetComponent<ParticleSystem>();
            var rem = rp.emission;
            rem.enabled = true;
        }
    }

    private void Update()
    {
        UpdateWeather();

        if (_bUseInit == true)
        {
            _fInitTimerStart += Time.deltaTime;

            if (_fInitTimerStart >= _fInitTimerEnd)
            {
                Init();
                _fInitTimerStart = 0.0f;
                _bUseInit = false;
            }
        }
    }

    public override void UpdateWeather()
    {

        if (_bUseDifferentFadeTimes == false)
            OneFadeTimeToRuleThemAll();
        else
            DifferentFadeTimes();
    }

    private void OneFadeTimeToRuleThemAll()
    {
        if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
        {
            clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pNightParticle);
            clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
        {
            clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0.0f, _cNight_MoonLightColor,
                _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
            clWeatherController.ActivateTimesetParticle(_pDayParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
        {
            clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pDayParticle);
            clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
        {
            clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity,
                _cNight_MoonLightColor, _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
            clWeatherController.ActivateTimesetParticle(_pNightParticle);
        }
    }

    private void DifferentFadeTimes()
    {
        if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
        {
            clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fSunriseFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pNightParticle);
            clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
        {
            clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0.0f, _cNight_MoonLightColor,
                _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fDayFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
            clWeatherController.ActivateTimesetParticle(_pDayParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
        {
            clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fSunsetFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pDayParticle);
            clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
        {
            clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity, _cNight_MoonLightColor,
                _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fNightFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
            clWeatherController.ActivateTimesetParticle(_pNightParticle);
        }
    }

    private void TurnOnRain()
    {
        if (_gPartRain != null)
        {
            if (_gPartRain.activeInHierarchy == false && _gPartRain != null)
            {
                _gPartRain.SetActive(true);

                if (
                    _bUsingSound == true)
                    TurnOnSound(_gPartRain);
            }
        }
        else
            Debug.Log("We are missing rain particles on: " + this.gameObject + " For weather type: RAIN");
    }

    public override void TurnOnSound(GameObject gameobject)
    {
        base.TurnOnSound(gameobject);

        // Check to tell that we should start fading the sound out when exiting this weather function
        _bTurnOffSoundAtExit = true;
    }

    /// <summary>
    /// How this works per rain type
    /// 1. I activate a timer since I want it to reset and not start again after deactivateing the particlesystem
    /// 2. Turn off emission so the rain stops falling before we turn off the particle
    /// 3. When exit start fading sound out if this is true
    /// 3.1. Set this to false again so we can turn on the sound again when this weather effect happens again
    /// 4. When this timer ends turn off the active rain
    /// 4.1. Set back start timer to 0.0f
    /// 4.2. Deactivate the particle system
    /// </summary>
    public override void ExitWeatherEffect(GameObject gameobject)
    {
        clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
        clWeatherController.DeactivateTimesetParticle(_pDayParticle);
        clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
        clWeatherController.DeactivateTimesetParticle(_pNightParticle);

        if (_gPartRain != null && _gPartRain.activeInHierarchy == true)
        {
            _fEndParticleTimerStart += Time.deltaTime;
            ParticleSystem rp = _gPartRain.GetComponent<ParticleSystem>();
            var rem = rp.emission;
            rem.enabled = false;

            if (_bTurnOffSoundAtExit == true)
            {
                if (_bUsingSound == true)
                    base.ExitWeatherEffect(gameobject);

                _bTurnOffSoundAtExit = false;
            }

            if (_fEndParticleTimerStart > _fEndParticleTimerEnd)
            {
                _fEndParticleTimerStart = 0.0f;
                _gPartRain.SetActive(false);
            }
        }
    }
}


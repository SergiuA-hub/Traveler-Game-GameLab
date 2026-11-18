
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource introSoundSource;
    public AudioSource worldSfxEffect;
    public AudioSource weatherEffect;
    public AudioSource specialWeatherFfx;
    public AudioSource specialSoundsSource;
    public AudioSource settlementSoundSource;
    public AudioSource playerSoundSource;

    [Header("Intro")]
    public AudioClip introSoundClip;

    [Header("World Sounds")]
    public List<AudioClip> birdsEffect;
    public AudioClip fallBaseWind;

    [Header("Weather Sounds")]
    public AudioClip summerRain;
    public AudioClip fallRain;
    public AudioClip storm;
    public AudioClip thunder;
    public AudioClip blizzard;

    [Header("Settlements Sounds")]
    public AudioClip genericSettlementSound;

    [Header("Camp Sounds")]
    public AudioClip eatSound;
    public AudioClip drinkSound;
    public AudioClip sleepSound;

    [Header("Weather Settings")]
    [SerializeField] private float weatherFadeDuration = 3f;

    private Coroutine weatherFadeCoroutine;
    private float weatherNormalVolume;

    private bool fall = false;
    private bool winter = false;

    void Start()
    {
        fall = false;
        winter = false;

        weatherNormalVolume = weatherEffect.volume;

        introSoundSource.clip = introSoundClip;
        introSoundSource.loop = false;
        introSoundSource.Play();

    }

    public void playEatSound()
    {
        playerSoundSource.clip = eatSound;
        playerSoundSource.loop = false;
        playerSoundSource.Play();
    }

    public void playDrinkSound()
    {
        playerSoundSource.clip = drinkSound;
        playerSoundSource.loop = false;
        playerSoundSource.Play();
    }
    public void playSleepSound()
    {
        playerSoundSource.clip = sleepSound;
        playerSoundSource.loop = true;
        playerSoundSource.Play();
    }

    public void stopPlayerSounds()
    {
        playerSoundSource.Stop();
    }

    public void playSettlementSound()
    {
        settlementSoundSource.clip = genericSettlementSound;
        settlementSoundSource.loop = true;
        settlementSoundSource.Play();
    }

    public void stopSettlementSound()
    {
        settlementSoundSource.Stop();
    }

    public void summerSounds()
    {
        if (birdsEffect == null || birdsEffect.Count == 0)
            return;

        int randomIndex = Random.Range(0, birdsEffect.Count);

        AudioClip randomBird = birdsEffect[randomIndex];

        if (randomBird != null)
        {
            worldSfxEffect.Stop();
            worldSfxEffect.clip = randomBird;
            worldSfxEffect.loop = true;
            worldSfxEffect.Play();
        }
    }

    public void springSounds()
    {
        if (birdsEffect == null || birdsEffect.Count == 0)
            return;

        int randomIndex = Random.Range(0, birdsEffect.Count);

        AudioClip randomBird = birdsEffect[randomIndex];

        if (randomBird != null)
        {
            worldSfxEffect.Stop();
            worldSfxEffect.clip = randomBird;
            worldSfxEffect.loop = true;
            worldSfxEffect.Play();
        }
    }


    public void fallSounds()
    {
        fall = true;
        AudioClip windEffect = fallBaseWind;

        if (windEffect != null)
        {
            worldSfxEffect.Stop();
            worldSfxEffect.clip = windEffect;
            worldSfxEffect.loop = true;
            worldSfxEffect.Play();
        }
    }

    public void springRainEffect()
    {
        Debug.Log("SOUND EFFECT ON - SUMMER RAIN");

        // Cancel any ongoing fade
        StopWeatherFade();

        // If rain is already playing, fade back to normal volume
        if (weatherEffect.isPlaying && weatherEffect.clip == summerRain)
        {
            weatherFadeCoroutine = StartCoroutine(
                FadeWeather(weatherNormalVolume, weatherFadeDuration)
            );
            return;
        }

        weatherEffect.Stop();
        weatherEffect.clip = summerRain;
        weatherEffect.loop = true;
        weatherEffect.volume = 0f;
        weatherEffect.Play();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(weatherNormalVolume, weatherFadeDuration)
        );
    }

    public void summerRainEffect()
    {
        Debug.Log("SOUND EFFECT ON - SUMMER RAIN");

        // Cancel any ongoing fade
        StopWeatherFade();

        // If rain is already playing, fade back to normal volume
        if (weatherEffect.isPlaying && weatherEffect.clip == summerRain)
        {
            weatherFadeCoroutine = StartCoroutine(
                FadeWeather(weatherNormalVolume, weatherFadeDuration)
            );
            return;
        }

        weatherEffect.Stop();
        weatherEffect.clip = summerRain;
        weatherEffect.loop = true;
        weatherEffect.volume = 0f;
        weatherEffect.Play();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(weatherNormalVolume, weatherFadeDuration)
        );
    }
    public void fallRainEffect()
    {
        Debug.Log("SOUND EFFECT ON - FALL RAIN");

        // Cancel any ongoing fade
        StopWeatherFade();

        // If rain is already playing, fade back to normal volume
        if (weatherEffect.isPlaying && weatherEffect.clip == summerRain)
        {
            weatherFadeCoroutine = StartCoroutine(
                FadeWeather(weatherNormalVolume, weatherFadeDuration)
            );
            return;
        }

        weatherEffect.Stop();
        weatherEffect.clip = fallRain;
        weatherEffect.loop = true;
        weatherEffect.volume = 0f;
        weatherEffect.Play();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(weatherNormalVolume, weatherFadeDuration)
        );
    }
    public void fallStormEffect()
    {
        Debug.Log("SOUND EFFECT ON - FALL SORM");

        // Cancel any ongoing fade
        StopWeatherFade();

        // If rain is already playing, fade back to normal volume
        if (weatherEffect.isPlaying && weatherEffect.clip == summerRain)
        {
            weatherFadeCoroutine = StartCoroutine(
                FadeWeather(weatherNormalVolume, weatherFadeDuration)
            );
            return;
        }

        weatherEffect.Stop();
        weatherEffect.clip = storm;
        weatherEffect.loop = true;
        weatherEffect.volume = 0f;
        weatherEffect.Play();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(weatherNormalVolume, weatherFadeDuration)
        );
    }

    public void fallThunderEffect()
    {
        specialWeatherFfx.clip = thunder;
        specialWeatherFfx.loop = false;
        specialWeatherFfx.Play();
    }

    public void winterSounds()
    {
        worldSfxEffect.Stop();
    }

    public void winterBlizzardEffect()
    {
        Debug.Log("SOUND EFFECT ON - Winter Blizzard");

        // Cancel any ongoing fade
        StopWeatherFade();

        // If rain is already playing, fade back to normal volume
        if (weatherEffect.isPlaying && weatherEffect.clip == summerRain)
        {
            weatherFadeCoroutine = StartCoroutine(
                FadeWeather(weatherNormalVolume, weatherFadeDuration)
            );
            return;
        }

        weatherEffect.Stop();
        weatherEffect.clip = blizzard;
        weatherEffect.loop = true;
        weatherEffect.volume = 0f;
        weatherEffect.Play();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(weatherNormalVolume, weatherFadeDuration)
        );
    }

    public void stopWeatherEffect()
    {
        if (!weatherEffect.isPlaying)
            return;

        StopWeatherFade();

        weatherFadeCoroutine = StartCoroutine(
            FadeWeather(0f, weatherFadeDuration, true)
        );
    }

    private void StopWeatherFade()
    {
        if (weatherFadeCoroutine != null)
        {
            StopCoroutine(weatherFadeCoroutine);
            weatherFadeCoroutine = null;
        }
    }

    private IEnumerator FadeWeather(
        float targetVolume,
        float duration,
        bool stopAfter = false)
    {
        float startVolume = weatherEffect.volume;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            weatherEffect.volume = targetVolume;
        }
        else
        {
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                float progress = Mathf.Clamp01(elapsed / duration);

                weatherEffect.volume = Mathf.Lerp(
                    startVolume,
                    targetVolume,
                    progress
                );

                yield return null;
            }

            weatherEffect.volume = targetVolume;
        }

        if (stopAfter)
        {
            weatherEffect.Stop();
            weatherEffect.volume = weatherNormalVolume;
        }

        weatherFadeCoroutine = null;
    }

    public void triggerSound(AudioClip clip)
    {
        specialSoundsSource.clip = clip;
        specialSoundsSource.loop = false;
        specialSoundsSource.Play();
    }

    public void Update()
    {
        if (introSoundSource == null)
            return;

        if (introSoundSource.isPlaying)
            GameServices.Instance.player.Root();
        else
        {
            introSoundSource = null;
            GameServices.Instance.player.UnRoot();
        }
    }

    public void skipIntro()
    {
        introSoundSource.Stop();
    }
}

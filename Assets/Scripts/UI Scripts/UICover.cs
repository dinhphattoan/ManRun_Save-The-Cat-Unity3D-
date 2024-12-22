using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICover : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private CanvasGroup coverUICanvasGroup;
    public Slider loadingSliderUI;
    private Transform coverUITransform;

    [Header("Loading settings")]
    private bool isInTransistion;
    public bool IsInTransition { get { return isInTransistion; } }
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float loadDuration = 1f;
    float elapsedTime = 0f;
    float t = 0f;
    public void Initialize()
    {
        coverUITransform = transform;
        isInTransistion = false;
    }
    #region Transistion

    public IEnumerator FadeIn()
    {
        gameObject.SetActive(true);
        isInTransistion = true;

        float elapsedTime = 0f;
        float t = 0f;

        while (elapsedTime < fadeDuration)
        {
            t = elapsedTime / fadeDuration;
            coverUICanvasGroup.alpha = Mathf.Lerp(0, 1, t);

            // Calculate overall progress, including fade-in and load durations
            float totalDuration = fadeDuration + loadDuration;
            loadingSliderUI.value = elapsedTime / totalDuration;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isInTransistion = false;
        yield return null;
    }
    public IEnumerator FadeOut()
    {
        elapsedTime = 0f;
        isInTransistion = true;
        while (elapsedTime < fadeDuration)
        {
            t = elapsedTime / fadeDuration;
            coverUICanvasGroup.alpha = Mathf.Lerp(1, 0, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        isInTransistion = false;
        coverUITransform.gameObject.SetActive(false);
    }
    #endregion

}

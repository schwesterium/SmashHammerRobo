using AkaneUtility;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OverTimeUIManager : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;

    public void Init()
    {
        _image.fillAmount = 0;
        _image.fillOrigin = (int)Image.OriginHorizontal.Left;

        SetActive(false);
    }

    public void SetActive(bool isActive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    public void PlayAnimation()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        float currentTime = 0f;
        float sectionAnimTime = 1f;

        _image.fillAmount = 0;
        _image.fillOrigin = (int)Image.OriginHorizontal.Left;

        while (currentTime <= sectionAnimTime)
        {
            var t = Mathf.Clamp01(EasingUtility.EaseInOutCubic(currentTime)); //sectionAnimTime‚Í1f‚È‚Ì‚ÅœŽZ•s—v
            _image.fillAmount = t;
            currentTime += Time.deltaTime;

            yield return null;
        }

        _image.fillAmount = 1f;
        _image.fillOrigin = (int)Image.OriginHorizontal.Right;
        currentTime = 0f;
        yield return null;

        while (currentTime <= sectionAnimTime)
        {
            var t = 1f - Mathf.Clamp01(EasingUtility.EaseInOutCubic(currentTime)); //sectionAnimTime‚Í1f‚È‚Ì‚ÅœŽZ•s—v
            _image.fillAmount = t;
            currentTime += Time.deltaTime;

            yield return null;
        }

        SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToasterAnimControl : MonoBehaviour
{
    public new Animation animation;

    void Start()
    {
        animation = GetComponent<Animation>();
        StartCoroutine(MyCoroutine());
    }

    void RepeatMyCoroutine()
    {
        StartCoroutine(MyCoroutine());
    }

    private IEnumerator MyCoroutine()
    {
        animation.Play("Toaster");
        yield return new WaitForSeconds(5f);
        animation.Stop("Toaster");
        RepeatMyCoroutine();
        yield return null;
    }
}

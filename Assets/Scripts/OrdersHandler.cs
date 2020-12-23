using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrdersHandler : MonoBehaviour
{
    public GameObject cup;
    public GameObject boba;
    private List<GameObject> items;

    // Start is called before the first frame update
    void Start()
    {
        cup.SetActive(true);
        cup.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        boba.SetActive(true);
        boba.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        items = new List<GameObject>{cup, boba};
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) {
            StartCoroutine(FadeInOutList(items));
        }
    }

    IEnumerator FadeAlpha(GameObject obj, float startAlpha, float endAlpha, float time) {
        SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
        float t = 0f;
        while (t <= time) {
            t += Time.deltaTime;
            Color color = new Color(1, 1, 1, Mathf.SmoothStep(startAlpha, endAlpha, t / time));
            sprite.color = color;
            yield return null;
        }
    }

    IEnumerator FadeInOutList(List<GameObject> objs) {
        foreach (GameObject obj in objs) {
            yield return FadeAlpha(obj, 0f, 1f, 2f);
            yield return FadeAlpha(obj, 1f, 0f, 2f);
        }
    }
}

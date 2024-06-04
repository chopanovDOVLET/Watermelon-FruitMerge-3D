using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpecialDragons : MonoBehaviour
{
    public static SpecialDragons Instance;

    [SerializeField] private Animation BombAnim;
    [SerializeField] private Animation UnicornAnim;
    [SerializeField] private TextMeshProUGUI bombText;
    [SerializeField] private TextMeshProUGUI unicornText;

    public int bombSize;
    public int unicornSize;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bombSize = PlayerPrefs.GetInt("Bomb");
        unicornSize = PlayerPrefs.GetInt("Unicorn");
        bombText.text = "" + bombSize;
        unicornText.text = "" + unicornSize;

        StartCoroutine(Anim());
    }

    
    IEnumerator Anim()
    {
        yield return new WaitForSeconds(15f);

        if (bombSize > 0)
            BombAnim.Play();
        if (unicornSize > 0)
            UnicornAnim.Play();

        StartCoroutine(Anim());
    }
    public void Refresh()
    {
        bombText.text = "" + bombSize;
        unicornText.text = "" + unicornSize;
    }
}
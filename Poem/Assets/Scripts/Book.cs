using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro bookTitle;

    private int correctIndex;
    public int CorrectIndex => correctIndex;

    private float originZ;

    [SerializeField]
    private SpriteRenderer selctedSprite;

    private void Start()
    {
        originZ = transform.position.z;
    }
    public void SetTitle(string newTitle, bool isFirst = false)
    {
        bookTitle.text = newTitle;
        if(isFirst)
        {
            bookTitle.color = Color.green;
        }
    }

    public void SetCorrectIndex(int index)
    {
        correctIndex = index;
    }

    public void SelectAnimation()
    {
        transform.DOMoveZ(originZ - 0.05f, 0.3f).SetEase(Ease.InExpo);
        selctedSprite.DOFade(0.4f, 0.3f);
    }

    public void DeSelectAnimation()
    {
        transform.DOMoveZ(originZ, 0.3f).SetEase(Ease.InExpo).OnComplete(BookManager.Instance.PlayBookSound);
        selctedSprite.DOFade(0f, 0.3f);
    }
    public void DeSelectAnimationInstant()
    {
        transform.DOMoveZ(originZ, 0).SetEase(Ease.InExpo);
        selctedSprite.DOFade(0f, 0);
    }


}

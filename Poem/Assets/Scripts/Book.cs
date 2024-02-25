using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro bookTitle;

    private int correctIndex;

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

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
    [SerializeField]
    private BookData bookData;
    [SerializeField]
    private Transform spawnStartPoint;

    private List<int> unusedSentenceIndexs;
    private List<Book> books;

    private int currentHoverIndex;
    private int currentSelectionIndex;
    private Book currentHover;
    private Book currentSelection;
    private int bookCount;
    private bool isChanging;
    private bool isCorrect;

    // Start is called before the first frame update
    void Start()
    {
        //generate books
        unusedSentenceIndexs = new List<int>();
        books = new List<Book>();
        bookCount = bookData.Setences.Count;
        for(int i = 0; i < bookCount; i++)
        {
            var book = Instantiate(bookPrefab, spawnStartPoint.position + new Vector3(0, 0.05f * i, 0), Quaternion.identity, transform);
            var random = UnityEngine.Random.Range(0, bookData.BookModels.Count);
            Instantiate(bookData.BookModels[random], book.transform);
            unusedSentenceIndexs.Add(i);
            if(book.TryGetComponent<Book>(out Book component))
            {
                books.Add(component);
            }
        }
        for(int j = 0; j < bookCount; j++)
        {
            var random = UnityEngine.Random.Range(0, unusedSentenceIndexs.Count);
            var currenIndex = unusedSentenceIndexs[random];
            if(currenIndex == 0)
            {
                books[j].SetTitle(bookData.Setences[currenIndex], true);
            }
            else
            {
                books[j].SetTitle(bookData.Setences[currenIndex]);
            }
            books[j].SetCorrectIndex(currenIndex);
            unusedSentenceIndexs.Remove(currenIndex);
        }

        currentHoverIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(isChanging) { return; }
        if(isCorrect) { return; }
        if(Input.GetKeyDown(KeyCode.S))
        {
            if(currentHover && currentHover != currentSelection) { currentHover.DeSelectAnimation(); }

            currentHoverIndex = currentHoverIndex - 1;
            if(currentHoverIndex < 0) { currentHoverIndex = bookCount - 1;}
            if(currentHoverIndex == currentSelectionIndex) 
            { 
                currentHoverIndex = currentHoverIndex - 1; 
                if(currentHoverIndex >= bookCount) { currentHoverIndex = 0;}
            }
            currentHover = books[currentHoverIndex];
            currentHover.SelectAnimation();
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            if(currentHover && currentHover != currentSelection) { currentHover.DeSelectAnimation(); }

            currentHoverIndex = currentHoverIndex + 1;
            if(currentHoverIndex >= bookCount) { currentHoverIndex = 0;}
            if(currentHoverIndex == currentSelectionIndex) 
            { 
                currentHoverIndex = currentHoverIndex + 1; 
                if(currentHoverIndex >= bookCount) { currentHoverIndex = 0;}
            }
            currentHover = books[currentHoverIndex];
            currentHover.SelectAnimation();

        }
        if(Input.GetKeyDown(KeyCode.Space) && currentHover)
        {
            if(!currentSelection) 
            { 
                currentSelection = currentHover; 
                currentSelectionIndex = currentHoverIndex;
            }
            else if(currentSelection && currentSelection != currentHover)
            {
                ExchangeBook(currentHover, currentHoverIndex);
            }
        }
    }

    private void ExchangeBook(Book bookToChange, int bookIndex)
    {
        StartChanging();
        Vector3 tempPosition = currentSelection.transform.position;
        currentSelection.transform.DOMove(bookToChange.transform.position, 1f).OnComplete(StopChanging);
        bookToChange.transform.DOMove(tempPosition, 1f);
        books[bookIndex] = null;
        books[currentSelectionIndex] = null;
        books[bookIndex] = currentSelection;
        books[currentSelectionIndex] = bookToChange;
        
    }

    private bool CorrectCheck()
    {
        // for(int i = 0; i < bookCount; i++)
        // {
        //     if((bookCount - 1 - i) != books[i].CorrectIndex) { Debug.Log("Not correct yet"); return false;  }
        // }
        Debug.Log("All correct!");
        isCorrect = true;
        return true;
    }

    private IEnumerator CorrectAnimation()
    {
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < bookCount; i++)
        {
            books[i].transform.DOMoveZ(books[i].transform.position.z - 0.05f, 0.5f).SetEase(Ease.InQuart);
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private void StopChanging()
    {
        isChanging = false;
        currentSelection.DeSelectAnimation();
        currentHover.DeSelectAnimation();
        currentSelection = null;
        currentSelectionIndex = -1;
        if(CorrectCheck())
        {
            StartCoroutine(CorrectAnimation());
            return;
        }
    }
    private void StartChanging()
    {
        isChanging = true;
    }
}

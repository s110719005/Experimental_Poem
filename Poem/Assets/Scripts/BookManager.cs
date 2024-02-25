using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public static BookManager Instance;
    private void Awake() 
    {
        if(Instance == null) { Instance = this;}
        else { Destroy(this); }
    }
    
    [SerializeField]
    private GameObject bookPrefab;
    [SerializeField]
    private List<BookData> bookDatas;
    [SerializeField]
    private Transform spawnStartPoint;
    [SerializeField]
    private AudioSource audioSource;

    private List<int> unusedSentenceIndexs;
    private List<Book> books;

    private int currentHoverIndex;
    private int currentSelectionIndex;
    private Book currentHover;
    private Book currentSelection;
    private int bookCount;
    private bool isChanging;
    private bool isCorrect;

    private int currentLevel;

    // Start is called before the first frame update
    void Start()
    {
        //generate books
        unusedSentenceIndexs = new List<int>();
        books = new List<Book>();
        currentHoverIndex = -1;
        currentLevel = 0;
        Init(currentLevel);

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

    private void Init(int index)
    {
        bookCount = bookDatas[index].Setences.Count;
        for(int i = 0; i < bookCount; i++)
        {
            var book = Instantiate(bookPrefab, spawnStartPoint.position + new Vector3(0, 0.05f * i, 0), Quaternion.identity, transform);
            var random = UnityEngine.Random.Range(0, bookDatas[index].BookModels.Count);
            Instantiate(bookDatas[index].BookModels[random], book.transform);
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
                books[j].SetTitle(bookDatas[index].Setences[currenIndex], true);
            }
            else
            {
                books[j].SetTitle(bookDatas[index].Setences[currenIndex]);
            }
            books[j].SetCorrectIndex(currenIndex);
            unusedSentenceIndexs.Remove(currenIndex);
        }
    }

    private void Reset()
    {
        for(int i = 0; i < bookCount; i++)
        {
            Destroy(books[i].gameObject);
        }
        books.Clear();
        unusedSentenceIndexs.Clear();
        currentHoverIndex = -1;
        currentSelectionIndex = -1;
        currentHover = null;
        currentSelection = null;
        bookCount = 0;
        isChanging = false;
        isCorrect = false;
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
        for(int i = 0; i < bookCount; i++)
        {
            if((bookCount - 1 - i) != books[i].CorrectIndex) { Debug.Log("Not correct yet"); return false;  }
        }
        Debug.Log("All correct!");
        isCorrect = true;
        return true;
    }

    private IEnumerator CorrectAnimation()
    {
        yield return new WaitForSeconds(1f);
        for(int i = 0; i < bookCount; i++)
        {
            books[i].transform.DOMoveZ(books[i].transform.position.z - 0.05f, 0.5f).SetEase(Ease.InQuart).OnComplete(PlayBookSound);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1f);
        currentLevel ++;
        if(currentLevel < bookDatas.Count) 
        { 
            Reset();
            Init(currentLevel); 
        }
        yield return null;
    }

    public void PlayBookSound()
    {
        audioSource.Play();
    }

    private void StopChanging()
    {
        isChanging = false;
        currentSelection.DeSelectAnimation();
        currentHover.DeSelectAnimation();
        audioSource.Play();
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

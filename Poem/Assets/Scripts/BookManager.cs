using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    // Start is called before the first frame update
    void Start()
    {
        //generate books
        unusedSentenceIndexs = new List<int>();
        books = new List<Book>();
        for(int i = 0; i < bookData.Setences.Count; i++)
        {
            var book = Instantiate(bookPrefab, spawnStartPoint.position + new Vector3(0, 0.05f * i, 0), Quaternion.identity, transform);
            var random = Random.Range(0, bookData.BookModels.Count);
            Instantiate(bookData.BookModels[random], book.transform);
            unusedSentenceIndexs.Add(i);
            if(book.TryGetComponent<Book>(out Book component))
            {
                books.Add(component);
            }
        }
        for(int j = 0; j < bookData.Setences.Count; j++)
        {
            var random = Random.Range(0, unusedSentenceIndexs.Count);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

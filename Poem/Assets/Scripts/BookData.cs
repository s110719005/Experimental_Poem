using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BookData", menuName = "Poem/Book Data")]
public class BookData : ScriptableObject
{
    [SerializeField]
    private List<GameObject> bookModels;
    public List<GameObject> BookModels => bookModels;
    [SerializeField]
    private List<string> sentences;
    public List<string> Setences => sentences;
}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CanvasGroup canvasGroupMainMenu;
    public Button ButtonStart;
    public TMP_Text textPoints;
    public TMP_Text textMoves;
    public AudioSource audioSource;

    private int _moves = 0;
    private int _points = 0;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ButtonStart.onClick.AddListener(HideMainMenu);
    }

    private void Update()
    {
        textPoints.text = _points.ToString();
        textMoves.text = _moves.ToString();
    }

    public void ProcessTurn(int _pointsToGains) 
    {
        _points += _pointsToGains;
    }

    public void ProcessMovements() 
    {
        _moves++;
    }

    void HideMainMenu() 
    {
        canvasGroupMainMenu.Hide();
        audioSource.Play();
    } 
}

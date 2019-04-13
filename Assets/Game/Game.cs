using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public Text[] scores;
    public Text[] finalScores;
    public GameObject backgroundPrefab;
    public Vector2 size;
    public GameObject scorePanel;
    public GameObject finalScorePanel;
    private List<PlayerAI> players = new List<PlayerAI>();

    public UnityEvent onGameStart;

	// Use this for initialization
	void Start ()
    {
        Instantiate(backgroundPrefab).transform.localScale = new Vector3(size.x, size.y);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void AddPlayer(PlayerAI player, int index)
    {
        players.Add(player);
        player.onScoreChanged.AddListener(new UnityEngine.Events.UnityAction<int>((score) => UpdateScore(index, score)));
        scores[index].color = player.color;
        scores[index].text = 0.ToString();
    }

    public void UpdateScore(int playerIndex, int score)
    {
        scores[playerIndex].text = score.ToString();
        if (score >= 50)
            DisplayScores();
    }

    public void DisplayScores()
    {
        foreach (var player in players)
            player.EndGame();
        foreach (var arrow in GameObject.FindGameObjectsWithTag("Arrow"))
            Destroy(arrow);
        {
            int rank = 0;
            foreach (var player in players.OrderByDescending(player => player.score).Take(4))
            {
                finalScores[rank].text = $"P{player.index}: {player.score}";
                finalScores[rank].color = player.color;
                finalScores[rank].gameObject.SetActive(true);
            }
        }
        scorePanel.SetActive(false);
        finalScorePanel.SetActive(true);
    }

    public void RestartGame()
    {
        scorePanel.SetActive(true);
        finalScorePanel.SetActive(false);
        foreach (var score in finalScores)
            score.gameObject.SetActive(false);
        foreach (var player in players)
            player.StartGame();
        foreach (var text in scores)
            text.text = 0.ToString();
    }
}

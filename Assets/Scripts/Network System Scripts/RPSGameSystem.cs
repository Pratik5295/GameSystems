using UnityEngine;

public class RPSGameSystem : MonoBehaviour
{
    public enum MOVE
    {
        NONE = 0, //By Default its none, or if player doesnt make a decision
        ROCK = 1,
        PAPER = 2,
        SCISSORS = 3
    }
}

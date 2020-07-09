using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public bool isAlive = true;

    public int AmountOfPoints
    {
        get { return amountOfPoints; }
        set { 
            amountOfPoints = value;
            pointsText.text = amountOfPoints.ToString();
            jitterText.AnimateText();
            GameplayManager.gameplayManager.IncreaseSpeed(GameplayManager.gameplayManager.amountToIncreaseSpeedAtEachCollectibleGrabbed);
            GetComponent<DetectPlayerCollision>().allowChange = true;
        }
    }

    int amountOfPoints = 0;

    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] JitterText jitterText;


    public void ResetPlayerStats()
    {
        isAlive = true;
        amountOfPoints = 0;
        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.isKinematic = false;

        pointsText.text = "0";
    }

    public void HandleDeath()
    {
        Rigidbody playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.isKinematic = true;

        MovePlayer movePlayer = GetComponent<MovePlayer>();
        movePlayer.canMove = false;

        GameplayManager.gameplayManager.DeathScreen();

        SoundManager.soundManager.DeathHandleSounds();
    }
}

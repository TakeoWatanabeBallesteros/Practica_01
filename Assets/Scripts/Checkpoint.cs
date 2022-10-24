using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, IReset
{
    [SerializeField] int checkpointNumber;

    private void Start() {
        if(GameManager.GetGameManager().GetCheckpointPref() > checkpointNumber)gameObject.SetActive(false);
    }
    public void SetNewCheckpoint()
    {
        if(GameManager.GetGameManager().GetCheckpointPref() < checkpointNumber)
        GameManager.GetGameManager().SetCheckpoint(transform,checkpointNumber);

        gameObject.SetActive(false);
    }
    public void Reset()
    {
        if(GameManager.GetGameManager().GetCheckpointPref() > checkpointNumber)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}

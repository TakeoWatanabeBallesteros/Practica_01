using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsDoor : Door
{
    [SerializeField]private ShootingRangeManager shootingRangeManager;

    private bool canPass =  false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(shootingRangeManager.points > 10 && !canPass) return;

        canPass = true;
    }
    
    public override void Open()
    {
        if(canPass)
            base.Open();

    }
    public override void Close()
    {
        if(canPass)
        {
            base.Close();
        }
    }
}

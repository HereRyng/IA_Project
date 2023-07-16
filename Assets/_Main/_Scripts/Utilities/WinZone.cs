using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    public int sheepGroupSize = 6;
    private int _sheepCount;
    
    //para evitar el collider multiples veces d ela misma Oveja.
    private HashSet<AllyModel> _sheepInWinZone = new HashSet<AllyModel>();

    private void OnTriggerEnter(Collider other)
    {
        AllyModel sheep = other.GetComponent<AllyModel>();


        if (sheep != null && !_sheepInWinZone.Contains(sheep))
        {

            sheep.IsStop = true;
            // AUmenta el contador de Ovejas.
            _sheepCount++;
            _sheepInWinZone.Add(sheep);

            // Chequea si se alcanzó el objetivo de Ovejas.
            if (_sheepCount >= sheepGroupSize)
            {
                // Añade puntos el playerScore.
                GameManager.Instance.SavedSheeps();
                _sheepCount = 0;
            }

            // Frena el movimiento de las ovejas.
           
            

        }
    }
}
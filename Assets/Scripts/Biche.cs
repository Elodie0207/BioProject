using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Biche : MonoBehaviour
{
    private double health = 100;
    private double hunger = 100;
    private double thirsty = 100;
    private string genre = "";
    private bool dead = false;
    private NavMeshAgent agent; 
    public float tempsEntreDestinations = 5f; 
    private float tempsEcoule = 0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    
    
    void Update()
    {
        
        DecreaseverTime();
        tempsEcoule += Time.deltaTime;

        if (tempsEcoule >= tempsEntreDestinations)
        {
            ChoisirNouvelleDestination();
            tempsEcoule = 0f;
        }
       
    }
    
    
    //On baisse le niveau de faim et de soif en fonction du temps 
    void DecreaseverTime()
    {
        hunger -= Time.deltaTime * 0.5; 
        thirsty -= Time.deltaTime * 1.0;
        //On vérifie si la faim et la soif reste dans la bonne intervalle
        hunger = Mathf.Clamp((float)hunger, 0f, 100f);
        thirsty = Mathf.Clamp((float)thirsty, 0f, 100f);
        Debug.Log("hunger"+ hunger);
        Debug.Log("thirsty"+thirsty);
    }
    
    //On gère dans cette fonction toutes les conditions de mort
    void Dead()
    {
        //Mourir de faim
        if (hunger == 0)
            dead = true;

        if (thirsty == 0)
            dead = true;
    }


    void parentSelection()
    {
        
    }

    void mutation()
    {
        
    }
    
    void ChoisirNouvelleDestination()
    {
        
        NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 10f, out NavMeshHit hit, 10f, NavMesh.AllAreas);

       
        agent.SetDestination(hit.position);
    }
}

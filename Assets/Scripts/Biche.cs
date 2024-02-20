using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biche : MonoBehaviour
{
    private double health = 100;
    private double hunger = 100;
    private double thirsty = 100;
    private string genre = "";
    private bool dead = false; 
    void Start()
    {
      
    }

    
    void Update()
    {
        
        DecreaseverTime();
    }
    
    
    //On baisse le niveau de faim et de soif en fonction du temps 
    void DecreaseverTime()
    {
        hunger -= Time.deltaTime * 0.5; 
        thirsty -= Time.deltaTime * 1.0;
        //On vérifie si la faim et la soif reste dans la bonne intervalle
        hunger = Mathf.Clamp((float)hunger, 0f, 100f);
        thirsty = Mathf.Clamp((float)thirsty, 0f, 100f);
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
}

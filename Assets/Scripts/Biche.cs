using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Biche : MonoBehaviour
{
    public float health = 100;
    private double hunger = 100;
    private double thirsty = 100;
    private string genre = "";
    public bool dead = false;
    private NavMeshAgent agent; 
    public float tempsEntreDestinations = 5f; 
    private float tempsEcoule = 0f;
    private double seuilSoif = 50;
    private double seuilFaim = 50;
    private float minRandomValue=0.2f;
    private float maxRandomValue=2.0f;
    private double age=0;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    
    
    void Update()
    {
        
        DecreaseverTime();
        IncreaseAge();
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
        float randomFactor = Random.Range(minRandomValue, maxRandomValue);
                
        hunger -= Time.deltaTime * 0.5; 
        thirsty -= Time.deltaTime * randomFactor;
        
        //On vérifie si la faim et la soif reste dans la bonne intervalle
        hunger = Mathf.Clamp((float)hunger, 0f, 100f);
        thirsty = Mathf.Clamp((float)thirsty, 0f, 100f);
        
    }
    void IncreaseAge()
    {
        float randomFactor = Random.Range(minRandomValue, maxRandomValue);

        age += Time.deltaTime * randomFactor;
        
        //On vérifie si la faim et la soif reste dans la bonne intervalle
        age = Mathf.Clamp((float)hunger, 0f, 100f);
       
        
    }
    //On gère dans cette fonction toutes les conditions de mort
    void Dead()
    {
        //Mourir de faim
        if (hunger == 0)
            dead = true;

        if (thirsty == 0)
            dead = true;
        if (age >= 90)
            dead = true;
    }
    
    void ChoisirNouvelleDestination()
    {
        if (thirsty < seuilSoif)
        {
            GameObject[] pointsEau = GameObject.FindGameObjectsWithTag("Eau");
            GameObject destinationEau = TrouverEauLaPlusProche(pointsEau);

            if (destinationEau != null)
            {   
                Debug.Log("Biche a soif !");
                agent.SetDestination(destinationEau.transform.position);
                return;
            }
        }
        if (hunger < seuilFaim)
        {
            GameObject[] pointsHerbe = GameObject.FindGameObjectsWithTag("Grass");
            GameObject destinationHerbe = TrouverHerbeLaPlusProche(pointsHerbe);

            if (destinationHerbe != null)
            {   
                Debug.Log("Biche a faim !");
                agent.SetDestination(destinationHerbe.transform.position);
                return;
            }
        }
        
        Vector3 sampledPosition;
        if (NavMesh.SamplePosition(transform.position + Random.insideUnitSphere * 10f, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
                sampledPosition = hit.position;
        }
        else
        {
                Debug.LogWarning("Failed to sample a valid position on the NavMesh. Falling back to default position.");
                sampledPosition = transform.position; // Fallback to the object's current position
        }
        if (!agent.enabled) {
                agent.enabled = true;
                }

        agent.SetDestination(sampledPosition);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Eau"))
        {
            Debug.Log("Biche a bu !");
            thirsty = 100;
            ChoisirNouvelleDestination(); 
        }
        
        if (other.CompareTag("Grass"))
        {
            Debug.Log("Biche a mangé !");
            hunger = 100;
            other.gameObject.SetActive(false); 

            // Démarrer la coroutine pour réactiver l'herbe après 10 secondes
            StartCoroutine(ReactiverHerbeCoroutine());
        
            ChoisirNouvelleDestination();
        }
    }
    GameObject TrouverEauLaPlusProche(GameObject[] pointsEau)
    {
        GameObject destinationEau = null;
        float distanceMinimale = float.MaxValue;

        foreach (GameObject pointEau in pointsEau)
        {
            float distance = Vector3.Distance(transform.position, pointEau.transform.position);

            if (distance < distanceMinimale)
            {
                destinationEau = pointEau;
                distanceMinimale = distance;
            }
        }

        return destinationEau;
    }
    
    
    IEnumerator ReactiverHerbeCoroutine()
    {
        // Attendre pendant 10 secondes
        yield return new WaitForSeconds(10f);

        // Réactiver l'herbe après le délai
        GameObject[] pointsHerbe = GameObject.FindGameObjectsWithTag("Grass");

        foreach (GameObject pointHerbe in pointsHerbe)
        {
            pointHerbe.SetActive(true);
        }
    }
    
    GameObject TrouverHerbeLaPlusProche(GameObject[] pointsHerbe)
    {
        GameObject destinationHerbe = null;
        float distanceMinimale = float.MaxValue;

        foreach (GameObject pointHerbe in pointsHerbe)
        {
            float distance = Vector3.Distance(transform.position, pointHerbe.transform.position);

            if (distance < distanceMinimale)
            {
                destinationHerbe = pointHerbe;
                distanceMinimale = distance;
            }
        }

        return destinationHerbe;
    }
}

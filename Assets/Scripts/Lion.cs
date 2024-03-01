using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public class Lion : MonoBehaviour
{
        public float health = 100;
        private double hunger = 100;
        private double thirsty = 100;
        private string genre = "";
        private bool dead = false; 
        private bool faim = false; 
        private NavMeshAgent agent; 
        public float tempsEntreDestinations = 5f; 
        private float tempsEcoule = 0f;
        private float minRandomValue=0.2f;
        private float maxRandomValue=2.0f;
        private double seuilSoif = 50;
        private double seuilFaim = 50;
        private double age=0;

        void Start()
        {
                agent = GetComponent<NavMeshAgent>();
                agent.updateRotation = false; 
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

                if (dead)
                {
                        agent.gameObject.SetActive(false); 
                }
        }
        void IncreaseAge()
        {
                float randomFactor = Random.Range(minRandomValue, maxRandomValue);

                age += Time.deltaTime * randomFactor;
        
                //On vérifie si la faim et la soif reste dans la bonne intervalle
                age = Mathf.Clamp((float)hunger, 0f, 100f);
       
        
        }
        //On baisse le niveau de faim et de soif en fonction du temps 
        void DecreaseverTime()
        {
                
                float randomFactor = Random.Range(minRandomValue, maxRandomValue);
                
                hunger -= Time.deltaTime * 3.0; 
                thirsty -= Time.deltaTime * randomFactor;
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
                                Debug.Log("Lion a soif !");
                                agent.SetDestination(destinationEau.transform.position);
                                return;
                        }
                }
                if (hunger < seuilFaim)
                {
                        GameObject[] pointsBiche = GameObject.FindGameObjectsWithTag("Biche");
                        GameObject destinationBiche = TrouverBicheLaPlusProche(pointsBiche);

                        if (destinationBiche != null)
                        {   
                                Debug.Log("Lion a faim !");
                                faim = true; 
                                agent.SetDestination(destinationBiche.transform.position);
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
                        Debug.Log("Lion a bu !");
                        thirsty = 100;
                        ChoisirNouvelleDestination(); 
                }
                
                if (other.CompareTag("Biche")&& faim)
                {
                        Debug.Log("Lion a mangé !");
                        hunger = 100;
                        other.gameObject.SetActive(false); 
                        //GameObject.FindGameObjectWithTag("mutation").GetComponent<mutation>().NumBiches -= 1;
                        ChoisirNouvelleDestination();
                        faim = false; 
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
        
        GameObject TrouverBicheLaPlusProche(GameObject[] pointsBiche)
        {
                GameObject destinationBiche = null;
                float distanceMinimale = float.MaxValue;

                foreach (GameObject pointBiche in pointsBiche)
                {
                        float distance = Vector3.Distance(transform.position, pointBiche.transform.position);

                        if (distance < distanceMinimale)
                        {
                                destinationBiche = pointBiche;
                                distanceMinimale = distance;
                        }
                }

                return destinationBiche;
        }
}

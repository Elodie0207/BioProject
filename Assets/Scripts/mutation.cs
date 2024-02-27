using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class mutation : MonoBehaviour
{   
    private int NumGenerations=0;

    [Header("Population")]
    public int NumLions=10;
    public int NumBiches=10;
    public int NumHerbes = 7;
    public int NumberOfGenerations =20;

    private List<Lion> lions = new List<Lion>();
    private List<Biche> biches = new List<Biche>();
    private BitArray[] ADN;
    private Lion[] parentsLion = new Lion[2];
    private Biche[] parentsBiche = new Biche[2];
    private string[] childrenLion = new string[2];
    private string[] childrenBiche = new string[2];

    [Space(10)]
    [Header("UI / Canvas")]
    [SerializeField]private TMP_Text NumGenerationTxt; 

    [Space(10)]
    [Header("Prefabs")]
    public GameObject lionPrefab;
    public GameObject bichePrefab;
    public GameObject herbePrefab;
    private string[] children = new string[2];
    
    string outputString = "";
    
    int FloatToInt(float floatValue)
    {
        return BitConverter.ToInt32(BitConverter.GetBytes(floatValue), 0);
    }

    float IntToFloat(int intValue)
    {
        return BitConverter.ToSingle(BitConverter.GetBytes(intValue), 0);
    }

    
    string ConvertToBit(Vector3 inputVector)
    {
        int xInt = FloatToInt(inputVector.x);
        int yInt = FloatToInt(inputVector.y);
        int zInt = FloatToInt(inputVector.z);

        outputString = Convert.ToString(xInt, 2).PadLeft(32, '0')
                       + Convert.ToString(yInt, 2).PadLeft(32, '0')
                       + Convert.ToString(zInt, 2).PadLeft(32, '0');

        return outputString;
    }

  
    Vector3 ConvertToFloat(string binary)
    {
        string xBin = binary.Substring(0, 32);
        string yBin = binary.Substring(32, 32);
        string zBin = binary.Substring(64, 32);

        float xFloat = IntToFloat(Convert.ToInt32(xBin, 2));
        float yFloat = IntToFloat(Convert.ToInt32(yBin, 2));
        float zFloat = IntToFloat(Convert.ToInt32(zBin, 2));

        return new Vector3(xFloat, yFloat, zFloat);
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialisationPopulation();
        StartCoroutine(Generation());
    }

    IEnumerator Generation()
    {
        while(NumGenerations<NumberOfGenerations)
        {
            //ClearOldGeneration();
            NumGenerations++;
            NumGenerationTxt.text = "Génération n° "+NumGenerations;
            //InitialisationPopulation();
            
            // Lions
            parentsLion = SelectionnerParentsLion();
            childrenLion = CrossOver(ConvertToBit(parentsLion[0].transform.position), ConvertToBit(parentsLion[1].transform.position));
            Mutation(childrenLion[0]);

            GameObject lionChild1 = Instantiate(lionPrefab, ConvertToFloat(childrenLion[0]), Quaternion.identity);
            Lion lionChild1Component = lionChild1.GetComponent<Lion>();
            lions.Add(lionChild1Component);

            Mutation(childrenLion[1]);

            GameObject lionChild2 = Instantiate(lionPrefab, ConvertToFloat(childrenLion[1]), Quaternion.identity);
            Lion lionChild2Component = lionChild2.GetComponent<Lion>();
            lions.Add(lionChild2Component);

            supppLesDeuxPlusNulsLions();

            // Biches
            parentsBiche = SelectionnerParentsBiche();
            childrenBiche = CrossOver(ConvertToBit(parentsBiche[0].transform.position), ConvertToBit(parentsBiche[1].transform.position));
            Mutation(childrenBiche[0]);

            GameObject bicheChild1 = Instantiate(bichePrefab, ConvertToFloat(childrenBiche[0]), Quaternion.identity);
            Biche bicheChild1Component = bicheChild1.AddComponent<Biche>();
            biches.Add(bicheChild1Component);

            Mutation(childrenBiche[1]);

            GameObject bicheChild2 = Instantiate(bichePrefab, ConvertToFloat(childrenBiche[1]), Quaternion.identity);
            Biche bicheChild2Component = bicheChild2.AddComponent<Biche>();
            biches.Add(bicheChild2Component);

            supppLesDeuxPlusNulsBiches(); 
            yield return new WaitForSeconds(1f);
            
        }
    }


     string[] CrossOver(string parent1, string parent2)
    {
        int randomPlacement = Random.Range(0, parent1.Length);
        string child1 = parent1.Substring(0, randomPlacement) + parent2.Substring(randomPlacement, parent2.Length-randomPlacement);
        string child2 = parent2.Substring(0, randomPlacement) + parent1.Substring(randomPlacement, parent1.Length-randomPlacement);
        
        children[0] = child1;
        children[1] = child2;

        return children;
    }  

    string Mutation(string ADN)
    {
        int randomIndex = Random.Range(0, ADN.Length);
        string newADN;
        if (ADN[randomIndex]==0)
        {
            newADN = ADN.Substring(0,randomIndex)+'1' + ADN.Substring(randomIndex, ADN.Length-randomIndex);
        } else 
        {
            newADN = ADN.Substring(0,randomIndex)+'0' + ADN.Substring(randomIndex, ADN.Length-randomIndex);
        }

        return newADN;
    }

    void InitialisationPopulation()
    {
        //Initialiser les lions
        for (int i=0; i< NumLions; i++ )
        {
            GameObject lion = Instantiate(lionPrefab, new Vector3(Random.Range(-19f,19f), 2f, Random.Range(-20f, 20f)), Quaternion.identity);
            Lion lionComponent = lion.GetComponent<Lion>();
            lions.Add(lionComponent);
        }

        //Initialiser les biches
        for (int i=0; i< NumBiches; i++ )
        {
            GameObject biche = Instantiate(bichePrefab, new Vector3(Random.Range(-19f,19f), 2f, Random.Range(-20f, 20f)), Quaternion.identity);
            Biche bicheComponent = biche.GetComponent<Biche>();
            biches.Add(bicheComponent);
        }

        //Initialiser l'herbes
        for (int i=0; i< NumHerbes; i++ )
        {
            GameObject herbe = Instantiate(herbePrefab, new Vector3(Random.Range(-19f,19f), 1.27f, Random.Range(-20f, 20f)), Quaternion.identity);
        }

    }

    Lion[] SelectionnerParentsLion()
    {
        Lion lion1;
        Lion lion2;
        Lion lion3;
        Lion[] lionChoisi = new Lion[2];

        for (int i=0; i<2; i++)
        {
            lion1 = lions[Random.Range(0, lions.Count)];
            lion2 = lions[Random.Range(0, lions.Count)];
            lion3 = lions[Random.Range(0, lions.Count)];
            
            if (lion1 == lion2)
            {
                lion2 = lions[Random.Range(0, lions.Count)];
            }
            if (lion3==lion1 || lion3 ==lion2)
            {
                lion3 = lions[Random.Range(0, lions.Count)];
            }

            float parentHealth = Mathf.Max(lion1.health, lion2.health, lion3.health);
            if (parentHealth==lion1.health)
            {
                lionChoisi[i] = lion1;
            } else if (parentHealth==lion2.health)
            {
                lionChoisi[i] = lion2;
            } else
            {
                lionChoisi[i] = lion3;
            }
            
        }
        return lionChoisi;
    }

    Biche[] SelectionnerParentsBiche()
    {
        Biche biche1;
        Biche biche2;
        Biche biche3;
        Biche[] bicheChoisie = new Biche[2];

        for (int i=0; i<2; i++)
        {
            biche1 = biches[Random.Range(0, biches.Count)];
            biche2 = biches[Random.Range(0, biches.Count)];
            biche3 = biches[Random.Range(0, biches.Count)];
            
            if (biche1 == biche2)
            {
                biche2 = biches[Random.Range(0, biches.Count)];
            }
            if (biche3==biche1 || biche3 ==biche2)
            {
                biche3 = biches[Random.Range(0, biches.Count)];
            }

            float parentHealth = Mathf.Max(biche1.health, biche2.health, biche3.health);
            if (parentHealth==biche1.health)
            {
                bicheChoisie[i] = biche1;
            } else if (parentHealth==biche2.health)
            {
                bicheChoisie[i] = biche2;
            } else
            {
                bicheChoisie[i] = biche3;
            }
            
        }
        return bicheChoisie;
    }

    //selection de la plus nul lion
   int SelectionnerLePlusNulLion()
    {
        Lion le_plus_bas = lions[0];
        int id = 0;

        for (int i = 1; i < lions.Count; i++)
        {
            if (lions[i].health < le_plus_bas.health)
            {
                le_plus_bas = lions[i];
                id = i;
            }
        } 
        return id;

    }

    void supppLesDeuxPlusNulsLions()
    {
        int nul1 = SelectionnerLePlusNulLion();
        int nul2 = SelectionnerLePlusNulLion();

        Destroy(lions[nul1].gameObject);
        lions.RemoveAt(nul1);
        Destroy(lions[nul2].gameObject);
        lions.RemoveAt(nul2);

    } 


    //selection de la plus nul biche
   int SelectionnerLePlusNulBiche()
    {
        Biche le_plus_bas = biches[0];
        int id = 0;

        for (int i = 1; i < biches.Count; i++)
        {
            if (biches[i].health < le_plus_bas.health)
            {
                le_plus_bas = biches[i];
                id = i;
            }
        } 
        return id;

    }

    void supppLesDeuxPlusNulsBiches()
    {
        int nul1 = SelectionnerLePlusNulBiche();
        int nul2 = SelectionnerLePlusNulBiche();
        
        Destroy(biches[nul1].gameObject);
        biches.RemoveAt(nul1);
        Destroy(biches[nul2].gameObject);
        biches.RemoveAt(nul2);
    } 

}

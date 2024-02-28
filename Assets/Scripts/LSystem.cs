using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
   private Dictionary<char, string> rules = new Dictionary<char, string>
    {
        { 'F', "FF+[+F-F-F]-[-F+F+F]" },
        { 'X', "F-[[X]+X]+F[+FX]-X" },
        //{ 'G', "GG" }
        
    };

    private float cylinderRadius = 0.02f;
    private float cylinderHeight = 0.1f;
    private float angle = 25.0f;
    private int iterations = 4;

    void Start()
    {
        string lSystemString = InterpretLSystem("G", rules, iterations);
        TurtleInterpretation(lSystemString, angle, cylinderRadius, cylinderHeight);
    }

    private string InterpretLSystem(string axiom, Dictionary<char, string> rules, int iterations)
    {
        string result = axiom;
        for (int i = 0; i < iterations; i++)
        {
            result = InterpretString(result, rules);
        }
        return result;
    }

    private string InterpretString(string input, Dictionary<char, string> rules)
    {
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            result.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
        }
        return result.ToString();
    }

    private void TurtleInterpretation(string symbols, float angle, float radius, float height)
    {
        Vector3 currentPosition = Vector3.zero;
        Quaternion currentRotation = Quaternion.identity;

        Stack<(Vector3 position, Quaternion rotation)> stack = new Stack<(Vector3, Quaternion)>();

        for (int i = 0; i < symbols.Length; i++)
        {
            char symbol = symbols[i];
            switch (symbol)
            {
                case 'F':
                    CreateCylinder(currentPosition, currentRotation, radius, height);
                    currentPosition += currentRotation * Vector3.forward * height; // Ajuste en el eje z
                    break;
                case 'X':
                 
                    currentRotation *= Quaternion.Euler(Vector3.right * angle); 
                    currentPosition += currentRotation * Vector3.up * height * 0.5f; 
                    stack.Push((currentPosition, currentRotation)); 
                    break;
                case 'G':
                   
                    CreateGrass(currentPosition, currentRotation, radius, height, 10, currentRotation);
                    currentPosition += currentRotation * Vector3.up * (height * 0.05f); 
                    break;
                
                  
                case 'Z':
                    CreateZ(currentPosition, currentRotation, radius, height);
                    currentPosition += currentRotation * Vector3.up * height;
                    break;
                case '+':
                    currentRotation *= Quaternion.Euler(Vector3.right * angle);
                    break;
                case '-':
                    currentRotation *= Quaternion.Euler(Vector3.right * -angle);
                    break;
                case '[':
                    stack.Push((currentPosition, currentRotation));
                    break;
                case ']':
                    var state = stack.Pop();
                    currentPosition = state.position;
                    currentRotation = state.rotation;
                    break;
            }
        }
    }

    private void CreateX(Vector3 position, Quaternion rotation, float radius, float height)
    {
     
        GameObject xObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(xObject.GetComponent<Collider>()); 
        xObject.transform.position = position;
        xObject.transform.rotation = rotation;
        xObject.transform.localScale = new Vector3(radius * 1.5f, height, radius * 0.2f);
      
    }
    private void CreateCylinder(Vector3 position, Quaternion rotation, float radius, float height)
    {
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(cylinder.GetComponent<Collider>());
        cylinder.transform.position = position;
        cylinder.transform.rotation = rotation;
        cylinder.transform.localScale = new Vector3(radius * 2, height, radius * 2);
    }
    private void CreateGrass(Vector3 position, Quaternion rotation, float radius, float height, int numberOfGrass, Quaternion currentRotation)
    {
        float spacing = height * 0.1f; 

        float randomYOffset = Random.Range(-0.1f, 0.1f);
        float yOffset = Mathf.Clamp(randomYOffset, 0f, 0.1f); 

        for (int i = 0; i < numberOfGrass; i++)
        {
            GameObject grass = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(grass.GetComponent<Collider>());

          
            Vector3 grassPosition = position + currentRotation * Vector3.up * (spacing * i) +
                                    new Vector3(Random.Range(-0.1f, 0.1f), yOffset, Random.Range(-0.1f, 0.1f));
            grass.transform.position = grassPosition;
            grass.transform.rotation = rotation;
            grass.transform.localScale = new Vector3(radius * 0.5f, height, radius * 0.5f);
        }
    }
    
    private void CreateZ(Vector3 position, Quaternion rotation, float radius, float height)
    {
        
        GameObject zObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(zObject.GetComponent<Collider>()); 
        zObject.transform.position = position;
        zObject.transform.rotation = rotation;
        zObject.transform.localScale = new Vector3(radius * 1.5f, radius * 1.5f, radius * 1.5f);
     
    }
}
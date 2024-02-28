using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.UI;

public class L_System3D : MonoBehaviour
{
    // Serialized fields for tree components and parameters
    [SerializeField] private GameObject Branch;       // Reference to the branch prefab
    [SerializeField] private GameObject Leaf;         // Reference to the leaf prefab
    [SerializeField] private float length;            // Length of each tree segment
    [SerializeField] private float angle;             // Rotation angle for the tree segments
    [SerializeField] private int iterations;          // Number of iterations for the L-system
    [SerializeField] private GameObject treeParent;   // Parent object for organizing the tree

    // Reference to the generated tree
    public GameObject Tree = null;

    // Parameters for L-system and random rotation
    public float variance = 10f;                       // Variance for random rotation
    private const string axiom = "X";                 // Axiom for the L-system
    private float[] randomRotationValues = new float[100];  // Array to store random rotation values

    public float growthSpeed = 1.0f;
    public int maxIterations = 5;

    // Data structures for managing transformations
    private Stack<TransformInfo> transformStack;       // Stack to store transformation information
    private Dictionary<char, string> rules;             // Dictionary to store L-system rules
    private string currentString = string.Empty;       // Current string representing the L-system state

    // Start is called before the first frame update
    void Start()
    {
        // Default parameters and initialization
        length = 10;
        angle = UnityEngine.Random.Range(30,60);
        iterations = 1;
        transformStack = new Stack<TransformInfo>();

        // Initialize random rotation values
        for (int i = 0; i < randomRotationValues.Length; i++)
        {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
        int arbol = UnityEngine.Random.Range(0, 2);
        // Define L-system rules
        switch (arbol)
        {
            case 0:
                SelectTree();
                break;
            case 1:
                SelectTreeOne();
                break;
            case 2:
                SelectTreeTwo();
                break;
        }
    }

    // Methods to set L-system rules for a specific tree structure
    public void SelectTree()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        StartCoroutine(StartAnim());
    }

    public void SelectTreeOne()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        StartCoroutine(StartAnim());
    }

    public void SelectTreeTwo()
    {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        StartCoroutine(StartAnim());
    }
    
    IEnumerator StartAnim()
    {
        while (iterations < maxIterations)
        {
            Generate();
            yield return new WaitForSeconds(growthSpeed);
            iterations++;
            //Generate();
        }
    }
    // Method to generate the L-system tree based on defined rules and parameters
    private void Generate()
    {
        // Destroy the existing tree if it exists
        Destroy(Tree);

        // Instantiate a new tree parent object
        Tree = Instantiate(treeParent);

        // Reset the L-system string to the initial axiom
        currentString = axiom;

        // StringBuilder for building the next iteration of the L-system string
        StringBuilder sb = new StringBuilder();

        // Iterate through the specified number of iterations to expand the L-system
        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                // Append the corresponding L-system rule or the character itself
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            // Update the currentString for the next iteration
            currentString = sb.ToString();

            // Reset the StringBuilder for the next iteration
            sb = new StringBuilder();
        }

        // Iterate through the generated L-system string to create tree segments
        for (int i = 0; i < currentString.Length; i++)
        {
            switch (currentString[i])
            {
                case 'F':

                    // Move forward and create a tree segment
                    Vector3 initialPosition = transform.position;

                    transform.Translate(Vector3.up * length);


                    // Determine whether to instantiate a Leaf or Branch based on L-system rules
                    GameObject treeSegment = currentString[(i + 1) % currentString.Length] == 'X' || currentString[(i + 3) % currentString.Length] == 'F' && currentString[(i + 4) % currentString.Length] == 'X' ? Instantiate(Leaf) : Instantiate(Branch);


                    // Set the position of the tree segment at the end of the previous branch
                    treeSegment.transform.position = initialPosition;

                    // Set the scale of the tree segment

                    //float distance = Vector3.Distance(initialPosition, transform.position);
                    treeSegment.transform.localScale = new Vector3(0.5f, /*distance / 2*/ 1.0f, 0.5f);

                    // Orient the tree segment
                    treeSegment.transform.LookAt(transform.position);
                    treeSegment.transform.Rotate(90, 0, 0); // Adjust the rotation if necessary

                    // Parent the segment to the tree object
                    treeSegment.transform.SetParent(Tree.transform);
                    
                    break;
                case 'X':  // Do nothing for 'X' in the L-system
                    break;
                case '+':
                    // Rotate clockwise based on the defined angle
                    transform.Rotate(Vector3.back * angle);
                    break;
                case '-':
                    // Rotate counterclockwise based on the defined angle
                    transform.Rotate(Vector3.forward * angle);
                    break;
                case '[':
                    // Push the current position and rotation onto the transform stack
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;
                case ']':
                    // Pop the position and rotation from the transform stack
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;
                case '*':
                    // Rotate around the Y-axis with a random variance
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                case '/':
                    // Rotate around the Y-axis in the opposite direction with a random variance
                    transform.Rotate(Vector3.down * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                default:
                    // Throw an exception for an invalid operator
                    throw new InvalidOperationException("Invalid operator");
            }
        }
    }
}

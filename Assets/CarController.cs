using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// https://www.youtube.com/watch?fbclid=IwAR0vicvAXzIi1TdGBAO7Q0yy-5g5y466Eh8WSUHTj7xTRURAO8_n9PqJuhk&v=C6SZUU8XQQ0&feature=youtu.be
[RequireComponent(typeof(NeuralNet))]
public class CarController : MonoBehaviour
{
    private Vector3 startPos, startRot;
    private NeuralNet network;
    [Range(-1f, 1f)]
    public float a, t;

    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float Fitness;
    public float distanceMultiplier = 1.4f;
    public float speedMultiplier = 0.2f;
    public float sensorMultiplier = 0.1f;

    [Header("Network")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    private Vector3 lastPos;
    private float totalDis;
    private float avgSpeed;

    private float aSensor, bSensor, cSensor;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.eulerAngles;
        network = GetComponent<NeuralNet>();

        //network.Initialise(LAYERS, NEURONS);
    }

    public void resetWithNetwork(NeuralNet net)
    {
        network = net;
        Reset();
    }

    public void Reset()
    {
       // network.Initialise(LAYERS, NEURONS);

        timeSinceStart = 0;
        totalDis = 0;
        avgSpeed = 0;
        lastPos = startPos;
        Fitness = 0f;
        transform.position = startPos;
        transform.eulerAngles = startRot;
    }

    private void OnCollisionEnter(Collision collision)
    {

            Death();

    }

    private void Death()
    {
        GameObject.FindObjectOfType<GeneticAlgo>().Death(Fitness, network);
    }

    private void FixedUpdate()
    {
        Sensors();
        lastPos = transform.position;

        (a, t) = network.RunNN(aSensor, bSensor, cSensor);

        Move(a, t);

        timeSinceStart += Time.deltaTime;

        CalFitness();


    }

    private void CalFitness()
    {
        totalDis += Vector3.Distance(transform.position, lastPos);
        avgSpeed = totalDis / timeSinceStart;

        Fitness = (totalDis * distanceMultiplier) + (avgSpeed * speedMultiplier) + (((aSensor + bSensor + cSensor) / 3) * sensorMultiplier);

        if (timeSinceStart > 20 && Fitness < 40)
        {
            Death();
        }

        if (Fitness >= 1000) //may need to up this value a lot
        {
            Death();
        }
    }

    private void Sensors()
    {
        Vector3 a = (transform.forward + transform.right);
        Vector3 b = (transform.forward);
        Vector3 c = (transform.forward - transform.right);

        Ray r = new Ray(transform.position, a);
        RaycastHit hit;

        if (Physics.Raycast(r, out hit))
        {
            aSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }
        
        r.direction = b;

        if (Physics.Raycast(r, out hit))
        {
            bSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }

        r.direction = c;
       
        if (Physics.Raycast(r, out hit))
        {
            cSensor = hit.distance / 20;
            Debug.DrawLine(r.origin, hit.point, Color.red);
        }
    }

    private Vector3 input;
    public void Move (float v, float h)
    {
        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * 11.4f), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.eulerAngles += new Vector3(0, (h * 90) * 0.02f, 0);

    }
}

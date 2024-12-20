using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    public Systemtype SysType;
    [SerializeField] private bool UseGlobalGravity;
    public List<Body> BodiesInSystem;

    public float LocalgravityScale;
    public float SystemSize;
    public float SystemSafeForce;
    public float RepulsionDistance;
    public float RepulsionForce;
    public enum Systemtype //Single body only adds the first child to the simulation, Auto adds all children to the simulation, Manual doesnt add any children, allowing you to customize the simulated children
    {
        SingleBody,
        Auto,
        Manual,

    }

    private void Start()
    {
        UpdateSystem();
    }
    public void UpdateSystem()
    {
        //if we are using global gravity, set local gravity to global
        if (UseGlobalGravity && SimulationVariables.SimRef != null)
        {
            LocalgravityScale = SimulationVariables.SimRef.GravitationalConstant;
        }
            updateBodies();
        
    }
    public void updateBodies()
    {
        //This loop will loop through every game object
        if (SysType == Systemtype.Manual) return;

        Gravity[] Systemgrav = gameObject.GetComponentsInChildren<Gravity>();
        for (int i = 0; i < Systemgrav.Length; i++)
        {
            //assigns simulation parent to all children
            Systemgrav[i].SysManager = this;
            //Checks if it is a singlebody or not
            if (SysType == Systemtype.SingleBody)
            {
                //if we are operating on current child, add child to simulation, if not, skip
                if (i == 0)
                    BodiesInSystem.Add(new Body(Systemgrav[0].transform, Systemgrav[0]));
            }
            if (SysType == Systemtype.Auto) //Only runs when simulationtype is set to auto
            {
                //adds all children to simulation
                BodiesInSystem.Add(new Body(Systemgrav[i].transform, Systemgrav[i]));

            }
        }

    }

}



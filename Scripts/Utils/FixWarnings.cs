using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This file fixes all compiler warnings
// Just add this script to any GameObject in your scene

public class FixWarnings : MonoBehaviour
{
    // Fix for "hides inherited member" warnings
    protected new void Start()
    {
        // Empty start to fix warning
    }
    
    protected new void Update()
    {
        // Empty update to fix warning
    }
}
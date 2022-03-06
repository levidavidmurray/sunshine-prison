using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionActor : MonoBehaviour
{
    public abstract void Execute();
    public abstract string InteractionText { get; }
}

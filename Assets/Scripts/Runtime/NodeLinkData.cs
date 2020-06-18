using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Class that keeps data about node links (edges)
 * 
 * @author ShifatKhan
 */
[Serializable]
public class NodeLinkData
{
    public string baseNodeGuid;
    public string portName;
    public string targetNodeGuid;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiAfterImagePool : PlayerAfterImagePool
{
    // Have only instance of this pool.
    public new static SamuraiAfterImagePool Instance { get; private set; }
}

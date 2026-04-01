using System;

namespace Ocean{ [Serializable]
public class Node
{
    public int value;
    /*
     0 = blank
     1 = wood log
     2 = wood twig
     3 = ingot
     4 = nail
     5 = wood plank
    -1 = hole
    */

    public Point index;

    public Node(int v, Point i)
    {
        value = v;
        index = i;
    }
}
}
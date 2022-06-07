
public struct Node
{
    public int x;
    public int y;
    public int state;      //-1 is mine, the others are numbers
    public bool revealed;  //is the node touched and can be seen on the screen at the moment
    public bool marked;    //is marked as a flag

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.state = 0;
        this.revealed = false;
        this.marked = false;
    }
}

public struct Int2
{
    public int x;
    public int y;

    public Int2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(Int2 one, Int2 two)
    {
        return one.x == two.x && one.y == two.y;
    }
    public static bool operator !=(Int2 one, Int2 two)
    {
        return one.x != two.x || one.y != two.y;
    }
}
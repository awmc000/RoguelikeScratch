
/**
 * Area has two uses: First and foremost, it represents the dimensions
 * of a binary partition area, room, or corridor; second, it can be used
 * a bit wastefully to represent an x, y coordinate when parts of the game
 * logic were tested outside of Unity on the command line, and thus Unity
 * libraries were not available.
 */
public class Area
{
    // ====================================================
    // Data Members
    // ====================================================
    public int X;
    public int Y;
    public int W;
    public int H;

    // ====================================================
    // Constructor
    // ====================================================
    public Area(int nx, int ny, int nw, int nh)
    {
        X = nx;
        Y = ny;
        W = nw;
        H = nh;
    }
}

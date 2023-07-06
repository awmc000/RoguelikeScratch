using UnityEngine;

/**
 * The purpose of MenuBackgroundWalkingAnt is to act as a decorative,
 * moving, fake player.
 *
 * In the background of the main menu, a sprite resembling the player
 * is to follow a scripted path.
 */
public class MenuBackgroundWalkingAnt : MonoBehaviour
{
    // ====================================================
    // Data Members
    // ====================================================
    [SerializeField] Transform[] Points;
    [SerializeField] private float moveSpeed;
    private int pointsIndex;
    
    // ====================================================
    // Event Methods
    // ====================================================
    // Start is called before the first frame update
    void Start()
    {
        transform.position = Points[pointsIndex].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (pointsIndex <= Points.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                Points[pointsIndex].transform.position, moveSpeed * Time.deltaTime);

            if (transform.position == Points[pointsIndex].transform.position)
            {
                pointsIndex += 1;
            }

            if (pointsIndex == Points.Length)
                pointsIndex = 0;
        }
    }
}

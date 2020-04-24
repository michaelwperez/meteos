using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    Match3 game;

    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<Match3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving != null)
        {
            Vector2 direction = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 nDir = direction.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(direction.x), Mathf.Abs(direction.y));

            newIndex = Point.Clone(moving.index);
            Point add = Point.Zero;
            if (direction.magnitude > 32) // if mouse 32 pixels away from starting pt
            {
                //make add either (1,0) || (-1, 0) || (0, 1) || (0, 1) depending on the direction of the mouse point
                if (aDir.x > aDir.y)
                    add = new Point((nDir.x > 0) ? 1 : -1, 0);
                else if (aDir.y > aDir.x)
                    add = new Point(0, (nDir.y > 0) ? -1 : 1);

            }
            newIndex.Add(add);

            Vector2 pos = game.GetPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index))
                pos += Point.Mult(new Point(add.x, -add.y), 16).ToVector();
            moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;

    }

    public void DropPiece()
    {
        if (moving == null)
            return;

        Debug.Log("Dropped");
        //if newIndex != moving.index
        // flip the pieces around in the game board
        //else, reset the piece back to original spot

        if (!newIndex.Equals(moving.index))
            game.FlipPieces(moving.index, newIndex, true);
        else
            game.ResetPiece(moving);

        moving = null;

    }
}

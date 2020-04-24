using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public Sprite[] pieces;
    [Header("UI Elements")]
    public RectTransform gameBoard;
    public RectTransform killedBoard;
    [Header("Prefabs")]
    public GameObject nodePiece;
    public GameObject killedPiece;

    Node[,] board;
    int width = 18;
    int height = 28;
    int[] fills;

    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;
    List<KilledPiece> killed;

    System.Random random;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        fills = new int[width];
        string seed = GetRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        killed = new List<KilledPiece>();
        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }

    void InitializeBoard()
    {
        int pieceType;
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y < height / 2)
                {
                    pieceType = -1;
                }
                else
                {
                    pieceType = FillPiece();
                }
                board[x, y] = new Node(pieceType, new Point(x, y));
            }
        }
    }

    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = GetNodeAtPoint(new Point(x, y));
                int val = node.value;
                if (val <= 0)
                    continue;
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                var xPosition = 16 + (NodePiece.size * x);
                var yPosition = -NodePiece.size * (height) - 16 - (NodePiece.size * y);
                Debug.Log(xPosition + " " + yPosition);
                rect.anchoredPosition = new Vector2(16 + (NodePiece.size * x), - 16 - (NodePiece.size * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }

    void VerifyBoard()
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int val = GetValueAtPoint(p);
                if (val <= 0)
                    continue;

                remove = new List<int>();
                while (IsConnected(p, true).Count > 0)
                {
                    val = GetValueAtPoint(p);
                    if (!remove.Contains(val))
                        remove.Add(val);
                    SetValueAtPoint(p, NewValue(ref remove));
                }
            }
        }
    }

    int NewValue(ref List<int> removed)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (int i in removed)
            available.Remove(i);

        if (available.Count <= 0)
            return 0;

        return available[random.Next(0, available.Count)];
    }

    List<Point> IsConnected(Point p, bool main)
    {
        var connected = new List<Point>();
        var val = GetValueAtPoint(p);
        Point[] directions =
        {
            Point.Up,
            Point.Right,
            Point.Down,
            Point.Left
        };

        foreach (Point dir in directions)
        {
            List<Point> line = new List<Point>();

            var same = 0;

            for (int i = 1; i < 3; i++)
            {
                Point check = Point.Add(p, Point.Mult(dir, i));
                if (GetValueAtPoint(check) == val)
                {
                    line.Add(check);
                    same++;
                }
            }

            if (same > 1) // if there are more than 1 shape in the direction, then we know its a match
            {
                AddPoints(ref connected, line); //add these points to the overarching connected list
            }
        }

        for (int i = 0; i < 2; i++) //check if we are in the middle of two of same color
        {
            List<Point> line = new List<Point>();
            var same = 0;
            Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[i + 2]) };
            foreach (Point next in check) //check both sides of the piece if they are the same, add them to list
            {
                if (GetValueAtPoint(next) == val)
                {
                    line.Add(next);
                    same++;
                }
            }
            if (same > 1)
            {
                AddPoints(ref connected, line);
            }
        }

        for (int i = 0; i < 4; i++) //check if 2x2
        {
            List<Point> square = new List<Point>();
            var same = 0;
            var next = i + 1;
            if (next >= 4)
                next -= 4;
            Point[] check = { Point.Add(p, directions[i]), Point.Add(p, directions[next]), Point.Add(p, Point.Add(directions[i], directions[next])) };
            foreach (Point point in check) //check both sides of the piece if they are the same, add them to list
            {
                if (GetValueAtPoint(point) == val)
                {
                    square.Add(point);
                    same++;
                }
            }

            if (same > 2)
            {
                AddPoints(ref connected, square);
            }
        }

        if (main) //check for other matches along current match
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, IsConnected(connected[i], false));
            }
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd)
            {
                points.Add(p);
            }
        }
    }

    int GetValueAtPoint(Point p)
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height)
            return -1;
        return board[p.x, p.y].value;
    }

    void SetValueAtPoint(Point p, int value)
    {
        board[p.x, p.y].value = value;
    }

    int FillPiece()
    {
        int val = 1;
        val = random.Next(0, 100) / (100 / pieces.Length) + 1;
        return val;
    }

    void Update()
    {
        List<NodePiece> finished = new List<NodePiece>();
        for (int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            if (!piece.UpdatePiece())
                finished.Add(piece);
        }
        for (int i = 0; i < finished.Count; i++)
        {
            NodePiece piece = finished[i];
            FlippedPieces flip = GetFlipped(piece);
            NodePiece flippedPiece = null;

            int x = piece.index.x;
            fills[x] = Mathf.Clamp(fills[x] - 1, 0, width);
            List<Point> connected = IsConnected(piece.index, true);
            bool wasFlipped = flip != null;
            if (wasFlipped) //if we flipped to make this update
            {
                flippedPiece = flip.GetOtherPiece(piece);
                AddPoints(ref connected, IsConnected(flippedPiece.index, true));
            }
            if (connected.Count == 0) // if no match
            {
                if (wasFlipped) //if we flipped
                {
                    FlipPieces(piece.index, flippedPiece.index, false); //flip back
                }
            }
            else //if we made a match
            {
                foreach (Point pnt in connected) //remove the node pieces connected
                {
                    KillPiece(pnt);
                    Node node = GetNodeAtPoint(pnt);
                    NodePiece nodePiece = node.GetPiece();
                    if (nodePiece != null)
                    {
                        nodePiece.gameObject.SetActive(false);
                        dead.Add(nodePiece);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }

            flipped.Remove(flip); //remove the flip after update
            update.Remove(piece);
        }
    }

    void ApplyGravityToBoard()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = (height-1); y >=0; y--)
            {
                Point p = new Point(x, y);
                Node node = GetNodeAtPoint(p);
                int val = GetValueAtPoint(p);
                if (val != 0)
                    continue;
                for(int ny = (y-1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = GetValueAtPoint(next);
                    if (nextVal == 0)
                        continue;
                    if(nextVal != -1) //if we did not hit an end, but its not 0 then use this to fill current hole
                    {
                        Node got = GetNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();

                        //set the hole
                        node.SetPiece(piece);
                        update.Add(piece);

                        //replace the hole
                        got.SetPiece(null);
                    }
                    else //hit an end
                    {
                        //fill in the hole
                        int newVal = FillPiece();
                        NodePiece piece;
                        Point fallPoint = new Point(x, (-1 - fills[x]));
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            piece = n;
                        }

                        piece.Initialize(newVal, p, pieces[newVal - 1]);
                        piece.rect.anchoredPosition = GetPositionFromPoint(fallPoint);

                        Node hole = GetNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                        fills[x]++;
                    }
                    
                    break;
                }
            }
        }
    }

    FlippedPieces GetFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].GetOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    string GetRandomSeed()
    {
        string seed = "";
        string acceptableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }

    public Vector2 GetPositionFromPoint(Point p)
    {
        return new Vector2(16 + (NodePiece.size * p.x), -(NodePiece.size * (height) + 16 + (NodePiece.size * p.y)));
    }

    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    Node GetNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (GetValueAtPoint(one) < 0)
            return;

        Node nodeOne = GetNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();
        if (GetValueAtPoint(two) > 0)
        {
            Node nodeTwo = GetNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeOne.SetPiece(pieceTwo);
            nodeTwo.SetPiece(pieceOne);

            if (main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceOne);
            update.Add(pieceTwo);
        }
        else
            ResetPiece(pieceOne);
    }

    void KillPiece(Point p)
    {
        List<KilledPiece> available = new List<KilledPiece>();
        for (int i = 0; i < killed.Count; i++)
        {
            if (!killed[i].falling)
                available.Add(killed[i]);
        }
        KilledPiece set = null;
        if (available.Count > 0)
            set = available[0];
        else
        {
            GameObject kill = GameObject.Instantiate(killedPiece, killedBoard);
            KilledPiece kPiece = kill.GetComponent<KilledPiece>();
            set = kPiece;
            killed.Add(kPiece);
        }

        int val = GetValueAtPoint(p) - 1;
        if (set != null && val >= 0 && val < pieces.Length)
            set.Initialize(pieces[val], GetPositionFromPoint(p));
    }

    void AddFallingPiece()
    {
        var piece = random.Next(0, 4);
        var node = new Node(piece, new Point(random.Next(0, width), 0));
        for (int x = 0; x < width; x++)
        {
            for (int y = (height - 1); y >= 0; y--)
            {

            }
        }
    }
}

[System.Serializable]
public class Node
{
    public int value; //0 - blank, 1 - blue, 2 - green, 3 - gray, 4 - purple, 5 - red
    public Point index;
    NodePiece piece;

    public Node(int v, Point p)
    {
        value = v;
        index = p;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null)
            return;
        piece.SetIndex(index);
    }

    public NodePiece GetPiece()
    {
        return piece;
    }
}

[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o;
        two = t;
    }

    public NodePiece GetOtherPiece(NodePiece p)
    {
        if (p == one)
        {
            return two;
        }
        else if (p == two)
        {
            return one;
        }
        else
            return null;
    }
}
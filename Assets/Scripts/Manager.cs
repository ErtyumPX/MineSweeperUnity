using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject nodePrefab;
    public Transform parentObject;

    [HideInInspector]
    public GameObject[,] physicalTable;
    public int width = 6;
    public int height = 6;
    public int mine_amount = 5;

    int flag_amount = 0;
    Vector2Int[] mines;

    [HideInInspector]
    public Node[,] table;
    bool is_map_created;

    bool is_first_touch;
    bool is_game_over;
    bool is_won;

    static Vector2Int[] surroundings = new Vector2Int[8];

    void Start()
    {
        surroundings = new Vector2Int[] 
        { 
            new Vector2Int(1, 0),   //right
            new Vector2Int(-1, 0),  //left
            new Vector2Int(0, 1),   //down
            new Vector2Int(0, -1),  //up
            new Vector2Int(1, 1),   //down right
            new Vector2Int(1, -1),  //down left
            new Vector2Int(-1, 1),  //up right
            new Vector2Int(-1, -1), //up left
        };

        Initialize();

    }

    void Update()
    //if the user left clicks, checks if the mouse is on a node, if so, checks if the node is revealed already,
    //if so, checks if this is the first node that has been touched so far:
    //----if so, creates the map accordingly
    //----if not, just reveals the node and continues, of course if the node is 0, it will reveal its surrounding nodes
    //    in the "Touch" method
    //
    //if the user right clicks, checks if the map is created, if so, checks if the user already used maximum amount of
    //flags which is equal to the number of mines (if the flags are in the correct nodes, user will win, that's why
    //flags are limited), if not, finds note with raycasting and checks if it's marked(flagged) before
    //if it's marked, unmarks it
    //if it's not marked, marks it and checks if all the mines are marked, if so, user wins
    {
        
        if (Input.GetMouseButtonDown(0) && !is_game_over && !is_won)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if(hit.transform.CompareTag("Node"))
                {
                    Vector2Int hit_position = new Vector2Int((int)hit.transform.position.x, -(int)hit.transform.position.y);

                    if (table[hit_position.x, hit_position.y].revealed == false)
                    {
                        if (!is_first_touch)
                        {
                            bool result = Touch(hit_position.x, hit_position.y);
                            if (!result) 
                            {
                                is_game_over = true;
                                print("Exploded!"); 
                            }
                        }

                        else
                        {
                            is_first_touch = false;

                            CreateMap(hit_position.x, hit_position.y);

                            Touch(hit_position.x, hit_position.y);
                        }
                    }
                } 
            }
        }

        if (Input.GetMouseButtonDown(1) && !is_game_over && !is_won)
        {
            if (is_map_created)
            {
                if (flag_amount < mine_amount)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        if (hit.transform.CompareTag("Node"))
                        {
                            Vector2Int hit_position = new Vector2Int((int)hit.transform.position.x, -(int)hit.transform.position.y);
                            if(table[hit_position.x, hit_position.y].marked)
                            {
                                flag_amount--;
                                table[hit_position.x, hit_position.y].marked = false;
                                physicalTable[hit_position.x, hit_position.y].GetComponentInChildren<TMP_Text>().text = "";
                            }
                            
                            else if (!table[hit_position.x, hit_position.y].revealed)
                            {
                                flag_amount++;
                                table[hit_position.x, hit_position.y].marked = true;
                                physicalTable[hit_position.x, hit_position.y].GetComponentInChildren<TMP_Text>().text = "-";

                                //checking if flagged all the mines
                                is_won = true;
                                foreach (Vector2Int mine in mines)
                                {
                                    if (table[mine.x, mine.y].marked == false)
                                    {
                                        is_won = false;
                                        break;
                                    }
                                }
                                if (is_won)
                                {
                                    print("Won!");
                                }
                            }
                        }
                    }
                }
            }
        }

    }

    public void Initialize()
    {
        //initializing variables

        is_first_touch = true;
        is_game_over = false;
        is_won = false;

        flag_amount = 0;
        mines = new Vector2Int[mine_amount];


        if (physicalTable != null)
        {
            foreach (GameObject item in physicalTable)
            {
                Destroy(item);
            }
        }

        table = new Node[width, height];
        is_map_created = false;

        physicalTable = new GameObject[width, height];

        for (int x_ = 0; x_ < width; x_++)
        {
            for (int y_ = 0; y_ < height; y_++)
            {
                physicalTable[x_, y_] = Instantiate(nodePrefab, new Vector3(x_, -y_, 0), new Quaternion(0, 0, 0, 0), parentObject);
                physicalTable[x_, y_].name = string.Format("{0}x{1}", x_, y_);
            }
        }
        //setting camera position to center the table
        if ((width - 8) > height)
        {
            Camera.main.transform.position = new Vector3(((float)width / 2) - 1, (-(float)height / 2) + 1, -width + 7);
        }
        else
        {
            Camera.main.transform.position = new Vector3(((float)width / 2) - 1, (-(float)height / 2) + 1, -height - 1);
        }
    }

    void CreateMap(int x, int y)
    //takes an x and y position as the coordinates of the first node that the player touched and creates the map accordingly
    {
        //setting every node's state to 0
        for (int x_ = 0; x_ < width; x_++)
        {
            for (int y_ = 0; y_ < height; y_++)
            {
                table[x_, y_].state = 0;
            }
        }

        //setting mines
        for (int i = 0; i < mine_amount; i++)
        {
            while (true)
            {
                int random_x = Random.Range(0, width);
                int random_y = Random.Range(0, height);
                if (table[random_x, random_y].state != -1)
                {
                    //using geometry to detect if the random position is next to the first touch but
                    //not efficient for future usage
                    //float distance = Mathf.Sqrt((random_x - x) ^ 2 + (random_y - y) ^ 2);
                    //if (distance > Mathf.Sqrt(2.1f))

                    bool is_allowed = true;
                    if (random_x == x && random_y == y) { is_allowed = false; ; } //if the random position is not the first node

                    Vector2Int random_pos = new Vector2Int(random_x, random_y);
                    foreach (Vector2Int surrounding_position in GetSurroundings(x, y)) //if the random position is not next to the first node
                    {
                        if (random_pos == surrounding_position) { is_allowed = false; }
                    }

                    if (is_allowed)
                    {
                        table[random_x, random_y].state = -1;
                        mines[i] = random_pos;
                        //mine is set

                        //now mine is increasing the state of its surroundings by 1
                        foreach (Vector2Int new_position in GetSurroundings(random_x, random_y))
                        {
                            if (table[new_position.x, new_position.y].state != -1) //if surrounding is not a mine
                            {
                                table[new_position.x, new_position.y].state += 1;
                            }
                        }
                        break;
                    }

                }
            }
        }

        is_map_created = true;
    }
    
    public List<Vector2Int> GetSurroundings(int x, int y)  
    //takes an x and y position and returns possible surrounding nodes of that position
    {
        List<Vector2Int> valid_surroundings = new List<Vector2Int>();
        foreach (Vector2Int offset in surroundings)
        {
            if (x + offset.x >= 0 && width > x + offset.x && //if x coordinate is in the table
                y + offset.y >= 0 && height > y + offset.y)  //if y coordinate is in the table
            {
                valid_surroundings.Add(new Vector2Int(x + offset.x, y + offset.y));
            }
        }

        return valid_surroundings;
    }

    bool Touch(int x, int y)
    //takes an x and y position and reveals the node so it can be seened on the screen
    //returns false if the touhced node is a mine
    {
        table[x, y].revealed = true;

        if (table[x, y].state == -1)
        {
            physicalTable[x, y].GetComponentInChildren<TMP_Text>().text = "M";
            return false;
        }

        else if (table[x, y].state == 0)
        {
            RevealSurroundings(x, y);
        }

        physicalTable[x, y].GetComponentInChildren<TMP_Text>().text = table[x, y].state.ToString();
        return true;
    }

    void RevealSurroundings(int x, int y)
    //takes an x and y position and touches the surrounding nodes of that position if they are not already revealed
    {
        foreach (Vector2Int valid_surrounding in GetSurroundings(x, y))
        {
            if (!table[valid_surrounding.x, valid_surrounding.y].revealed)
            {
                Touch(valid_surrounding.x, valid_surrounding.y);
            }
        }
    }

}

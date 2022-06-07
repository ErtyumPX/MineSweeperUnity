# MineSweeperUnity
 
# MineSweeperUnity

Good old Mine Sweeper game in Unity, made with using basic functions of C#. Only Vectors and Random Library is called from Unity's own packages.


---

<p align="center">
  <img src="https://github.com/ErtyumPX/MineSweeperUnity/blob/main/Images/puzzle_image_one.jpeg">
</p>

<p align="center">
  <img src="https://github.com/ErtyumPX/MineSweeperUnity/blob/main/Images/puzzle_image_two.jpeg">
</p>

---


Algorithm:

The game starts when the user clicks (clicks detected by Raycasting) any node of the board.

Then algorithm creates the mine field according to the first node user clicked in such a way that there will not be any mine near the first node.

In the rest of the game, whenever user clicks a node, that node reveals itself.

- If the node is a mine, it's game over.

- If the nodes value is 0, which means that there is no mine covers that node, it will reveal the nodes next to it as well.

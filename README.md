This was a home assignment done during the hiring process for a game development position.

For this home assignment, I implemented a highly simplified version of the game [Peggle](https://www.youtube.com/watch?v=Z9Y40K7IH3s). However, the caveat was that it had to be done using ECS for the physics and game logic. 
This was because the game that this company works in has a high object count, and thus Unity's default physics system is not performant enough to work with. Peggle similarly has a high object count, with many physics calculations that need to be ran
each frame, thus ECS was a good approach.

This ended up being a learning experience, as I needed to learn Unity ECS from scratch for this project over the span of 3-4 days.

For this project, I leveraged ECS for the entirety of the game logic as well as the physics system. A unique problem arised as well where the ECS physics system did not support 2D directly. As a result, the "balls"
are actually simulated as 3D cylinders, and the squares as 3D bricks, with the Z position and all rotation locked.


## Results

Below is the resulting gameplay. Destroying blue obstacles yields 10 points, orange obstacles yield 50, and green obstacles duplicate into more balls. The high dynamic rigidbody count (seems to be around 20-40 balls) doesn't noticeably affect performance, though this
is yet to be tested with larger amounts.

https://github.com/user-attachments/assets/660b819d-89b8-45c2-826d-a3c9382fb138

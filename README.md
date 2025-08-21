# Sniper Frenzy
A project focused on creating a simple but frantic gameloop for the Quest 2 platform using Unity 6.2. The game focuses on defending your tower against an ever-increasing amount of robots using your rocket rifle! 
![Gameplay Screenshot](https://github.com/AKSB-GP/SniperFrenzy/blob/main/RifleScopeview2.jpg)
![Gameplay Screenshot](https://github.com/AKSB-GP/SniperFrenzy/blob/main/RifleView.jpg)

## About the Game
For the project I wanted to explore various aspects of XR interactions with a toon style as well as explore the general game-development cycle.
All asssets except textures and sounds were made from scratch. Assets such as the enemy, the rifle, and level were modelled, rigged and animated in Blender-3D. 
<img width="1431" height="745" alt="image" src="https://github.com/user-attachments/assets/2cedd01a-1d31-4dbe-bfc5-a4138e1d737a" />

<img width="1309" height="618" alt="image" src="https://github.com/user-attachments/assets/77e2bede-c948-4cc2-8154-6665a0a22d7b" />

## Features
- **Weapon Handling**: The weapon interaction replicates a boltaction rifle with the user needing to cycle each rocket between firing. Magazines are attachable using a socket interactor with the animation of the boltcycling consisting of blendtrees of rifle animations and interaction colliders. 
![Gameplay Screenshot](https://github.com/AKSB-GP/SniperFrenzy/blob/main/boltinteraction-compressed.gif)
- **Custom grab Interactable** The rifle is can be held two handed, however the basic two handed interaction was too basic and I wanted to make the experience more enjoyable. Thus a custom XR grab was implemented allowing for more dynamic attachpoints.
![Gameplay Screenshot](customgrabreload_.gif)
- **Customaziable gameloop**: Gameloop can be adjusted with maximum amount of enemies, spawnintervals, minimum and-maximum distances, movement speed etc.
- **Toon shading**: I wanted various shaders technics such as scope shaders and toon shading technics using both HLSL and Shadergraph.

## 🛠️ Tech Stack
- **Engine**: Unity 
- **VR Platform**: Quest 2 (not tested on other platforms)

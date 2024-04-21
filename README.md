# UAVSI
This is the repository for the code, supporting documentation and raw files as part of my final year Computer Science project at the University of Warwick.

The project is in Unity and the code is written in C#. This README seres as a guide on how to run the project.

---
## Guide
### Prerequisites
- Unity 2022.3.11f1

### Running the project
1. Clone the repository to your local machine:
```bash
git clone https://github.com/SwiftfoxStudios/UAVSI.git
```
2. Open Unity Hub and click on the `Projects` tab. Click on `Add` and navigate to the cloned repository.
3. Click on the project to open it in Unity.

### Running the simulation
Generally, the simulation can be run using the 'play' button in Unity. However, the simulation can be run multiple times, with different parameters, by changing the values of the `Restart.cs` script. These can also be changed in the Unity editor, attached to the `Main Camera` object.

The `Flock` object contains the spawning parameters for the agents. The flock has a behaviour attached to it, which can hold mutliple behaviours as per the `Composite Behaviour` behaviour. The behaviours can be added and removed in the Unity editor.

Ultimately, getting to know the project is the best way to understand how it works.
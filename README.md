# JellyFish-Lite
A Unity Scriptable Object Workflow SDK inspired by Ryan Hipple's Unite 2017 talk "Game Architecture with Scriptable Objects".  
I hope this SDK helps you the way it helps me.

JellyFish-Lite comes in two flavors
+ JellyFish-Lite and 
+ JellyFish-Lite Core.<br>

JellyFish-Lite is the complete package that contains all addons and plugins. <br>
JellyFish-Lite Core is minimal package that only contains the core Scriptable Object Architecture Structure giving you the user the choice to extend its functionality with the available addons or add your own or third party plugins, tools or scripts to either package to make your own version of JellyFish-Lite. 

![JellyFish Astronaut](https://w.wallhaven.cc/full/g8/wallhaven-g8dm6e.jpg)

# Features
## Editor
### Sprite Extractor
The Sprite Extractor is a Simple tool that Allows you to easily Extract Sub Sprites from a Spritesheet.
It comes with a Right Click to use feature as well as a Sprite Extractor Window.

Right Click Feature:
1. Right Click on a Spritesheet
2. Navigate to JellyFish/SpriteExtractor
3. Choose between Source and Meta
+ Meta Allows you to Extract a single or multiple Sub Sprites from the Spritesheet dropdown.
+ Source Allows you to Extract all the Sub Sprites.
4. Extract at current location or anywhere else on your machine.
5. Done!

SpriteExtractor Window
1. Select JellyFish from the Menubar
2. Select Sprite Extractor
3. Drag and Drop your Sprite
4. Click Extract
5. Done!

### Gif Extractor

The Gif Extractor simply allows you to extract the frames of a Gif Image. <br>
Just like the Sprite Extractors Right Click, the Gif Extract follows a Similar pattern.

### Project Setup Tool
The Project Setup Tool is a tool that allows you to serialize any number of Folders by saving the path to each folder selected into a json file with a custom extension *.pdjson. <br>
The newly created *.pdjson file can now be brought into any new unity project you create and be used to easily recreate your projects directories with an easy double click. <br>

I have created a default .pdjson file that can found in the Resources Folder. You can double click that file and see what folders where created and added to your project.

How To Serialize:
1. Select your Projects Directories that you wish to Serialize
2. Right Click
3. Navigate to JellyFish/Project Directories/Serialize Folders
4. Select where your *.pdjson file should be saved
5. Done!

How To Deserialize:
1. Start a new Unity Project.
2. Bring in JellyFish Lite or the Project Setup Tool
3. Bring in your *.pdjson file
4. Double click and Create Folders
5. Done.

### Text Creator
The Text Creator allows your to create text and json files without the need to open any other external editor to creator one.

## Runtime

### Event System
#### Unity Events
Unity Events is the Unity Created Event System that can be used to trigger actions similar to how canvas buttons are used to trigger actions when the button is clicked. 
It is setup exactly the same way.
I have added a few Custom UnityEvents for:
1. Booleans
2. Floats
3. Strings
4. Integers

These Custom Unity Events can be used to pass values to the functions triggered by these Unity Events.

```c#
public UnityEvent OnSomeEvent = new UnityEvent();

public void SomeMethod(){
    OnSomeEvent.Invoke();
}
```
or
```c#
public BoolEvent OnSomeEvent = new BoolEvent();

public void SomeMethod(bool value){
    OnSomeEvent.Invoke(value);
}
```
#### UltEvents
UltEvents is an Event System similar to Unity's Event System. However, it allows you to trigger not only public functions, but also private and static functions.<br>
You can have a detailed look at its features [here](https://kybernetik.com.au/ultevents/)

#### GameEvents and Listeners
I won't do enough justice to this topic sadly. <br>
However, from my personal understanding. GameEvents, follow a similar pattern to Publisher and Subscriber Pattern. Where the GameEvent is "Raised" and a Listener "Responds" to that call of the "Raised GameEvent" and "Triggers" a Response. <br>
Please, for more information watch Ryan Hipple's talk on [Game Architecture](https://www.youtube.com/watch?v=raQ3iHhE_Kk) to get a better understanding of how GameEvents work and how to use them.

How to Use GameEvents in your Code:
```c#
public GameEvent SomeEvent;

public void RaiseGameEvent(){
    SomeEvent.Raise();
}
```

How to Create a GameEvent:
1. Right Click
2. Navigate to Create/JellyFish/Events/GameEvent
3. Drag and Drop on the appropriate field.

### Primitive Data Scriptable Objects
Primitive Data Scriptable Objects are ideal, because they allow you to pass data from one object to another without the need to create any singleton patterns or hard references to any gameobjects, 
The scriptable objects can be extended further and support many other unity objects, however, the current package only includes:
1. Boolean Data
2. Float Data
3. Integer Data
4. String Data
5. Vector2 and Vector3 Data

How to Use Primitive Scriptable Objects in your code: 
 ```c#
public BoolField BooleanField;
public FloatField FloatField;
public StrField StringField;
public IntField IntegerField;
public Vector2Field Vector2Field;
public Vector3Field Vector3Field;

private void SomeMethod(bool value){
    BooleanField.Value = value;
    IntegerField.Value = BooleanField ? 1 : 0;
    // etc
}
```

How to Create a Primitive Data Scriptable Object:
1. Right Click
2. Navigate to Create/JellyFish/Data/Primitives
3. Drag and Drop your Primitive Data onto the appropriate Field on your GameObject.
+ A Boolean Scriptable Object can only be Drag and Dropped on a BoolField or BoolData Field in the Inspector.

### Object Pool
Object Pooling is a way to manage reduce Garbage Collection calls by instantiating "x" number of objects and caching references to them for use when required and disabling them thereafter.

Object Pooling is useful when you are instantiating and destroying the same object multiple times, such as bullets. By creating an object pool of bullets you reduce garbage collection that could lead to performance drops on low end devices.

Please Look at the Example Scene for a detail example of how to setup an Object Pool.

### Fader System
Fader System allows your to fade or unfade a colour, sprite, material or a light using the "UnityAsync" plugin.

How To:
1. Add the Required Fader to the Object
+ Sprite Fader to an Object with a Sprite Renderer
+ Material Fader to an Object with a Material
+ Light Fader to an Object with a Light
+ UI Fader to an Object with a UI Component
2. Create a Generic Fader Object to Manage the Fading or Unfading of the Object.
+ Drag and Drop the Object with the Fader Component to the Fader List.
3. Call the Fade or Unfade Method from the Generic Fader to Perform the Required Action.

Please view the Example Scene for a practical Example.

### Scene Sets
Scene Set create Scriptable Object References that can be used to Load and Unload scenes (additively and asynchronously) by name. 

How To: 
1. Right Click on a Scene Object
2. Navigate to Scene Set/Create Scene Set
3. A new Scene Set object will be created of the selected scene.
4. Reference a Scene Set in code to load or unload the referenced scene during runtime.

### Monitors
#### FPS Monitor
Monitor the FPS game or application.

Add Component => FPSMonitor

#### Screen Monitor
Monitor changes in your games resolution or world size with ease. The exposed world height and width can be used to clamp within the game view or monitor objects left the view. 

Add Component => ScreenSizeMonitor

Requirements:
1. Camera Reference Scriptable Object
2. Resolution Reference Scriptable Object

Additional:
1. World Height and Width Primitive Float Scriptable Objects.<br>
(Right Click: Create/JellyFish/Data/Primitives/Float)

### Camera Utilities
The Camera Utility allows you to keep a reference to your games camera using a Camera Reference Scriptable Object.

How To:
1. Add a GameCamera Component to your Camera Object.
2. Create a Camera Reference Scriptable Object and Add it to the GameCamera Component.<br>
(Right Click: Create/JellyFish/Utilities/Camera)

You can now pass around the "Camera Reference" to any object that may require a reference to the current active camera.

### Extensions
Custom Extensions that I have made  or found over the years that add additional functionality to the types listed below.
1. Array
2. GameObject
3. Layer Mask
4. List
5. String
6. Texture
7. Transform
8. Type
9. Vector2

# Documentation (GIFS)
Please look at the Documentation Folder for some Practical Examples on how to use the tools and create some of the runtime Scriptable Objects and Components. 

# Roadmap
Add GIFs to README
Add Example Scenes
Bug Fixes!

# Packages and Plugins
## GitHub
1. [UnityAsync](https://github.com/muckSponge/UnityAsync)
2. [SimpleJson](https://github.com/Bunny83/SimpleJSON)

## AssetStore Packages
1. [UltEvents](https://assetstore.unity.com/packages/tools/gui/ultevents-111307?aid=1100l8ah5&utm_source=aff)

## Nuget Packages and Plugins
1. [System.Drawing.Common](https://www.nuget.org/packages/System.Drawing.Common/)

# Attributions
1. [Kearan Petersen](https://github.com/BLUDRAG) - Mentor and Nakama
2. [Ryan Hipple](https://github.com/roboryantron) - Game Architecture with Scriptable Objects Unite 2017 Talk
3. [Kybernetik](https://kybernetikgames.github.io/kailas-dierk/) - UltEvents
4. [UnityAsync](https://github.com/muckSponge/UnityAsync)
5. [Angoes Artwork Instagram](https://www.instagram.com/angoes25artwork/) Creator of the JellyFish Astronaut Image
6. [Angoes Facebook](https://www.facebook.com/Angoes25Studio/) Creator of the JellyFish Astronaut Image
7. [Angoes TeePublic](https://www.teepublic.com/user/angoes25) Creator of the JellyFish Astronaut Image

# Details
1. [LinkedIn Profile](https://www.linkedin.com/in/ubaidullah-effendi-emjedi-202494183/)  
2. [Itchio Page](https://uee.itch.io/)

# Important
The Creator of the JellyFish Astronaut Image is [Angoes25](https://www.instagram.com/angoes25artwork/). All other plugins and addons are owned by their respective creators.

JellyFish SDK is an open-source SDK for Unity.

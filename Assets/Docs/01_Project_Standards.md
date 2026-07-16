Important: 
NEVER EVER LEAVE AN ASSET OR MATERIAL UNNAMED. IT NEEDS TO BE DESCRIPTIVE

In Unity:
Each main folder under “_Project” should be based on features, and each folder in each feature is divided by categories. Do not separate the feature’s assets from its designated folder.

Asset Naming Convention:
Naming should be descriptive.
2Ds: [UseCase]_[Name]_[Optional Type]_[VersionNumber]
Use Case : bg (background), ic (icon), btn (button skin), ava (avatar). mdl (Model), etc.
Optional Type : ActiveType, InactiveType, Casting, Pressed, etc.

Model Conventions:
Imported model GameObjects should have a local transform scale of (1,1,1). Any scaling adjustments should be made in Blender before export. 

All models should be made relative to the player character (1.8 meters tall)

Models should be properly UV unwrapped to allow texture replacement, material changes, and future reskinning. 

Map Creations:
Levels should be assembled in Unity.
Use Blender to create modular pieces such as walls, floors, stairs, rocks, trees, props, and architectural sections.
Do not create an entire level or map as a single Blender model and import it into Unity.
Each piece should be reusable, grid-aligned when appropriate, and imported with Scale = (1,1,1).
Level layout, lighting, navigation, colliders, and gameplay placement should be managed inside Unity.


Gdrive:
Only includes the following:
Blender files
PSD/Krita files
Audio project files
References
Documentation
Other editable source assets
GitHub:
Only includes the following:
Scripts
Prefabs
Scenes
Materials
ScriptableObjects
Unity-imported assets



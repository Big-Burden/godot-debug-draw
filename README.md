# Godot debug drawing

**Godot mono 4.0+**  
**This is c# only, I have no plans to support GDScript**

![Godot_4 1b_mono_JcDsjpc9N8](https://github.com/Big-Burden/godot-debug-draw/assets/50963453/dd426c83-d48d-4502-b12a-016aa76f3f10)

## Features
Quick and easy of drawing shapes, text and physics queries.

### Shapes

- Box  
`DebugDraw.Box(GlobalTransform, boxShape.size);`
- Sphere  
`DebugDraw.Sphere(GlobalTransform, 1.0f);`
- Capsule  
`DebugDraw.Capsule(GlobalTransform, 0.5f, 2.0f);`
- Cylinder  
`DebugDraw.Cylinder(GlobalTransform, 1.0f, 1.0f);`
- Plane  
`DebugDraw.Plane(GlobalTransform, Vector3.Up);`  
- Quad  
`DebugDraw.Quad(GlobalPosition, 1.0f);`
- Point  
`DebugDraw.Point(GlobalTransform, 1.0f);`
- Circle  
`DebugDraw.Circle(GlobalTransform, 1.0f);`
- Axes  
`DebugDraw.Axes(GlobalTransform, 1.0f);`
- Arrow  
`DebugDraw.Arrow(GlobalTransform, Vector3.Forward);`
- Line  
`DebugDraw.Line(Vector3.Zero, Vector3.Right);`
- Lines  
`DebugDraw.Lines(points);`

All shape methods have variants that accept either a Transform, Position & Rotation or Position  
`DebugDraw.Box(GlobalTransform, boxShape.size);`  
`DebugDraw.Box(DrawPosition, DrawRotation, boxShape.size);`  
`DebugDraw.Box(DrawPosition, boxShape.size);`  

Primitive shapes also have an option to bee drawn solid  
`DebugDraw.Box(GlobalTransform, boxShape.size, Colors.Blue, 1.0f, true);`  
`DebugDraw.Sphere(GlobalTransform, sphereShape.radius, Colors.Red, 1.0f, true);`  

They can also be passed a Shape3D and take the shape parameters from that  
`DebugDraw.Box(GlobalTransform, boxShape, Colors.Blue, 1.0f, true);`  
`DebugDraw.Sphere(GlobalTransform, sphereShape, Colors.Red, 1.0f, true);`  


There is also a generic Shape method that takes a `Shape3D`, although it only supports drawing these shapes:
`BoxShape3D`, `SphereShape3D`, `CapsuleShape3D`, `CylinderShape3D` and `WorldBoundaryShape3D`

`CollisionShape3D collisionShape = GetNode<CollisionShape3D>("CollisionShape")`  
`DebugDraw.Shape(collisionShape.GlobalTransform, collisionShape.Shape);`  

### Queries

There are methods for drawing common physics queries

- Intersect ray  
`Dictionary result = GetWorld3D().DirectSpaceState.IntersectRay(Query);`  
`DebugDraw.RayIntersect(Query, result, 5.0f);`  
- Shape motion  
`float[] result = GetWorld3D().DirectSpaceState.CastMotion(Query);`  
`DebugDraw.ShapeMotion(Query, result, 5.0f);`  
- Shape collision  
`Array result = GetWorld3D().DirectSpaceState.CollideShape(Query);`  
`DebugDraw.RayIntersect(Query, result, 5.0f);`  

### Text

Text can be drawn on the viewport, either temporary or keyed

`DebugDraw.Text("This is a temporary message", 1.0f, Colors.Blue);`  
`DebugDraw.TextKeyed("Speed", _player.GetSpeed(), 1.0f, Colors.Yellow);`  

Text can also be drawn projected to a position in the scene easily with the Text3D methods  
`DebugDraw.Text3DKeyed(GetEnemyId().ToString() "Move target", GetMoveTarget(), 1.0f, Colors.Blue);`  

### Layers and in game menu

Each draw method takes an optional layer parameter, this allows easy toggling of debug drawing e.g player, enemies, weapons etc.

`DebugDraw.Box(GlobalTransform, Vector3.One, Colors.Green, 0.0f, false, 1 << 2);`  
`DebugDraw.Box(GlobalTransform, Vector3.One, Colors.Green, 0.0f, false, (uint)DebugLayers.Layer2);`  

The in-game menu is accessed with the \` key by default

The in game menu also shows the pools of debug draw instances as:  
 current size / max size (available)  

### Plugin settings

The plugin settings can be found in the project settings "Project > Project Settings > Debug Drawing"
advanced settings will need to be toggled

Here the layer names, menu key and initial and max pool sizes can be set.

You should restart your project after changing any of these.

using Godot;
using GC = Godot.Collections;

namespace Burden.DebugDrawing;
public enum DebugShape
	{
		Cube,
		Cylinder,
		Sphere,
		Point,
		Quad,
		Plane,
		Circle,
		Axes,
		Arrow
	}

//Move some of these into static arrays (cube, axis, point, quad etc)
public class DebugMeshes
{
	public static Mesh Construct(DebugShape mesh, Mesh.PrimitiveType type = 
		Mesh.PrimitiveType.Lines)
	{
		var arrMesh = new ArrayMesh();
		
		Vector3[] vertices = null;
		int[] indices = null;
		Color[] colors = null;
	
		switch (mesh)
		{
			case DebugShape.Cube:
			{
				vertices = new[]
				{
					new Vector3(-0.5f, -0.5f, -0.5f),
					new Vector3(0.5f, -0.5f, -0.5f),
					new Vector3(0.5f, 0.5f, -0.5f),
					new Vector3(-0.5f, 0.5f, -0.5f),
					new Vector3(-0.5f, -0.5f, 0.5f),
					new Vector3(0.5f, -0.5f, 0.5f),
					new Vector3(0.5f, 0.5f, 0.5f),
					new Vector3(-0.5f, 0.5f, 0.5f)
				};
		
				indices = new[]
				{
					//top
					0, 1,
					1, 2,
					2, 3,
					3, 0,

					//bottom
					4, 5,
					5, 6,
					6, 7,
					7, 4,

					//edges
					0, 4,
					1, 5,
					2, 6,
					3, 7
				};
				break;
			}
			case DebugShape.Cylinder:
			{
				int resolution = 8;
				float radius = 0.5f;
				float height = 1.0f;

				vertices = new Vector3[resolution * 2];
				
				indices = new int[resolution * 6];

				float angleStep = Mathf.Tau / resolution;
				
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i] = new Vector3(x, height * 0.5f, z); //Top
					vertices[i + resolution] = new Vector3(x, height * -0.5f, z); //Bottom
				}

				
				for (int i = 0; i < (resolution * 2)  ; i++)
				{
					var ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					//Top
					indices[i] = ind;
					
					//Bottom
					indices[i + (resolution * 2)] = ind + resolution;

					//Edges
					if (i % 2 == 0)
					{
						var offset = resolution * 4;
						indices[i + offset] = ind;
						indices[i + offset+ 1] = ind + resolution;
					}
				}
				break;
			}
			case DebugShape.Sphere:
			{
				
				//re-write into a single/two loops
				
				//y
				
				int resolution = 16;
				float radius = 0.5f;

				vertices = new Vector3[resolution * 6];
				
				indices = new int[resolution * 18];

				float angleStep = Mathf.Tau / resolution;
				
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i] = new Vector3(x, 0.0f, z);
				}

				for (int i = 0; i < (resolution * 2); i++)
				{
					var ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i] = ind;
				}
				
				//x
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i + resolution] = new Vector3(0.0f, x, z);
				}

				for (int i = 0; i < (resolution * 2); i++)
				{
					var ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (resolution * 2)] = ind + resolution;
				}

				
				//z
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i + (resolution * 2 )] = new Vector3(z, x, 0.0f);
				}

				for (int i = 0; i < (resolution * 2); i++)
				{
					var ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (resolution * 4)] = ind + (resolution * 2);
				}
				
				
				break;
			}
			case DebugShape.Point: 
			{
				vertices = new[]
				{
					new Vector3(0.5f, 0.0f, 0.0f),
					new Vector3(-0.5f, 0.0f, 0.0f),
				
					new Vector3(0.0f, 0.5f, 0.0f),
					new Vector3(0.0f, -0.5f, 0.0f),
				
					new Vector3(0.0f, 0.0f, 0.5f),
					new Vector3(0.0f, 0.0f, -0.5f),
				};

				indices = new[]
				{
					0, 1,

					2, 3,

					4, 5,
				};
				break;
			}
			case DebugShape.Quad:
			{
				vertices = new[]
				{
					new Vector3(-0.5f,-0.5f, 0.0f),
					new Vector3(-0.5f, 0.5f, 0.0f),
					new Vector3(0.5f, 0.5f, 0.0f),
					new Vector3(0.5f, -0.5f, 0.0f)
				};
				indices = new[]
				{
					0, 1,
					1, 2,
					2, 3,
					3, 0
				};
				break;
			}
			case DebugShape.Plane:
			{
				vertices = new[]
				{
					new Vector3(-0.5f,-0.5f, 0.0f),
					new Vector3(-0.5f, 0.5f, 0.0f),
					new Vector3(0.5f, 0.5f, 0.0f),
					new Vector3(0.5f, -0.5f, 0.0f),

				};
				indices = new[]
				{
					0, 1, 2,
					2, 3, 0,
					
				};
				break;
			}
			case DebugShape.Circle:
			{
				int resolution = 16;
				float radius = 0.5f;

				vertices = new Vector3[resolution * 2];
				
				indices = new int[resolution * 6];

				float angleStep = Mathf.Tau / resolution;
				
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i] = new Vector3(x, 0.0f, z);
				}

				for (int i = 0; i < (resolution * 2); i++)
				{
					var ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i] = ind;
				}

				break;
			}
			case DebugShape.Axes:
			{
				float axisLength = 0.5f;
				float arrowLength = 0.25f;
				float arrowSize = 0.125f;
			
				vertices = new[]
				{
					//X
					//line
					Vector3.Right * axisLength,
					Vector3.Right * -axisLength,
				
					//arrow
					Vector3.Right * (axisLength + arrowLength),
				
					new Vector3(axisLength, -arrowSize, -arrowSize),
					new Vector3(axisLength, -arrowSize, arrowSize),
					new Vector3(axisLength, arrowSize, arrowSize),
					new Vector3(axisLength, arrowSize, -arrowSize),
				
					//Y
					//line
					Vector3.Up * axisLength,
					Vector3.Up * -axisLength,
				
					//arrow
					Vector3.Up * (axisLength + arrowLength),
				
					new Vector3(-arrowSize, axisLength, -arrowSize),
					new Vector3(-arrowSize, axisLength, arrowSize),
					new Vector3(arrowSize, axisLength, arrowSize),
					new Vector3(arrowSize, axisLength, -arrowSize),
				
					//Z
					//line
					Vector3.Back * axisLength,
					Vector3.Back * -axisLength,

					//arrow
					Vector3.Back * (axisLength + arrowLength),

					new Vector3(-arrowSize, -arrowSize, axisLength),
					new Vector3(arrowSize, -arrowSize, axisLength),
					new Vector3(arrowSize, arrowSize, axisLength),
					new Vector3(-arrowSize, arrowSize, axisLength),
				};

				indices = new[]
				{
					//X
					//axis
					0, 1,
				
					//arrow
					3, 4,
					4, 5,
					5, 6,
					6, 3,
				
					//tip
					3, 2,
					4, 2,
					5, 2,
					6, 2,
				
					//Y
					//axis
					7, 8,
				
					//arrow
					10, 11,
					11, 12,
					12, 13,
					13, 10,
					//tip
					10, 9,
					11, 9,
					12, 9,
					13, 9,
				
					//Z
					//axis
					14, 15,
				
					//arrow
					17, 18,
					18, 19,
					19, 20,
					20, 17,
					//tip
					17, 16,
					18, 16,
					19, 16,
					20, 16,
				};
				colors = new[]
				{
					Colors.Red,
					Colors.Red,
					Colors.Red,
					Colors.Red,
					Colors.Red,
					Colors.Red,
					Colors.Red,
				
					Colors.Green,
					Colors.Green,
					Colors.Green,
					Colors.Green,
					Colors.Green,
					Colors.Green,
					Colors.Green,
				
					Colors.Blue,
					Colors.Blue,
					Colors.Blue,
					Colors.Blue,
					Colors.Blue,
					Colors.Blue,
					Colors.Blue
				};
			
				break;
			}
			case DebugShape.Arrow:
			{
				float arrowHeadLength = 0.25f;
				float arrowSize = 0.125f;
				
				vertices = new Vector3[]
				{
					//line
					Vector3.Zero,
					Vector3.Back,
				
					//arrow
					Vector3.Back * (1.0f + arrowHeadLength),
				
					new Vector3(-arrowSize, -arrowSize, 1.0f),
					new Vector3(arrowSize, -arrowSize, 1.0f),
					new Vector3(arrowSize, arrowSize, 1.0f),
					new Vector3(-arrowSize, arrowSize, 1.0f),
				};
				
				indices = new int[]
				{
					0, 1,

					//arrow
					3, 4,
					4, 5,
					5, 6,
					6, 3,

					//tip
					3, 2,
					4, 2,
					5, 2,
					6, 2,

				};
				break;
			}
		}

		var arrays = new GC.Array();
		arrays.Resize((int)Mesh.ArrayType.Max);
		arrays[(int)Mesh.ArrayType.Vertex] = vertices;
		arrays[(int)Mesh.ArrayType.Index] = indices;
	
		if (colors != null)
		{
			arrays[(int)Mesh.ArrayType.Color] = colors;
		}
	
		arrMesh.AddSurfaceFromArrays(type, arrays);

		return arrMesh;
	}


	private static void GenerateCircle(Vector3 axis, float radius, out Vector3[] vertices, 
		out int[] indices)
	{
		vertices = new Vector3[] { };
		indices = new int[] { };
	}
}

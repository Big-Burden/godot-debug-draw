using Godot;
using GC = Godot.Collections;

namespace Burden.DebugDrawing;

public enum DebugShape
{
	Cube,
	Cylinder,
	Capsule,
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
	private static readonly Vector3[] CubeVertices =
	{
		new(-0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),
		new(-0.5f, 0.5f, 0.5f)
	};

	private static readonly int[] CubeIndices =
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

	private static readonly Vector3[] PointVertices =
	{
		new(0.5f, 0.0f, 0.0f),
		new(-0.5f, 0.0f, 0.0f),

		new(0.0f, 0.5f, 0.0f),
		new(0.0f, -0.5f, 0.0f),

		new(0.0f, 0.0f, 0.5f),
		new(0.0f, 0.0f, -0.5f)
	};

	private static readonly int[] PointIndices =
	{
		0, 1,

		2, 3,

		4, 5
	};

	private static readonly Vector3[] QuadVertices =
	{
		new(-0.5f, -0.5f, 0.0f),
		new(-0.5f, 0.5f, 0.0f),
		new(0.5f, 0.5f, 0.0f),
		new(0.5f, -0.5f, 0.0f)
	};

	private static readonly int[] QuadIndices =
	{
		0, 1,
		1, 2,
		2, 3,
		3, 0
	};

	private static readonly int[] PlaneIndices =
	{
		2, 1, 0,
		0, 3, 2
	};


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
				vertices = CubeVertices;
				indices = CubeIndices;
				break;
			}
			case DebugShape.Cylinder:
			{
				int resolution = 16;
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


				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					//Top
					indices[i] = ind;

					//Bottom
					indices[i + resolution * 2] = ind + resolution;

					//Edges
					if (i % 8 == 0)
					{
						int offset = resolution * 4;
						indices[i + offset] = ind;
						indices[i + offset + 1] = ind + resolution;
					}
				}

				break;
			}
			case DebugShape.Capsule:
			{
				int resolution = 16;
				float radius = 0.5f;
				float height = 0.5f;

				vertices = new Vector3[resolution * 8];

				indices = new int[resolution * 24];

				float angleStep = Mathf.Tau / resolution;
				int offset = 0;

				//X
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float y = Mathf.Sin(angle) * radius * 2.0f;
					if (i < resolution * 0.5f)
					{
						y += height;
					}
					else
					{
						y -= height;
					}

					vertices[i + offset] = new Vector3(x, y, 0.0f);
				}

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (offset * 2)] = ind;
				}


				//Z
				offset += resolution;
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float y = Mathf.Sin(angle) * radius * 2.0f;
					if (i < resolution * 0.5f)
					{
						y += height;
					}
					else
					{
						y -= height;
					}

					vertices[i + offset] = new Vector3(0.0f, y, x);
				}


				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (offset * 2)] = ind + offset;
				}


				//Top circle
				offset += resolution;
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i + offset] = new Vector3(x, height, z);
				}


				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (offset * 2)] = ind + offset;
				}


				//Bottom circle
				offset += resolution;
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i + offset] = new Vector3(x, -height, z);
				}

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + (offset * 2)] = ind + offset;
				}


				break;
			}
			case DebugShape.Sphere:
			{
				//re-write into a single/two loops

				//y

				int resolution = 16;
				float radius = 1.0f;

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

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
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

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + resolution * 2] = ind + resolution;
				}


				//z
				for (int i = 0; i < resolution; i++)
				{
					float angle = i * angleStep;
					float x = Mathf.Cos(angle) * radius;
					float z = Mathf.Sin(angle) * radius;

					vertices[i + resolution * 2] = new Vector3(z, x, 0.0f);
				}

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
					indices[i + resolution * 4] = ind + resolution * 2;
				}


				break;
			}
			case DebugShape.Point:
			{
				vertices = PointVertices;
				indices = PointIndices;
				break;
			}
			case DebugShape.Quad:
			{
				vertices = QuadVertices;
				indices = QuadIndices;
				break;
			}
			case DebugShape.Plane:
			{
				vertices = QuadVertices;
				indices = PlaneIndices;
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

				for (int i = 0; i < resolution * 2; i++)
				{
					int ind = Mathf.CeilToInt(i / 2.0f) % resolution;
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

					new(axisLength, -arrowSize, -arrowSize),
					new(axisLength, -arrowSize, arrowSize),
					new(axisLength, arrowSize, arrowSize),
					new(axisLength, arrowSize, -arrowSize),

					//Y
					//line
					Vector3.Up * axisLength,
					Vector3.Up * -axisLength,

					//arrow
					Vector3.Up * (axisLength + arrowLength),

					new(-arrowSize, axisLength, -arrowSize),
					new(-arrowSize, axisLength, arrowSize),
					new(arrowSize, axisLength, arrowSize),
					new(arrowSize, axisLength, -arrowSize),

					//Z
					//line
					Vector3.Back * axisLength,
					Vector3.Back * -axisLength,

					//arrow
					Vector3.Back * (axisLength + arrowLength),

					new(-arrowSize, -arrowSize, axisLength),
					new(arrowSize, -arrowSize, axisLength),
					new(arrowSize, arrowSize, axisLength),
					new(-arrowSize, arrowSize, axisLength)
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
					20, 16
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

				vertices = new[]
				{
					//line
					Vector3.Zero,
					Vector3.Forward,

					//arrow
					Vector3.Forward * (1.0f + arrowHeadLength),

					new(-arrowSize, -arrowSize, -1.0f),
					new(arrowSize, -arrowSize, -1.0f),
					new(arrowSize, arrowSize, -1.0f),
					new(-arrowSize, arrowSize, -1.0f)
				};

				indices = new[]
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
					6, 2
				};
				break;
			}
		}

		GC.Array arrays = new();
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
}

using UnityEngine;
using System;
using System.Runtime.InteropServices;



public class Supershape : MonoBehaviour {
    public enum TransformType { None, Logarithmic, Trigonometric, TransferFunction, Polynomial };

    // Unity component interface params
	public float a_m = 1f;
    public float a_a = 1; 
    public float a_b = 1;
	public float a_n1 = 1f;
	public float a_n2 = 1f;
	public float a_n3 = 1f;
	public float b_m = 1f;
    public float b_a = 1; 
    public float b_b = 1;
	public float b_n1 = 1f;
	public float b_n2 = 1f;
	public float b_n3 = 1f;
	
    public float min_phi = (float) - Math.PI;
	public float max_phi = (float) Math.PI;
	public float min_theta = (float) - Math.PI/2;
    public float max_theta = (float)Math.PI / 2;

    public TransformType transform1_type;
    public float transform1_var1;
    public float transform1_var2;
    public float transform1_var3;
    public float transform1_var4;

    public TransformType transform2_type;
    public float transform2_var1;
    public float transform2_var2;
    public float transform2_var3;
    public float transform2_var4;

    /*
    public TransformType transform_type1;
    public float transform1Var1;
    public float transform1Var2;
    public float transform1Var3;
    public float transform1Var4;
    
    public TransformType transform_type2;
    public float transform2Var1;
    public float transform2Var2;
    public float transform2Var3;
    public float transform2Var4;
     */
	
    public int xSteps = 64;
	public int ySteps = 64;

	// 
	private Mesh m_SupershapeMesh;
	private Vector3[] m_Vertices;
	//private Color[] m_Colors;
	
	private GCHandle m_VerticesHandle;
		
    //[DllImport ("UnitySupershape")]
	//private static extern void GenerateSupershape(IntPtr vertices, IntPtr uv, IntPtr normals);

    //public void Supershape(Mesh mesh)
    //{
    //    m_SupershapeMesh = mesh;
    //}
	
    void Start () {
		


        // Create game object containing renderer
        //GameObject myGameObject = new GameObject();
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
		
		// retrieve a mesh instance
		m_SupershapeMesh = meshFilter.mesh;
		

		// make vertex data arrays
		m_Vertices = new Vector3[(xSteps+1) * (ySteps+1)];
		//m_Colors   = new Color[xSteps * ySteps];
		//m_VerticesHandle = GCHandle.Alloc(m_Vertices, GCHandleType.Pinned);
		
		CalcSupershape();
		
        //    Debug.Log("Game object has no renderer or gui texture to assign the generated texture to!");
    }
    
    void OnDisable() {
        // Free the pinned array handle.
        //m_PixelsHandle.Free();
    }
    
    // Now we can simply call UpdateTexture which gets routed directly into the plugin
    void Update () {
        //UpdateTexture (m_PixelsHandle.AddrOfPinnedObject(), m_Texture.width, m_Texture.height, Time.time);
        //m_Texture.SetPixels (m_Pixels, 0);
        //m_Texture.Apply ();
		
		//CalcSupershape();
    }
	
	
	// Supershape math (how to make these private .. ?
	//2D supershape Paul Burke		
	void Eval2D(float phi, ref float x, ref float y)
	{
		double r;
		double t1, t2;
		//float b_a = 1, b_b = 1;

		t1 = Math.Cos(b_m * phi / 4) / b_a;
		t1 = Math.Abs(t1);
		t1 = Math.Pow(t1, b_n2);

		t2 = Math.Sin(b_m * phi / 4) / b_b;
		t2 = Math.Abs(t2);
		t2 = Math.Pow(t2, b_n3);

		r = Math.Pow(t1 + t2, 1 / b_n1);
		if (Math.Abs(r) == 0)
		{
			x = 0;
			y = 0;
		}
		else
		{
			r = 1 / r;
			x = (float)(r * Math.Cos(phi));
			y = (float)(r * Math.Sin(phi));
		}
	}

	//3D supershape Paul Burke		
	void Eval3D(float phi, float theta, ref float x, ref float y, ref float z)
	{
		double r;
		double t1, t2;
		//float a_a = 1, a_b = 1;

		Eval2D(phi, ref x, ref y);

		t1 = Math.Cos(a_m * theta / 4) / a_a;
		t1 = Math.Abs(t1);
		t1 = Math.Pow(t1, a_n2);

		t2 = Math.Sin(a_m * theta / 4) / a_b;
		t2 = Math.Abs(t2);
		t2 = Math.Pow(t2, a_n3);

		r = Math.Pow(t1 + t2, 1 / a_n1);
		//if(Enable_Spiral)
		//	r *= Math.Log(phi);
		
		if (Math.Abs(r) == 0)
		{
			x = 0;
			y = 0;
			z = 0;
		}
		else
		{
			r = 1 / r;

            // Apply the R transform!
	        // Radial function
	        double rRadial = r;
            DoTransform(ref rRadial, phi, transform1_type, transform1_var1, transform1_var2, transform1_var3, transform1_var4);

	        x *= (float) rRadial * (float) Math.Cos(theta);
	        y *= (float) rRadial * (float) Math.Cos(theta);

	        double rVertical = r;
            DoTransform(ref rVertical, phi, transform2_type, transform2_var1, transform2_var2, transform2_var3, transform2_var4);
	        
	        z = (float) rVertical * (float) Math.Sin(theta);


		}

	}
	
    void DoTransform(ref double from, float phi, TransformType type, float var1, float var2, float var3, float var4)
    {
        switch (type)
        {
	        case TransformType.None:					// No Transform
		        break;
            case TransformType.Logarithmic:			// Logarithmic = Var1 * e^(Var2 * Theta)
                from *= var1 * Math.Exp(var2 * phi);
		        break;
            case TransformType.Trigonometric:			// Trigonometric = Var1 * cos (Var2 * Theta) + Var3 * sin (Var4 * Theta)
                from *= var1 * Math.Cos(var2 * phi) + var3 * Math.Sin(var4 * phi);
		        break;
            case TransformType.TransferFunction:		// Transfer Function  = (Var1 + Var2 * Theta)/(Var3 + Var4 * Theta)
                from *= (var1 + (var2 * phi)) / (var3 + (var4 * phi));
		        break;
            case TransformType.Polynomial:			// Polynomial = Var1 + Var2 * Theta + Var3 * Theta^2 + Var4 * Theta^3 + ...
                from *= var1 + (var2 * phi) + (var3 * Math.Pow(phi, 2.0f)) + (var4 * Math.Pow(phi, 3.0f));
		        // TODO: add the reset of the formula
		        break;
        }
    }

	void CalcSupershape()
	{
		float phi = -(float)Math.PI;
		//float addPhi = (float)Math.PI * 2 / xSteps;
		float theta;
		//float addTheta = (float)Math.PI / ySteps;
		
		for (int i = 0; i <= xSteps; i++)
		{
			phi = min_phi + ((float)i/xSteps) * (max_phi-min_phi);
			
			for (int j = 0; j <= ySteps; j++)
			{
				theta = min_theta + ((float)j/ySteps) * (max_theta - min_theta);
				
				
				Eval3D(phi, theta, ref m_Vertices[i*xSteps + j].x, ref m_Vertices[i*xSteps + j].y, ref m_Vertices[i*xSteps + j].z);

				//m_Colors[i*xSteps+j] = Color((i%10)/10.0, (i%10)/10.0, (i%10)/10.0, 0.9);
				//m_Colors[i*xSteps+j].r = (i%10)/10;
				//m_Colors[i*xSteps+j].g = (i%10)/10;
				//m_Colors[i*xSteps+j].b = (i%10)/10;
				//m_Colors[i*xSteps+j].a = 0.9f;
				
				/*
				// Calc normal ->
				// create vectors
				float v1x = x1 - x2; float v2x = x2 - x3;
				float v1y = y1 - y2; float v2y = y2 - y3;
				float v1z = z1 - z2; float v2z = z2 - z3;
				// Get cross product of vectors
				float nx = (v1y * v2z) - (v1z * v2y);
				float ny = (v1z * v2x) - (v1x * v2z);
				float nz = (v1x * v2y) - (v1y * v2x);
				// Normalise final vector
				float vLen = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
				*/
				
				/*
				Gl.glNormal3f(nx / vLen, ny / vLen, nz / vLen);
				Gl.glVertex3f(x1 * scale, y1 * scale, z1 * scale);
				Gl.glVertex3f(x2 * scale, y2 * scale, z2 * scale);
				Gl.glVertex3f(x3 * scale, y3 * scale, z3 * scale);
				Gl.glVertex3f(x4 * scale, y4 * scale, z4 * scale);
				*/
				
				//theta += addTheta;
			}
			//phi += addPhi;
		}
		
		// specify 2 triangles for each (i, j)
		// TODO: add closing seam(s)
		
		int vertexCount = m_Vertices.Length;
		int[] Triangles = new int[xSteps*ySteps*2*3];
		
		uint index = 0;
		for (int i = 0; i < xSteps; i++)
		{
			for (int j = 0; j < ySteps; j++)
			{
				Triangles[index++] = (i*xSteps + j) % vertexCount;
				Triangles[index++] = ((i+1)*xSteps + j) % vertexCount;
				Triangles[index++] = (i*xSteps + j+1) % vertexCount;
				
				Triangles[index++] = ((i+1)*xSteps + j) % vertexCount;
				Triangles[index++] = ((i+1)*xSteps + j+1) % vertexCount;
				Triangles[index++] = (i*xSteps + j+1) % vertexCount;
			}
		}
		m_SupershapeMesh.vertices = m_Vertices;
		//m_SupershapeMesh.colors = m_Colors;
		m_SupershapeMesh.triangles = Triangles;
		m_SupershapeMesh.RecalculateNormals();
	}

}


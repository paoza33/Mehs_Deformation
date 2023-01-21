using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleManager3D : MonoBehaviour
{
    public Material rippleMaterial;
    public Texture2D rippleTexture;
    public bool reflectiveBoundary;

    float[][] rippleN, rippleNm1, rippleNp1;

    float Lx = 10;
    float Ly = 10;
    [SerializeField] float dx = 0.1f; // x-axis density
    float dy { get => dx; } // y-axis density
    int nx, ny; // resolution

    public float CFL = 0.5f;
    public float c = 1;
    float dt; // time step
    float t; // current time
    [SerializeField] float floatToColorMultiplier = 2f; // emphasize color
    [SerializeField] float pulseFrenquency = 1f;
    [SerializeField] float pulseMagnitude = 1f;
    [SerializeField] Vector2Int pulsePosition = new Vector2Int(50,50);
    [SerializeField] float elasticity = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        nx = Mathf.FloorToInt(Lx / dx);
        ny = Mathf.FloorToInt(Ly / dy);
        rippleTexture = new Texture2D(nx, ny, TextureFormat.RGBA32, false);

        // creates empty field
        rippleN = new float[nx][];
        rippleNm1 = new float[nx][];
        rippleNp1 = new float[nx][];
        for(int i = 0; i < nx; i++)
        {
            rippleN[i] = new float[ny];
            rippleNm1[i] = new float[ny];
            rippleNp1[i] = new float[ny];
        }

        rippleMaterial.SetTexture("_MainTex", rippleTexture); //coloring texture
        rippleMaterial.SetTexture("_Displacement", rippleTexture); // displacement texture
    }

    void rippleStep()
    {
        dt = CFL * dx / c; // recalculate dt
        t += dx; // increment time

        if (reflectiveBoundary)
            ApplyReflectiveBoundary();
        else ApplyAbsorptiveBoundary();

        for (int i = 0; i < nx; i++)
        {
            for(int j =0; j<ny; j++)
            {
                rippleNm1[i][j] = rippleN[i][j]; // copy state at N to state N-1
                rippleN[i][j] = rippleNp1[i][j]; // copy state at n+1 to state N
            }
        }

        // dripping effect
        rippleN[pulsePosition.x][pulsePosition.y] = dt * dt * 20 * pulseMagnitude * Mathf.Cos(t * Mathf.Rad2Deg * pulseFrenquency);

        for (int i = 1; i < nx-1; i++) // do not process edges
        {
            for (int j = 1; j < ny-1; j++)
            {
                float n_ij = rippleN[i][j];
                float n_ip1j = rippleN[i + 1][j];
                float n_im1j = rippleN[i - 1][j];
                float n_ijp1 = rippleN[i][j+1];
                float n_ijm1 = rippleN[i][j-1];
                float nm1_ij = rippleNm1[i][j];
                rippleNp1[i][j] = 2f * n_ij - nm1_ij + CFL * CFL * (n_ijm1 + n_ijp1 + n_im1j + n_ip1j - 4f * n_ij); // wave equation
                rippleNp1[i][j] *= elasticity;
            }
        }
    }

    void ApplyMatrixToTexture(float[][] state, ref Texture2D tex, float floatToColorMultiplier)
    {
        for (int i = 0; i < nx; i++)
        {
            for (int j = 0; j < ny; j++)
            {
                float val = state[i][j] * floatToColorMultiplier;
                tex.SetPixel(i, j, new Color(val + 0.5f, val + 0.5f, val + 0.5f, 1f)); // paint grayscale
            }
        }
        tex.Apply();
    }

    void ApplyReflectiveBoundary()
    {
        for (int i = 0; i < nx; i++)
        {
            rippleN[i][0] = 0f;
            rippleN[i][ny - 1] = 0f;
        }

        for (int j = 0; j < ny; j++)
        {
            rippleN[0][j] = 0f;
            rippleN[ny - 1][j] = 0f;
        }
    }

    void ApplyAbsorptiveBoundary()
    {
        float v = (CFL - 1f) / (CFL + 1f);
        for (int i = 0; i < nx; i++)
        {
            rippleNp1[i][0] = rippleN[i][1] + v * (rippleNp1[i][1] - rippleN[i][0]);
            rippleNp1[i][ny - 1] = rippleN[i][ny-2] + v* (rippleNp1[i][ny-2] - rippleN[i][ny-1]);
        }

        for (int j = 0; j < ny; j++)
        {
            rippleNp1[0][j] = rippleN[1][j] + v * ( rippleNp1[1][j] - rippleN[0][j]);
            rippleNp1[ny - 1][j] = rippleN[ny - 2][j] + v * ( rippleNp1[ny - 2][j] - rippleN[ny - 1][j]);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StopAllCoroutines();
        Vector3 posImpact = transform.InverseTransformPoint(collision.contacts[0].point);
        pulsePosition = new Vector2Int(Mathf.Abs((int) posImpact.x -50) , Mathf.Abs((int) posImpact.z -50));
        elasticity = 0.99f;
        pulseMagnitude = 1.5f;
        StartCoroutine(RippleTime());
    }

    IEnumerator RippleTime()
    {
        yield return new WaitForSeconds(0.05f);
        elasticity -= 0.01f;
        pulseMagnitude -= 0.015f;
        if (elasticity >= 0f && pulseMagnitude >= 0)
            StartCoroutine(RippleTime());
    }

    // Update is called once per frame
    void Update()
    {
        rippleStep();
        ApplyMatrixToTexture(rippleN, ref rippleTexture, floatToColorMultiplier);
    }
}
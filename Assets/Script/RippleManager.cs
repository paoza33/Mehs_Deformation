using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleManager : MonoBehaviour
{
    public Material rippleMaterial;

    public ComputeShader rippleCompute;
    public RenderTexture NState, NminusState, NplusState; // keep information regarding the differents state of the ripple
    public Vector2Int resolution;

    public Vector3 centerEffect; // center of the ripple centerEffect (x coord, y coord, strength)
    public float dispersion = 0.98f; // control how many waves disappear in each cycle

    // Start is called before the first frame update
    void Start()
    {
        InitializeTexture(ref NState);
        InitializeTexture(ref NminusState);
        InitializeTexture(ref NplusState);

        rippleMaterial.mainTexture = NState;
    }

    void InitializeTexture (ref RenderTexture tex)
    {
        // this format can take negative value ( eesch cells can be negative or positive, when water is stagnant the value is zero)
        tex = new RenderTexture(resolution.x, resolution.y, 1, UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_SNorm);
        tex.enableRandomWrite = true;
        tex.Create();
    }

    // Update is called once per frame
    void Update()
    {

        Graphics.CopyTexture(NState, NminusState);
        Graphics.CopyTexture(NplusState, NState);


        rippleCompute.SetTexture(0, "NState", NState);
        rippleCompute.SetTexture(0, "NminusState", NminusState);
        rippleCompute.SetTexture(0, "NplusState", NplusState);
        rippleCompute.SetVector("centerEffect", centerEffect);
        rippleCompute.SetVector("resolution", new Vector2 (resolution.x, resolution.y));
        rippleCompute.SetFloat("dispersion", dispersion);
        rippleCompute.Dispatch(0, resolution.x / 8, resolution.y / 8, 1);
    }
}

using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FLayeredEditor : ShaderGUI
{
    bool LayerA, LayerB, LayerC, LayerD;
    bool LayerAFold, LayerBFold, LayerCFold, LayerDFold;
    int tempVar;

    void loadMatVariables(Material targetMat)
    {
        tempVar = targetMat.GetInt("_LayerASwitch");
        LayerAFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_LayerBSwitch");
        LayerBFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_LayerCSwitch");
        LayerCFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_LayerDSwitch");
        LayerDFold = tempVar == 1 ? true : false;
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;
        loadMatVariables(targetMat);

        Color[] BDColors = {
            new Color(0, 0 ,0 ,0),
            new Color(0.6f, 0.6f, 0.6f, 0.3f),
            new Color(0.4f, 0.0f, 0.0f, 0.3f)
        };

        GUIStyle style = new GUIStyle();

        #region Fiber Layered Shader Title
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/_Main/Art/Shaders/Editor/GUI/FiberLayeredShader.png", typeof(Texture));

        GUILayout.BeginArea(new Rect(0, 0, 512, 32));
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Box(banner, GUILayout.MinHeight(32));
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
        GUILayout.Space(32);
        #endregion

        #region Layer A Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        {
            EditorGUI.BeginChangeCheck();
            {
                LayerAFold = EditorGUILayout.Toggle("Layer A", LayerAFold);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (LayerAFold)
                {
                    targetMat.SetInt("_LayerASwitch", 1);
                }
                else
                {
                    targetMat.SetInt("_LayerASwitch", 0);
                }
            }
            EditorGUILayout.EndVertical();
            style.normal.background = MakeBackground(1, 1, BDColors[0]);
            if (LayerAFold)
            {
                MaterialProperty mc = ShaderGUI.FindProperty("_LayerAColor", properties);
                MaterialProperty mt = ShaderGUI.FindProperty("_LayerATexture", properties);

                EditorGUILayout.BeginVertical();
                {
                    materialEditor.ColorProperty(mc, "Main Color");
                    materialEditor.TextureProperty(mt, "Main Texture");
                }
                EditorGUILayout.EndVertical();

            }
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Layer B Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            LayerBFold = EditorGUILayout.Toggle("Layer B", LayerBFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (LayerBFold)
            {
                targetMat.SetInt("_LayerBSwitch", 1);
            }
            else
            {
                targetMat.SetInt("_LayerBSwitch", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (LayerBFold)
        {
            MaterialProperty lbc = ShaderGUI.FindProperty("_LayerBColor", properties);
            MaterialProperty lbt = ShaderGUI.FindProperty("_LayerBTexture", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(lbc, "Layer B Color");
                materialEditor.TextureProperty(lbt, "Layer B Texture");
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Space(5);
        #endregion

        #region Layer C Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            LayerCFold = EditorGUILayout.Toggle("Layer C", LayerCFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (LayerCFold)
            {
                targetMat.SetInt("_LayerCSwitch", 1);
            }
            else
            {
                targetMat.SetInt("_LayerCSwitch", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (LayerCFold)
        {
            MaterialProperty lcc = ShaderGUI.FindProperty("_LayerCColor", properties);
            MaterialProperty lct = ShaderGUI.FindProperty("_LayerCTexture", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(lcc, "Layer C Color");
                materialEditor.TextureProperty(lct, "Layer C Texture");
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Space(5);
        #endregion

        #region Layer D Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            LayerDFold = EditorGUILayout.Toggle("Layer D", LayerDFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (LayerDFold)
            {
                targetMat.SetInt("_LayerDSwitch", 1);
            }
            else
            {
                targetMat.SetInt("_LayerDSwitch", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (LayerDFold)
        {
            MaterialProperty ldc = ShaderGUI.FindProperty("_LayerDColor", properties);
            MaterialProperty ldt = ShaderGUI.FindProperty("_LayerDTexture", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(ldc, "Layer D Color");
                materialEditor.TextureProperty(ldt, "Layer D Texture");
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Space(5);
        #endregion
    }

    private Texture2D MakeBackground(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}


/*
       #region Main Group
        MaterialProperty mc = ShaderGUI.FindProperty("_MainColor1", properties);
        MaterialProperty mt = ShaderGUI.FindProperty("_MainTexture", properties);
        EditorGUILayout.BeginVertical();
        {
            materialEditor.ColorProperty(mc, "Main Color");
            materialEditor.TextureProperty(mt, "Main Texture");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
        #endregion

        #region Shade Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            shadeFold = EditorGUILayout.Toggle("Shade", shadeFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (shadeFold)
            {
                targetMat.SetInt("_ShadeFold", 1);
            }
            else
            {
                targetMat.SetInt("_ShadeFold", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (shadeFold)
        {
            MaterialProperty sc = ShaderGUI.FindProperty("_ShadeColor", properties);
            MaterialProperty tnzv = ShaderGUI.FindProperty("_ToonizeVar", properties);
            MaterialProperty sCont = ShaderGUI.FindProperty("_ShadingContrast", properties);
            MaterialProperty sCnt = ShaderGUI.FindProperty("_ShadingContribution", properties);
            MaterialProperty cellSharpness = ShaderGUI.FindProperty("_BaseCellSharpness", properties);
            MaterialProperty cellOffset = ShaderGUI.FindProperty("_BaseCellOffset", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(sc, "Shade Color");
                materialEditor.RangeProperty(sCont, "Shade Contrast");
                materialEditor.RangeProperty(sCnt, "Sub Surface Scatter");

                #region Check Toonize
                style.normal.background = MakeBackground(1, 1, BDColors[2]);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginVertical(style);
                {
                    checkToonize = EditorGUILayout.Toggle("Toonize", checkToonize);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (checkToonize)
                        {
                            targetMat.SetInt("_Toonize", 1);
                        }
                        else
                        {
                            targetMat.SetInt("_Toonize", 0);
                        }
                    }
                    if (checkToonize)
                    {
                        materialEditor.RangeProperty(tnzv, "Toonize Shade");
                    }
                }
                EditorGUILayout.EndVertical();
                #endregion

                #region Check Shade Detail
                EditorGUILayout.BeginVertical(style);
                {
                    EditorGUI.BeginChangeCheck();
                    checkShadeDetail = EditorGUILayout.Toggle("Cel Shade Finetune", checkShadeDetail);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (checkShadeDetail)
                        {
                            targetMat.SetInt("_ShadeFineTune", 1);
                        }
                        else
                        {
                            targetMat.SetInt("_ShadeFineTune", 0);
                        }
                    }
                    if (checkShadeDetail)
                    {
                        materialEditor.RangeProperty(cellSharpness, "Cel Sharpness");
                        materialEditor.RangeProperty(cellOffset, "Cel Offset");
                    }
                }
                EditorGUILayout.EndVertical();
                #endregion

            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion


        #region Specular Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            specularFold = EditorGUILayout.Toggle("Specular", specularFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (specularFold)
            {
                targetMat.SetInt("_Specular", 1);
            }
            else
            {
                targetMat.SetInt("_Specular", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (specularFold)
        {
            MaterialProperty spc = ShaderGUI.FindProperty("_SpecColor1", properties);
            MaterialProperty spi = ShaderGUI.FindProperty("_SpecularIntensity", properties);
            MaterialProperty spa = ShaderGUI.FindProperty("_SpecularAmbient", properties);
            MaterialProperty spg = ShaderGUI.FindProperty("_SpecularGlossy", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(spc, "Specular Color");
                materialEditor.RangeProperty(spi, "Specular Intensity");
                materialEditor.RangeProperty(spa, "Specular Ambient");
                materialEditor.RangeProperty(spg, "Specular Glossy");
            }

            #region Check Specular Toonize
            style.normal.background = MakeBackground(1, 1, BDColors[2]);
            EditorGUILayout.BeginVertical(style);
            {
                EditorGUI.BeginChangeCheck();
                checkSpecToonize = EditorGUILayout.Toggle("Specular Toonize", checkSpecToonize);
                if (EditorGUI.EndChangeCheck())
                {
                    if (checkSpecToonize)
                    {
                        targetMat.SetInt("_SpecularToonize", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_SpecularToonize", 0);
                    }
                }

                if (checkSpecToonize)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        MaterialProperty tSpec = ShaderGUI.FindProperty("_ToonizeSpecular", properties);

                        materialEditor.RangeProperty(tSpec, "Toonize Specular");
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Check Specular Detail
            style.normal.background = MakeBackground(1, 1, BDColors[2]);
            EditorGUILayout.BeginVertical(style);
            {
                EditorGUI.BeginChangeCheck();
                checkSpecDetail = EditorGUILayout.Toggle("Specular Cel Finetune", checkSpecDetail);
                if (EditorGUI.EndChangeCheck())
                {
                    if (checkSpecDetail)
                    {
                        targetMat.SetInt("_SpecularCelFinetune", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_SpecularCelFinetune", 0);
                    }
                }
                if (checkSpecDetail)
                {
                    MaterialProperty specCelIn = ShaderGUI.FindProperty("_SpecularCelIn", properties);
                    MaterialProperty specCelOut = ShaderGUI.FindProperty("_SpecularCelOut", properties);

                    materialEditor.RangeProperty(specCelIn, "Specular Cel In");
                    materialEditor.RangeProperty(specCelOut, "Specular Cel Out");
                }
            }
            EditorGUILayout.EndVertical();
            #endregion

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Rim Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            rimFold = EditorGUILayout.Toggle("Rim Light", rimFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (rimFold)
            {
                targetMat.SetInt("_RimLight", 1);
            }
            else
            {
                targetMat.SetInt("_RimLight", 0);
            }
        }
        EditorGUILayout.EndVertical();
        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (rimFold)
        {
            EditorGUI.BeginChangeCheck();
            {
                MultRim = EditorGUILayout.Toggle("Multiple Rim", MultRim);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (MultRim)
                {
                    targetMat.SetInt("_MultiplyRim", 1);
                }
                else
                {
                    targetMat.SetInt("_MultiplyRim", 0);
                }
            }
            MaterialProperty rimc = ShaderGUI.FindProperty("_RimColor", properties);
            MaterialProperty rimIntensity = ShaderGUI.FindProperty("_RimIntensity", properties);
            MaterialProperty rimBias = ShaderGUI.FindProperty("_RimBias", properties);
            MaterialProperty rimScale = ShaderGUI.FindProperty("_RimScale", properties);
            MaterialProperty rimPower = ShaderGUI.FindProperty("_RimPower", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(rimc, "Rim Color");
                materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
                materialEditor.RangeProperty(rimBias, "Rim Bias");
                materialEditor.RangeProperty(rimScale, "Rim Scale");
                materialEditor.RangeProperty(rimPower, "Rim Power");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Reflection
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            reflectFold = EditorGUILayout.Toggle("Reflection", reflectFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (reflectFold)
            {
                targetMat.SetInt("_Reflect", 1);
            }
            else
            {
                targetMat.SetInt("_Reflect", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);

        if (reflectFold)
        {
            MaterialProperty refC = ShaderGUI.FindProperty("_ReflectColor", properties);
            MaterialProperty refS = ShaderGUI.FindProperty("_ReflectionStrength", properties);
            MaterialProperty refMap = ShaderGUI.FindProperty("_ReflectMap1", properties);
            MaterialProperty refMapRot = ShaderGUI.FindProperty("_CubeMapRotate", properties);
            MaterialProperty refMapY = ShaderGUI.FindProperty("_CubemapYPosition", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(refC, "Reflect Color");
                materialEditor.RangeProperty(refS, "Reflect Intensity");
                materialEditor.TextureProperty(refMap, "Reflect Map");
                materialEditor.RangeProperty(refMapRot, "Reflect Rotate");
                materialEditor.RangeProperty(refMapY, "Reflect Y Position");
            }
            EditorGUILayout.EndVertical();

            #region Check Reflect Rim
            EditorGUI.BeginChangeCheck();
            checkReflectRim = EditorGUILayout.Toggle("Reflect Fresnel", checkReflectRim);
            if (EditorGUI.EndChangeCheck())
            {
                if (checkReflectRim)
                {
                    targetMat.SetInt("_ReflectionFresnel", 1);
                }
                else
                {
                    targetMat.SetInt("_ReflectionFresnel", 0);
                }
            }
            if (checkReflectRim)
            {
                #region Check Reflection Rim Invert
                EditorGUI.BeginChangeCheck();
                checkReflectRimInvert = EditorGUILayout.Toggle("Reflect Fresnel Invert", checkReflectRimInvert);
                if (EditorGUI.EndChangeCheck())
                {
                    if (checkReflectRimInvert)
                    {
                        targetMat.SetInt("_RefFresnelInvert", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_RefFresnelInvert", 0);
                    }
                }
                #endregion
                EditorGUILayout.BeginVertical();
                {
                    MaterialProperty refFresBias = ShaderGUI.FindProperty("_ReflectFresnelBias", properties);
                    MaterialProperty refFresScale = ShaderGUI.FindProperty("_ReflectFresnelScale", properties);
                    MaterialProperty refFresPower = ShaderGUI.FindProperty("_ReflectFresnelPower", properties);

                    materialEditor.RangeProperty(refFresBias, "Reflect Fresnel Bias");
                    materialEditor.RangeProperty(refFresScale, "Reflect Fresnel Scale");
                    materialEditor.RangeProperty(refFresPower, "Reflect Fresnel Power");
                }
                EditorGUILayout.EndVertical();

            }
            #endregion

        }
        #endregion

        #region Shader Defaults
        materialEditor.RenderQueueField();
        materialEditor.EnableInstancingField();
        #endregion
 */
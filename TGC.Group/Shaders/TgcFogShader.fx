/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))

								   //Textura para DiffuseMap
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
    Texture = (texDiffuseMap);
    ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;
};

// variable de fogs
float4 ColorFog;
float4 CameraPos;
float StartFogDistance;
float EndFogDistance;
float Density;

//Input del Vertex Shader
struct VS_INPUT_VERTEX
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT_VERTEX
{
    float4 Position : POSITION0;
    float2 Texture : TEXCOORD0;
    float4 PosView : COLOR0;
	float3 PosReal : TEXCOORD1;
};

//Vertex Shader
VS_OUTPUT_VERTEX vs_main(VS_INPUT_VERTEX input)
{
    VS_OUTPUT_VERTEX output;

	//Proyectar posicion
    output.Position = mul(input.Position, matWorldViewProj);
    output.Texture = input.Texture;
    output.PosView = mul(input.Position, matWorldView);
	float4 pos_real= mul(input.Position,matWorld);
	output.PosReal=float3(pos_real.x, pos_real.y, pos_real.z);
    return output;
}

//Input del Pixel Shader
struct PS_INPUT_PIXEL
{
    float2 Texture : TEXCOORD0;
    float1 Fog : FOG;
};

//Pixel Shader
float4 ps_main(VS_OUTPUT_VERTEX input) : COLOR0
{
    float zn = StartFogDistance;
    float zf = EndFogDistance;

    float4 fvBaseColor = tex2D(diffuseMap, input.Texture);

			if(input.PosReal.y < 0){
				if (input.PosView.z < zn){
					return fvBaseColor;
				}else if (input.PosView.z > zf)
				{
					fvBaseColor = ColorFog;
					return fvBaseColor;
				}
				else
				{	
					// combino fog y textura
					float total = zf - zn;
					float resto = input.PosView.z - zn;
					float proporcion = resto / total;
					fvBaseColor = lerp(fvBaseColor, ColorFog, proporcion);
					//fvBaseColor = lerp(fvBaseColor, ColorFog,0.2);
					//fvBaseColor =  (1-proporcion)* fvBaseColor + proporcion * ColorFog;
       
					return fvBaseColor;
				}
				
			}else{
			
			return fvBaseColor;
			}
		

}

//Heighmap
texture texHeighmap;

sampler2D heighmap=sampler_state
{
	Texture	= (texHeighmap);
	ADDRESSU = WRAP;
    ADDRESSV = WRAP;
    MINFILTER = LINEAR;
    MAGFILTER = LINEAR;
    MIPFILTER = LINEAR;

};

float time = 0;
//Pixel Shader
float4 ps_main2(VS_OUTPUT_VERTEX input) : COLOR0
{
    float zn = StartFogDistance;
    float zf = EndFogDistance;

    float4 fvBaseColor = tex2D(diffuseMap, input.Texture);

			if(input.PosReal.y < 0){
							if (input.PosView.z < zn){
										//float2(0, 0.007*sin(time*3));
										float2 desf = float2(0, 0.007*sin(time*3));
										float4 texelheigh= tex2D(heighmap, (input.Texture+desf)*4);
										if(texelheigh.r>0.15){
										return fvBaseColor*1.04;//float4(0.5,0.5,0.5,1);//texelheigh;
	
										}else{
	
										return fvBaseColor;
										}


								//return fvBaseColor;
							}else if (input.PosView.z > zf)
							{
								fvBaseColor = ColorFog;
								return fvBaseColor;
							}
							else
							{	
					
									float2 desf = float2(0 , 0.007*sin(time*3));
										float4 texelheigh= tex2D(heighmap, (input.Texture+desf)*4);
										if(texelheigh.r>0.15){
										fvBaseColor=fvBaseColor*1.04;//float4(0.5,0.5,0.5,1);//texelheigh;
	
										}
	
										

						
								// combino fog y textura
								float total = zf - zn;
								float resto = input.PosView.z - zn;
								float proporcion = resto / total;
								fvBaseColor = lerp(fvBaseColor, ColorFog, proporcion);
								//fvBaseColor = lerp(fvBaseColor, ColorFog,0.2);
								//fvBaseColor =  (1-proporcion)* fvBaseColor + proporcion * ColorFog;
       
								return fvBaseColor;
							}
				
			}else{
			
			return fvBaseColor;
			}
		

}
// ------------------------------------------------------------------
technique RenderScene
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}

technique RenderScene2
{
    pass Pass_0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main2();
    }
}
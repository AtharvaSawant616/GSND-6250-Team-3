Shader "Custom/WPS_ProjectionShader"
{
    Properties
    {
        _Tilling ("Tilling", Vector) = (1,1,1,1)
        _Offset ("UV Offset", Vector) = (0, 0, 0, 0)
        _Rotation_X ("UV Rotation_X", Range(0, 360)) = 0 
        _Rotation_Y ("UV Rotation_Y", Range(0, 360)) = 0
        _Rotation_Z ("UV Rotation_Z", Range(0, 360)) = 0
        _Color ("Color", Color) = (1,1,1,1)
        _DiffuseMap ("Diffuse (RGB)", 2D) = "white" {}
        _GlossinessMap ("Smoothness", 2D) = "white" {}
        _Smoothness ("Smoothness Factor", Range(0,1)) = 0.5   
        _HeightMap ("HeightMap", 2D) = "white" {} 
        _Height ("Height", Range(0,1)) = 0.5
        _NormalMap ("NormalMap", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0,1)) = 0.5
        _SpecularMap ("SpecularMap", 2D) = "black" {}
        _SpecularStrength ("Specular Factor", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        

        CGPROGRAM
        
        #pragma surface surf StandardSpecular fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _DiffuseMap;
        sampler2D _GlossinessMap;
        sampler2D _HeightMap;
        sampler2D _NormalMap;
        sampler2D _SpecularMap;
        float _Rotation_X; // 使用_Rotation属性
        float _Rotation_Y; // 使用_Rotation属性
        float _Rotation_Z; // 使用_Rotation属性
        float4 _Offset;
        float _NormalStrength;

//        Vector _Tilling;

        struct Input
        {
            float3 worldNormal;
            float3 worldPos;
            INTERNAL_DATA
        };

        half _Smoothness;  // 使用_Smoothness属性
        half _SpecularStrength;  // 使用_SpecularStrength属性
        fixed4 _Tilling;

        fixed4 _Color;


        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

// Rotation Matrix function
        float2 RotateUV(float2 uv, float angle)
        {
            // 将角度从度数转换为弧度
            float rad = radians(angle);
            float cosA = cos(rad);
            float sinA = sin(rad);

            // 使用旋转矩阵进行UV旋转
            float2x2 rotationMatrix = float2x2(cosA, -sinA, sinA, cosA);
            return mul(rotationMatrix, uv);  // 旋转后的UV坐标
        }

// Triplanar Mapping
        fixed4 TriplanarMapping(float3 worldPos, float3 worldNormal,sampler2D _InputMap, 
        float Rotation_X, float Rotation_Y, float Rotation_Z)
        {
            float3 blendWeights = abs(worldNormal);
            blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);
            float _Scale= _Tilling.x *_Tilling.y;
            
            float2 uvX = worldPos.yz * _Scale;
            uvX = _Offset.xy + RotateUV(uvX, Rotation_X); 
            float2 uvY = worldPos.xz * _Scale;
            uvY = _Offset.xy + RotateUV(uvY, Rotation_Y);
            float2 uvZ = worldPos.xy * _Scale;
            uvZ = _Offset.xy + RotateUV(uvZ, Rotation_Z);

            fixed4 texX = tex2D(_InputMap, uvX);
            fixed4 texY = tex2D(_InputMap, uvY);
            fixed4 texZ = tex2D(_InputMap, uvZ);

            return texX * blendWeights.x + texY * blendWeights.y + texZ * blendWeights.z;
        }
        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {

            // Albedo comes from a texture tinted by color
            fixed4 triplanarColor = TriplanarMapping(IN.worldPos, IN.worldNormal,
                                                    _DiffuseMap, _Rotation_X, 
                                                    _Rotation_Y, _Rotation_Z);
            fixed4 SpecularColor = TriplanarMapping(IN.worldPos, IN.worldNormal,
                                                    _SpecularMap, _Rotation_X,
                                                    _Rotation_Y, _Rotation_Z);
            fixed4 Glossiness = TriplanarMapping(IN.worldPos, IN.worldNormal,
                                                    _GlossinessMap, _Rotation_X,
                                                    _Rotation_Y, _Rotation_Z);
            fixed4 Height = TriplanarMapping(IN.worldPos, IN.worldNormal,
                                                    _HeightMap, _Rotation_X,
                                                    _Rotation_Y, _Rotation_Z);
            fixed4 NormalRGB = TriplanarMapping(IN.worldPos, IN.worldNormal,
                                                    _NormalMap, _Rotation_X,
                                                    _Rotation_Y, _Rotation_Z);
            //fixed3 Normal = UnpackNormal(NormalRGB) + _NormalStrength;  // 使用 UnpackNormal 处理法线
            //fixed4 c = tex2D (_DiffuseMap, xyPos) * _Color;
            //fixed4 s = tex2D (_SpecularMap, xyPos);
            //fixed3 n = UnpackNormal(tex2D(_NormalMap, xyPos));
            o.Albedo = triplanarColor.rgb * _Color.rgb;
            o.Specular = SpecularColor.rgb * _SpecularStrength;
            o.Smoothness = Glossiness.r * _Smoothness;
            //o.Height = Height.r;
            fixed3 normalRGB = tex2D(_NormalMap, NormalRGB.xy).rgb;
            
            

        }
        ENDCG
    
    
    }
    FallBack "Diffuse"
}

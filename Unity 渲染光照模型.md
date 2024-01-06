# 光照
## BRDF
BRDF （Bidirectional Reflectance Distribution Function，双向反射分布函数 )用来描述表面如何反射光线的方程。常见经验模型有
1. lambert漫反射
2. phong模型
3. blinn-phong模型
一个物体的光照，可以分为三个部分：环境光+漫反射+高光

## lambert漫反射
```hlsl
// 公式 float4 colorResult = ambient.color * diffuseColor * saturate(dot(worldSpaceNormal, worldSpaceLight));
float3 worldNormal = normalize(UnityObjectToWorldNormal(i.normal));
float3 worldView = normalize(_WorldSpaceLightPos0.xyz);
return float4(_LightColor0.rgb * _diffuseColor.rgb * saturate(dot(worldNormal, worldView)), 1);
```

## Q
* 漫反射使用的光
漫反射使用环境光的漫反射贴图颜色。
* 漫反射与高光的区别是反射强度？

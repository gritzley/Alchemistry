void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float DistanceAttenuation, out float ShadowAttenuation)
{
    // If not in shadergraph preview, where no light exists, calculate light values
    #ifndef SHADERGRAPH_PREVIEW

        // These are unity functions to get light information
        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        Light mainLight = GetMainLight(shadowCoord)

        // Pass light information back out
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAttenuation = mainLight.distanceAttenuation;
        ShadowAttenuation = mainLight.shadowAttenuation;

    // If we are in the shadergraph preview, pass back defaults
    #else
        
        Direction = normalize(float3(0.5f, 0.5f, 0.25f));
        Color = float3 (1.0f, 1.0f, 1.0f);
        DistanceAttenuation = 1.0f;
        ShadowAttenuation = 1.0f;

    #endif
}

void AdditionalLight_float(float3 WorldPos, int Index, out float3 Direction, out float3 Color, out float DistanceAttenuation, out float ShadowAttenuation)
{

    // Set default values for shadergraph preview and invalid light indices
    Direction = normalize(float3(0.5f, 0.5f, 0.25f));
    Color = float3 (0.0f, 0.0f, 0.0f);
    DistanceAttenuation = 0.0f;
    ShadowAttenuation = 0.0f;

    // If not in shadergraph preview, where no light exists, calculate light values
    #ifndef SHADERGRAPH_PREVIEW

        // check if index is in range
        int pixelLightCount = GetAdditionalLightsCount;
        if (Index < pixelLightCount)
        {
            // Get additional light by index
            Light light = GetAdditionalLight(Index, WorldPos);

            // Pass light information back out
            Direction = mainLight.direction;
            Color = mainLight.color;
            DistanceAttenuation = mainLight.distanceAttenuation;
            ShadowAttenuation = mainLight.shadowAttenuation;
        }
    #endif
}

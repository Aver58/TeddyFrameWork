#include "UnityPrefix.h"
#include "Picking.h"

#include "Runtime/Camera/Camera.h"
#include "Runtime/Camera/CameraUtil.h"
#include "Runtime/Camera/RenderManager.h"
#include "Runtime/Camera/Renderqueue.h"
#include "Runtime/Camera/CullResults.h"
#include "Runtime/Camera/RenderNodeQueuePrepareContext.h"
#include "Runtime/Camera/CameraCullingParameters.h"
#include "Runtime/Camera/CameraStack.h"
#include "Editor/Src/Gizmos/GizmoManager.h"
#include "Editor/Src/Gizmos/GizmoRenderer.h"
#include "Editor/Src/Prefabs/PrefabInstance.h"
#include "Editor/Src/Prefabs/Prefab.h"
#include "Editor/Src/EditorResources.h"
#include "Runtime/ParticleSystem/ParticleSystemRenderer.h"
#include "Runtime/GfxDevice/GfxDevice.h"
#include "Runtime/GfxDevice/InstancingBatcher.h"
#include "Runtime/Shaders/ShaderImpl/ShaderImpl.h"
#include "Runtime/Shaders/ShaderPassContext.h"
#include "Runtime/Graphics/LOD/LODGroupManager.h"
#include "Runtime/Graphics/Mesh/MeshFilter.h"
#include "Runtime/Graphics/Mesh/Mesh.h"
#include "Runtime/Graphics/Mesh/SpriteRenderer.h"
#include "Runtime/Graphics/GraphicsHelper.h"
#include "Runtime/Transform/RectTransform.h"
#include "Runtime/Graphics/SpriteUtility.h"
#include "Runtime/Graphics/RenderBufferManager.h"
#include "Runtime/Graphics/RenderTexture.h"
#include "Runtime/Graphics/IntermediateRendererManager.h"

// Things can't be picked below this alpha value
static float s_SelectionAlphaCutoutValue = 0.01f;
static const Camera* s_CurrentCamera = NULL;
static void RectfToViewPort(const Rectf& r, int* viewPort);
static PPtr<Shader> s_AlphaBasedSelectionShader = 0;


Material* GetPickMaterial()
{
    static Material* s_Material = NULL;
    if (s_Material == NULL)
    {
        Shader* shader = GetEditorResource<Shader>("SceneView/OpaqueSelection.shader");
        if (!shader || !shader->IsSupported())
        {
            ErrorString("Could not find scene view picking shader. Are editor resources out of date/broken?");
        }
        s_Material = Material::CreateMaterial(*shader, Object::kHideAndDontSave);
    }
    return s_Material;
}

Material *GetAlphaPickMaterial()
{
    static Material *s_matAlpha = NULL;
    if (s_matAlpha == NULL)
    {
        Shader* shader = GetEditorResource<Shader>("SceneView/AlphaBasedSelection.shader");
        if (!shader || !shader->IsSupported())
        {
            ErrorString("Could not find scene view alpha picking shader. Are editor resources out of date/broken?");
        }
        s_matAlpha = Material::CreateMaterial(*shader, Object::kHideAndDontSave);
    }
    return s_matAlpha;
}

Material *GetAlphaPickMaterialNoZWrite()
{
    static Material *s_matAlphaNoZWrite = NULL;
    if (s_matAlphaNoZWrite == NULL)
    {
        Shader* shader = GetEditorResource<Shader>("SceneView/AlphaBasedSelectionNoZWrite.shader");
        s_matAlphaNoZWrite = Material::CreateMaterial(*shader, Object::kHideAndDontSave);
    }
    return s_matAlphaNoZWrite;
}

static inline float MultiplyPointZ(const Matrix4x4f& m, const Vector3f& v)
{
    return m.m_Data[2] * v.x + m.m_Data[6] * v.y + m.m_Data[10] * v.z + m.m_Data[14];
}

static float EvaluateObjectDepth(const RenderNode& node)
{
    float outDistanceForSort;

    Vector3f center = node.rendererData.m_TransformInfo.worldAABB.GetCenter();
    if (s_CurrentCamera->GetOrthographic())
    {
        outDistanceForSort = MultiplyPointZ(s_CurrentCamera->GetProjectionMatrix(), center);
    }
    else
    {
        center -= s_CurrentCamera->GetPosition();
        outDistanceForSort = -SqrMagnitude(center);
    }

    return outDistanceForSort;
}

static bool SortByObjectDepth(const RenderNode& lhs, const RenderNode& rhs)
{
    int result = CompareGlobalLayeringData(lhs.rendererData.m_GlobalLayeringData, rhs.rendererData.m_GlobalLayeringData);
    if (!result)
    {
        return EvaluateObjectDepth(lhs) < EvaluateObjectDepth(rhs);
    }
    return result < 0;
}

static Object* GetPickingObjectFromRenderer(BaseRenderer* renderer)
{
    Assert(renderer != NULL);
    if (renderer->IsRenderer())
        return static_cast<Renderer*>(renderer);
    else if (renderer->IsIntermediateRenderer())
        return dynamic_instanceID_cast<Object*>(static_cast<IntermediateRenderer*>(renderer)->GetComponentInstanceID());
    return NULL;
}

class PickingCuller
{
public:
    explicit PickingCuller(Camera& camera, bool sort, bool requiresGeometryJobs, const std::set<GameObject*>* ignore, const std::set<GameObject*>* filter) : m_Camera(camera), m_Nodes(kMemTempJobAlloc)
    {
        m_OriginalIntermediateRenderers = GetIntermediateRendererManager().GetIntermediateRenderers(camera.GetInstanceID()).GetRendererCount();  //The camera's intermediate renderer may not have been added yet?

        if (filter == NULL)
        {
            CameraCullingParameters parameters(camera, kCullFlagForceEvenIfCameraIsNotActive);
            camera.CustomCull(parameters, m_CullResults);
            SyncFenceCullResults(m_CullResults);

            if (requiresGeometryJobs)
            {
                RendererCullingCallbackProperties rendererCullParams(camera, camera.GetWorldToCameraMatrix());
                DispatchGeometryJobs(m_CullResults.rendererCullCallbacks, rendererCullParams);
            }

            ExtractSceneRenderNodeQueue(m_CullResults, kDefaultPickingExtractionFlags, m_Nodes);

            if (ignore != NULL)
                Remove(ignore);
        }
        else
        {
            dynamic_array<BaseRenderer*> filteredRenderers(kMemTempAlloc);
            filteredRenderers.reserve(filter->size());

            for (std::set<GameObject*>::const_iterator i = filter->begin(); i != filter->end(); ++i)
            {
                GameObject* go = *i;
                Renderer* renderer = go->QueryComponent<Renderer>();
                if (renderer == NULL)
                    continue;

                if (ignore == NULL || ignore->find(go) == ignore->end())
                    filteredRenderers.push_back(renderer);
            }
            AddRenderersToRenderNodeQueue(filteredRenderers.begin(), filteredRenderers.size(), m_Nodes);
        }

        if (sort)
        {
            Assert(s_CurrentCamera == NULL);
            s_CurrentCamera = &camera;

            std::sort(m_Nodes.GetNodeArray(), m_Nodes.GetNodeArray() + m_Nodes.GetRenderNodesCount(), SortByObjectDepth);

            s_CurrentCamera = NULL;
        }
    }

    void Remove(const std::set<GameObject*>* gameObjects)
    {
        for (int i = m_Nodes.GetRenderNodesCount() - 1; i >= 0; i--)
        {
            Unity::Component* component = dynamic_pptr_cast<Unity::Component*>(GetPickingObjectFromRenderer(m_Nodes.GetNode(i).GetBaseRendererMainThread()));
            if (component != NULL)
            {
                if (gameObjects->find(component->GetGameObjectPtr()) != gameObjects->end())
                {
                    RenderNode& node = m_Nodes.GetNode(i);
                    if (node.cleanupCallback)
                        node.cleanupCallback(m_Nodes, i);
                    m_Nodes.EraseNode(i);
                }
            }
        }
    }

    ~PickingCuller()
    {
        GetIntermediateRendererManager().ClearIntermediateRenderers(m_Camera.GetInstanceID(), m_OriginalIntermediateRenderers);
        m_Nodes.SyncDependentJobs();
    }

    const RenderNodeQueue& GetResults() const { return m_Nodes; }

private:
    Camera& m_Camera;

    RenderNodeQueue m_Nodes;
    CullResults m_CullResults;
    size_t m_OriginalIntermediateRenderers;
};

static bool ShouldSkipRendererForPicking(BaseRenderer* renderer)
{
    // skip in locked layers
    if (GetTagManager().GetLockedPickingLayers() & renderer->GetLayerMask())
        return true;

    if (renderer->IsRenderer())
    {
        Renderer* r = static_cast<Renderer*>(renderer);

        if (r->TestHideFlag(Object::kHideInHierarchy))
            return true;
        if (r->GetShadowCastingMode() == kShadowCastingShadowsOnly)
            return true;

        // skip in locked sorting layers
        int layerIndex = GetTagManager().GetSortingLayerIndexFromValue(r->GetCachedSortingLayer());
        if (GetTagManager().GetSortingLayerLocked(layerIndex))
            return true;

        return false;
    }
    else if (renderer->IsIntermediateRenderer())
        return static_cast<IntermediateRenderer*>(renderer)->GetComponentInstanceID() == 0;

    return true;
}

static const ShaderLab::FastPropertyName kSLPropMainTex = ShaderLab::Property("_MainTex");
static const ShaderLab::FastPropertyName kSLPropSelectionAlphaCutoff = ShaderLab::Property("_SelectionAlphaCutoff");
static const ShaderLab::FastPropertyName kSLPropSelectionId = ShaderLab::Property("_SelectionID");
static const ShaderLab::FastPropertyName kSLPropCutoff = ShaderLab::Property("_Cutoff");

class SelectionRenderQueue
{
public:
    SelectionRenderQueue(UInt32 layerMask) { m_LayerMask = layerMask; }

    static ColorRGBA32 EncodeIndex(UInt32 index)
    {
        // output is R,G,B,A bytes; index is ABGR dword
        ColorRGBA32 color;
        color.r = index & 0xFF;
        color.g = (index >> 8) & 0xFF;
        color.b = (index >> 16) & 0xFF;
        color.a = (index >> 24) & 0xFF;
        return color;
    }

    static UInt32 DecodeIndex(UInt32 argb)
    {
        // input is color from ARGB32 format pixel; index is ABGR dword
        #if PLATFORM_ARCH_LITTLE_ENDIAN
        // ARGB32 in memory on little endian looks like BGRA in dword
        return ((argb >> 8) & 0xFFFFFF) | ((argb & 0xFF) << 24);
        #else
        // ARGB32 in memory on bit endian looks like ARGB in dword
        return (argb & 0xFF00FF00) | ((argb & 0xFF) << 16) | ((argb >> 16) & 0xFF);
        #endif
    }

    Object* GetObject(UInt32 color, int* materialIndex)
    {
        DebugAssert(materialIndex);
        *materialIndex = -1;

#if PLATFORM_LINUX
        // FIXME: Not correctly setting transparency for editor windows?
        // Hopefully we don't get more than 2^24 renderers/gizmos...
        color = (color & 0xFFFFFF00);
#endif
        UInt32 index = DecodeIndex(color);
        if (index == 0)
            return NULL;
        index--;
        int gizmoIndex = index - m_Renderers.size();
        if (index < m_Renderers.size())
        {
            *materialIndex = m_Renderers[index].second;
            return GetPickingObjectFromRenderer(m_Renderers[index].first);
        }
        else if (gizmoIndex < m_Gizmos.size())
            return dynamic_instanceID_cast<Object*>(m_Gizmos[gizmoIndex]);
        else
            return NULL;
    }

    static void BeforePickGizmoFunctor(void* userData, const Object* object)
    {
        Assert(userData && object);
        SelectionRenderQueue* self = reinterpret_cast<SelectionRenderQueue*>(userData);

        int gizmoIndex = self->m_Gizmos.size() + self->m_Renderers.size();
        int instanceID = object->GetInstanceID();
        self->m_Gizmos.push_back(instanceID);
        gizmos::g_GizmoPickID = EncodeIndex(gizmoIndex + 1);
    }

    void Render(Material& pickingMaterial, Material& alphaPickingMaterial, Material& alphaPickingMaterialNoZWrite, float alphaCutoff, PickingCuller& pickingCuller, std::set<GameObject*> *ignore, std::set<GameObject*> *filter)
    {
        GfxDevice& device = GetGfxDevice();

        const RenderNodeQueue& contents = pickingCuller.GetResults();

        size_t nodesCount = contents.GetRenderNodesCount();

        if (nodesCount != 0)
        {
            const int kAverageNumberOfMaterialsPerRenderer = 4;
            m_Renderers.reserve(nodesCount * kAverageNumberOfMaterialsPerRenderer);

            // render objects for picking
            for (int i = 0; i < nodesCount; ++i)
            {
                const RenderNode& node = contents.GetNode(i);
                BaseRenderer* baseRenderer = node.GetBaseRendererMainThread();

                Assert(baseRenderer != NULL);
                if (ShouldSkipRendererForPicking(baseRenderer))
                    continue;

                Matrix4x4f matView = device.GetViewMatrix();
                Matrix4x4f matWorld = device.GetWorldMatrix();

                SetupObjectMatrix(node.rendererData.m_TransformInfo.worldMatrix, node.rendererData.m_TransformInfo.transformType);
                DebugAssert(node.executeCallback);

                ShaderPropertySheet &props = GetDefaultPassContext().properties;

                // uniforms for alphaPickingMaterial (AlphaBasedSelection.shader)
                props.SetFloat(kSLPropSelectionAlphaCutoff, alphaCutoff);

                for (int m = 0; m < baseRenderer->GetMaterialCount(); m++)
                {
                    Material* baseMaterial = baseRenderer->GetMaterial(m);
                    Shader* baseShader = baseMaterial != NULL ? baseMaterial->GetShader() : NULL;

                    m_Renderers.push_back(std::make_pair(baseRenderer, m));
                    ColorRGBA32 encoded = EncodeIndex(m_Renderers.size());
                    props.SetVector(kSLPropSelectionId, Vector4f(ByteToNormalized(encoded[0]), ByteToNormalized(encoded[1]), ByteToNormalized(encoded[2]), ByteToNormalized(encoded[3])));

                    // see if we have a Picking pass in the shader...
                    ShaderLab::Pass* pickingPass = NULL;
                    int pickingPassIndex = -1;
                    for (int p = 0, pn = baseShader != NULL ? baseShader->GetShaderLabShader()->GetActiveSubShader().GetValidPassCount() : 0; p < pn; ++p)
                    {
                        ShaderLab::Pass* pass = baseShader->GetShaderLabShader()->GetActiveSubShader().GetPass(p);
                        ShaderLab::TagMap::const_iterator found = pass->GetTags().find(shadertag::kLightMode);
                        if ((found != pass->GetTags().end()) && (shadertag::kPicking == found->second))
                        {
                            pickingPass = pass;
                            pickingPassIndex = p;
                            break;
                        }
                    }

                    if (pickingPass != NULL)
                    {
                        Instancing::SetupKeywordsForSingleRenderNode(GetDefaultPassContext().keywords, node, pickingPass, baseMaterial);

                        ShaderLab::SubPrograms subPrograms;
                        VertexInputMasks vertexInput = baseMaterial->SetPassFast(pickingPass, GetDefaultPassContext(), baseShader, pickingPassIndex, true, &subPrograms);
                        if (vertexInput.IsValid())
                        {
                            if (!Instancing::RenderSingleRenderNodeWithInstancing(GetDefaultPassContext().keywords, contents, i, baseRenderer->GetSubsetIndex(m), baseMaterial, subPrograms, vertexInput))
                                node.executeCallback(contents, i, vertexInput, baseRenderer->GetSubsetIndex(m));
                        }

                        Instancing::DisableKeywords(GetDefaultPassContext().keywords);
                        continue;
                    }

                    // We'll want raster state from original shader, to get the correct culling mode
                    const DeviceRasterState* origRasterState = NULL;
                    if (baseShader)
                        origRasterState = baseShader->GetShaderLabShader()->GetActiveSubShader().GetPass(0)->GetState().GetRasterState();

                    bool useAlpha;
                    bool ZWrite;
                    ShaderTagID subshaderTypeID;
                    GetPickingSettings(baseMaterial, baseShader, useAlpha, ZWrite, subshaderTypeID);

                    Material* useForPicking = &alphaPickingMaterial;
                    if (!ZWrite)
                        useForPicking = &alphaPickingMaterialNoZWrite;

                    int usedSubshaderIndex = useForPicking->GetShader()->GetSubShaderWithTagValue(shadertag::kRenderType, subshaderTypeID);
                    if (usedSubshaderIndex < 0)
                        useAlpha = false;

                    // If original material has _MainTex and RenderType is Transparent or TransparentCutout, we use alphaPickingMaterial (AlphaBasedSelection.shader)
                    if (useAlpha)
                    {
                        DebugAssert(baseMaterial);

                        if (baseMaterial && baseMaterial->HasProperty(kSLPropCutoff))
                            useForPicking->SetFloat(kSLPropCutoff, baseMaterial->GetFloat(kSLPropCutoff));
                        else
                            useForPicking->SetFloat(kSLPropCutoff, 0);

                        if (baseMaterial && baseMaterial->HasProperty(kSLPropMainTex))
                        {
                            useForPicking->SetTexture(kSLPropMainTex, baseMaterial->GetTexture(kSLPropMainTex));
                            Vector4f offsetAndScale = baseMaterial->GetTextureScaleAndOffset(kSLPropMainTex);
                            useForPicking->SetTextureScaleAndOffset(kSLPropMainTex, Vector2f(offsetAndScale.x, offsetAndScale.y), Vector2f(offsetAndScale.z, offsetAndScale.w));
                        }
                        else
                            useForPicking->SetTexture(kSLPropMainTex, NULL);

                        ShaderLab::SubShader& subshader = useForPicking->GetShader()->GetShaderLabShader()->GetSubShader(usedSubshaderIndex);
                        int shaderPassCount = subshader.GetValidPassCount();
                        for (int p = 0; p < shaderPassCount; ++p)
                        {
                            VertexInputMasks vertexInput = useForPicking->SetPassSlow(p, GetDefaultPassContext(), usedSubshaderIndex);
                            if (vertexInput.IsValid())
                            {
                                if (origRasterState)
                                    device.SetRasterState(origRasterState);
                                node.executeCallback(contents, i, vertexInput, baseRenderer->GetSubsetIndex(m));
                            }
                        }
                    }
                    else
                    {
                        VertexInputMasks vertexInput = pickingMaterial.SetPassSlow(0, GetDefaultPassContext());
                        if (vertexInput.IsValid())
                        {
                            if (origRasterState)
                                device.SetRasterState(origRasterState);
                            node.executeCallback(contents, i, vertexInput, baseRenderer->GetSubsetIndex(m));
                        }
                    }
                }

                GraphicsHelper::SetWorldViewAndProjection(device, &matWorld, &matView, NULL);
            }
        }

        device.SetBackfaceMode(false);

        // render gizmos for picking
        GizmoManager::Get().DrawAllGizmosWithFunctor(&pickingMaterial, BeforePickGizmoFunctor, ignore, filter, this);
    }

    typedef std::vector<std::pair<BaseRenderer*, int> > Renderers;
    Renderers m_Renderers;
    typedef std::vector<int> Gizmos;
    Gizmos m_Gizmos;
    UInt32 m_LayerMask;
};

void GetPickingSettings(Material* baseMaterial, Shader* baseShader, bool& useAlpha, bool& zWrite, ShaderTagID& subshaderTypeID)
{
    useAlpha = true;
    if (!baseMaterial)
        useAlpha = false;

    subshaderTypeID = ShaderTagID::Invalid();
    if (baseShader)
    {
        subshaderTypeID = baseMaterial->GetTag(shadertag::kRenderType, true);
    }
    if (!subshaderTypeID.IsValid())
        useAlpha = false;

    zWrite = true;
    if (baseShader)
    {
        const DeviceDepthState* depthState = baseShader->GetShaderLabShader()->GetActiveSubShader().GetPass(0)->GetState().GetDepthState();
        // case 744588 : GrabPass shaders could have NULL states, they are not created by default for GrabPass shaders
        if (depthState)
            zWrite = depthState->sourceState.depthWrite;
    }

    if (baseMaterial && !baseMaterial->HasProperty(kSLPropMainTex))
        useAlpha = false;
}

Object* PickClosestObject(Camera& cam, UInt32 layers, const Vector2f& pos, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, int* pickedMaterialIndex)
{
    // Find all objects at pickray
    SelectionList selection;
    PickObjects(cam, layers, pos, Vector2f(4, 4), ignore, filter, &selection);

    // Find object with largest score of all returned objects (most pixels in picking area)
    int bestScore = -1;
    Object* closestObject = NULL;
    for (SelectionList::iterator i = selection.begin(); i != selection.end(); i++)
    {
        if (i->score > bestScore)
        {
            bestScore = i->score;
            closestObject = i->object;
            if (pickedMaterialIndex)
                *pickedMaterialIndex = i->materialIndex;
        }
    }

    if (closestObject == NULL)
        return NULL;

    return closestObject;
}

GameObject* PickClosestGO(Camera& cam, UInt32 layers, const Vector2f& pos, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, int* pickedMaterialIndex)
{
    Object* object = PickClosestObject(cam, layers, pos, ignore, filter, pickedMaterialIndex);
    if (object == NULL)
        return NULL;
    if (object->Is<GameObject>())
        return static_cast<GameObject*>(object);
    else if (object->Is<Unity::Component>())
        return static_cast<Unity::Component*>(object)->GetGameObjectPtr();
    else
        return NULL;
}

static void ExtendVisibleAsChild(GameObject& obj, dynamic_array<GameObject*>& result)
{
    Transform& trx = obj.GetComponent<Transform>();

    for (Transform::iterator i = trx.begin(); i != trx.end(); ++i)
    {
        GameObject* go = (*i)->GetGameObjectPtr();
        if (go->IsMarkedVisible() == GameObject::kVisibleAsChild)
        {
            result.push_back(go);
            ExtendVisibleAsChild(*go, result);
        }
    }
}

/** Add all descendants to objs that has visibility set to kVisibleAsChild.
    For each object, as soon as an descendant is found not having this
    visiblity then no more descendants are added.
*/
static void ExtendVisibleAsChild(std::set<GameObject*>& objs)
{
    dynamic_array<GameObject*> toBeAdded;

    for (std::set<GameObject*>::const_iterator i = objs.begin(); i != objs.end(); ++i)
        ExtendVisibleAsChild(**i, toBeAdded);

    objs.insert(toBeAdded.begin(), toBeAdded.end());
}

int PickObjects(Camera& cam, UInt32 layers, const Vector2f& pos, const Vector2f& size, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, SelectionList* selection)
{
    // The code early exits at different spots but must undo some changes it does at the beginning, so we use the utility below
    struct ScopedPickOperations
    {
        ScopedPickOperations()
        {
            oldCurrentCamera = GetRenderManager().GetCurrentCameraPtr();
            oldStackState = GetRenderManager().GetCurrentCameraStackStatePtr();
            gizmos::g_GizmoPicking = true;
        }

        ~ScopedPickOperations()
        {
            gizmos::g_GizmoPicking = false;
            GetRenderManager().SetCurrentCameraAndStackState(oldCurrentCamera, oldStackState);
        }

        Camera* oldCurrentCamera;
        CameraStackRenderingState* oldStackState;
    } scopedPickOperations;

    //@TODO: Try removing this...
    AutoScopedCameraStackRenderingState scopedStackRenderState(cam);

    RenderManager::UpdateAllRenderers();

    if (ignore != NULL && ignore->size() > 0)
    {
        // First make sure children to ignore objects have their descendants with
        // kVisibleAsChild added to the ignore list so that they cannot be picked.
        ExtendVisibleAsChild(*ignore);
    }

    SelectionRenderQueue renderqueue(layers);

    GfxDevice& device = GetGfxDevice();

    // Make sure we are in
    RenderTexture::SetBackbufferActive();

    // Setup viewport
    RectInt viewPort = RectfToRectInt(cam.GetScreenViewportRect());

    // render objects for picking
    int minx = RoundfToInt(pos.x - size.x / 2.0F);
    int miny = RoundfToInt(pos.y - size.y / 2.0F);
    int maxx = RoundfToInt(pos.x + size.x / 2.0F);
    int maxy = RoundfToInt(pos.y + size.y / 2.0F);
    minx = std::max(minx, viewPort.x);
    miny = std::max(miny, viewPort.y);
    maxx = std::min(maxx, viewPort.x + viewPort.width - 1);
    maxy = std::min(maxy, viewPort.y + viewPort.height - 1);
    int xSize = maxx - minx;
    int ySize = maxy - miny;

    if (xSize <= 0 || ySize <= 0)
        return 0;

    // Create an RT that is the size of the pick area, and use it as a window to look at the scene at the required pick location
    // Note that it is in linear space as we don't want any sRGB mumbo jumbo to screw up our IDs
    RenderTexture* renderTarget = GetRenderBufferManager().GetTextures().GetTempBuffer(xSize, ySize, kDepthFormatMin16bits_NoStencil, kRTFormatARGB32, kRTReadWriteLinear, RenderBufferManager::Textures::kFlagsNone, kVRTextureUsageNone, 1, kMemorylessNone);
    RenderTexture::SetActive(renderTarget, 0, kCubeFaceUnknown, 0, RenderTexture::kFlagNone);

    RectInt pickViewport(0, 0, xSize, ySize);
    device.SetViewport(pickViewport);

    // Setup view and projection matrices to peek a window through the original viewport, this is kinda like scissoring but without using scissoring
    // and we use an RT of only the size of the pick rectangle instead of the entire frame buffer
    Matrix4x4f matProj, matVPScale(Matrix4x4f::kIdentity);
    matVPScale.Scale(Vector3f(viewPort.width / (float)pickViewport.width, viewPort.height / (float)pickViewport.height, 1));
    MultiplyMatrices4x4(&matVPScale, &cam.GetProjectionMatrix(), &matProj);

    const float offsetX = (pos.x - viewPort.x - viewPort.width * 0.5f) / (pickViewport.width * 0.5f);
    const float offsetY = (pos.y - viewPort.y - viewPort.height * 0.5f) / (pickViewport.height * 0.5f);
    if (cam.GetOrthographic())
    {
        matProj.Get(0, 3) -= offsetX;
        matProj.Get(1, 3) -= offsetY;
    }
    else
    {
        matProj.Get(0, 2) += offsetX;
        matProj.Get(1, 2) += offsetY;
    }
    gizmos::g_GizmoPickingViewportTransform = matVPScale;
    gizmos::g_GizmoPickingViewportTransform.Get(0, 3) -= offsetX;
    gizmos::g_GizmoPickingViewportTransform.Get(1, 3) -= offsetY;

    const Matrix4x4f& matView = cam.GetWorldToCameraMatrix();
    GraphicsHelper::SetWorldViewAndProjection(device, NULL, &matView, &matProj);

    // Clear buffer
    device.Clear(kGfxClearAll, ColorRGBAf(0, 0, 0, 0), 1.0f, 0);

    Assert(device.IsInsideFrame());

    // Render
    PickingCuller pickingCuller(cam, true, true, ignore, filter);
    renderqueue.Render(*GetPickMaterial(), *GetAlphaPickMaterial(), *GetAlphaPickMaterialNoZWrite(), s_SelectionAlphaCutoutValue, pickingCuller, ignore, filter);

    device.FinishRendering();
    GfxDevice::GetGeometryJobs().EndGeometryJobFrame(device);

    Image image(xSize, ySize, kTexFormatARGB32);
    bool readOk = device.ReadbackImage(image, 0, 0, xSize, ySize, 0, 0);

    RenderTexture::SetBackbufferActive();
    GetRenderBufferManager().GetTextures().ReleaseTempBuffer(renderTarget);

    // On MacOSX ReadbackImage may fail with default framebuffer size being 0.
    // I could not find an explanation for this so we just try to handle that fail case clearly
    if (!readOk)
        return 0;

    const UInt32* pixel = (const UInt32*)image.GetImageData();
    for (int i = 0; i < xSize * ySize; ++i)
    {
        SelectionEntry entry;
        entry.score = 1;
        entry.object = renderqueue.GetObject(pixel[i], &entry.materialIndex);

        if (entry.object)
        {
            Unity::Component *component = dynamic_pptr_cast<Unity::Component*>(entry.object);
            if (component)
            {
                GameObject *go = component->GetGameObjectPtr();
                while (go->IsMarkedVisible() == GameObject::kVisibleAsChild)
                {
                    Transform *parent = go->GetComponent<Transform>().GetParent();
                    if (parent)
                    {
                        go = parent->GetGameObjectPtr();
                        if (ignore != NULL && ignore->find(go) == ignore->end())
                        {
                            entry.object = go;
                        }
                    }
                    else
                        break;
                }
            }

            SelectionList::iterator found = selection->insert(entry).first;
            SelectionEntry& foundEntry = const_cast<SelectionEntry&>(*found);
            ++foundEntry.score;
        }
    }

    return selection->size();
}

static void RectfToViewPort(const Rectf& r, int* viewPort)
{
    viewPort[0] = RoundfToInt(r.x);
    viewPort[1] = RoundfToInt(r.y);
    viewPort[2] = RoundfToInt(r.Width());
    viewPort[3] = RoundfToInt(r.Height());
}

bool BoundsInRect(const AABB& bounds, const Rectf& rect, const Matrix4x4f& objectMatrix)
{
    Vector3f min = bounds.CalculateMin();
    Vector3f max = bounds.CalculateMax();
    Vector3f p;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(min.x, min.y, min.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(max.x, min.y, min.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(min.x, max.y, min.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(max.x, max.y, min.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(min.x, min.y, max.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(max.x, min.y, max.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(min.x, max.y, max.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    objectMatrix.PerspectiveMultiplyPoint3(Vector3f(max.x, max.y, max.z), p);
    if (!rect.Contains(p.x, p.y))
        return false;
    return true;
}

struct UserData
{
    std::set<GameObject*>* list;
    Camera *camera;
    Rectf *viewRect;
};

//static bool RectTransformInRect(Rect& screenrect)

static void CheckGizmosInRect(void* userData, const Object* object)
{
    UserData *data = (UserData*)userData;
    GameObject *go = dynamic_pptr_cast<GameObject*>(object);

    if (!go)
    {
        Unity::Component *comp = dynamic_pptr_cast<Unity::Component*>(object);
        if (comp)
            go = &comp->GetGameObject();
    }

    if (!go)
        return;

    if (!go->IsMarkedVisible())
        return;

    if (GetTagManager().GetLockedPickingLayers() & go->GetLayerMask())
        return;

    Transform& transform = go->GetComponent<Transform>();
    UI::RectTransform* rectTransform = dynamic_pptr_cast<UI::RectTransform*>(&transform);

    if (rectTransform)
    {
        Vector3f points[4];
        rectTransform->GetWorldCorners(points);

        for (int i = 0; i < 4; i++)
        {
            Vector3f screenPoint = data->camera->WorldToViewportPoint(points[i]);
            if (!data->viewRect->Contains(screenPoint.x, screenPoint.y))
                return;
        }

        if (data->list->find(go) == data->list->end())
            data->list->insert(go);

        return;
    }

    Vector3f p = data->camera->WorldToViewportPoint(transform.GetPosition());
    if ((p.z >= 0 || data->camera->GetOrthographic()) && data->viewRect->Contains(p.x, p.y))
    {
        if (data->list->find(go) == data->list->end())
            data->list->insert(go);
    }
}

bool IsPickingInProgress()
{
    return gizmos::g_GizmoPicking;
}

void PickRectObjects(Camera& cam, const Rectf& rect, std::set<GameObject*>* result, bool selectPrefabAssetsOnly)
{
    PickingCuller pickingCuller(cam, true, false, NULL, NULL);

    Matrix4x4f worldToView;
    MultiplyMatrices4x4(&cam.GetProjectionMatrix(), &cam.GetWorldToCameraMatrix(), &worldToView);
    Rectf viewRect = rect;
    viewRect.y = 1 - viewRect.y - viewRect.height;
    Rectf modRect = rect;
    modRect.x = modRect.x * 2.0f - 1.0f;
    modRect.y = -(modRect.y * 2.0f - 1.0f);
    modRect.width *= 2.0f;
    modRect.height *= 2.0f;
    modRect.y -= modRect.height;

    std::set<GameObject*>* selection;
    if (selectPrefabAssetsOnly)
    {
        selection = new std::set<GameObject*>();
    }
    else
    {
        selection = result;
    }

    const RenderNodeQueue& queue = pickingCuller.GetResults();

    for (size_t i = 0; i != queue.GetRenderNodesCount(); i++)
    {
        const RenderNode& node = queue.GetNode(i);
        BaseRenderer* renderer = node.GetBaseRendererMainThread();

        if (ShouldSkipRendererForPicking(renderer))
            continue;

        GameObject* go = NULL;

        Unity::Component* component = dynamic_pptr_cast<Unity::Component*>(GetPickingObjectFromRenderer(renderer));
        if (component != NULL)
            go = component->GetGameObjectPtr();

        if (go != NULL && !go->IsMarkedVisible())
            continue;

        while (go->IsMarkedVisible() == GameObject::kVisibleAsChild)
        {
            Transform *parent = go->GetComponent<Transform>().GetParent();
            if (parent)
                go = parent->GetGameObjectPtr();
            else
                break;
        }

        // select meshes
        MeshFilter* mf = go->QueryComponent<MeshFilter>();
        if (mf)
        {
            Mesh* mesh = mf->GetSharedMesh();
            if (mesh)
            {
                AABB bounds = mesh->GetBounds();
                Matrix4x4f objectMatrix;
                Matrix4x4f localToWorldMatrix = go->GetComponent<Transform>().GetLocalToWorldMatrix();
                MultiplyMatrices4x4(&worldToView, &localToWorldMatrix, &objectMatrix);
                if (BoundsInRect(bounds, modRect, objectMatrix))
                    selection->insert(go);
                else
                {
                    // If the object is in our list, we check against the verts (it has already be broadphase-culled)
                    bool found = true;
                    for (StrideIterator<Vector3f> i = mesh->GetVertexBegin(); i != mesh->GetVertexEnd(); i++)
                    {
                        Vector3f v;
                        objectMatrix.PerspectiveMultiplyPoint3(*i, v);
                        if (!modRect.Contains(v.x, v.y))
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                        selection->insert(go);
                }
                continue;
            }
        }
        // select misc renderers
        const bool isParticleRenderer = go->QueryComponent<ParticleSystemRenderer>();
        if (!isParticleRenderer && BoundsInRect(node.rendererData.m_TransformInfo.worldAABB, modRect, worldToView))
        {
            selection->insert(go);
            continue;
        }
    }

    UserData data;
    data.camera = &cam;
    data.list = selection;
    data.viewRect = &viewRect;
    GizmoManager::Get().CallFunctorForGizmos(CheckGizmosInRect, &data);

    for (std::set<GameObject*>::iterator i = selection->begin(); i != selection->end(); i++)
    {
        if (IsPartOfNonAssetPrefabInstance(**i))
            result->insert(GetOutermostPrefabInstanceRoot(*i));
        else if (IsPartOfPrefabAsset(**i))
            result->insert((*i)->GetComponent<Transform>().GetRoot().GetGameObjectPtr());
        else
            result->insert(*i);
    }
    if (selectPrefabAssetsOnly)
        delete selection;
}

struct ObjectWithDistance
{
    float distance;
    GameObject *go;

    ObjectWithDistance(float dist, GameObject *_go)
    {
        distance = dist;
        go = _go;
    }
};

static bool CompareObjectDistances(const ObjectWithDistance &v1, const ObjectWithDistance &v2)
{
    return (v1.distance < v2.distance);
}

static bool AddToSortedObjectsIfCanFindVerticesOnRenderer(Transform* t, Renderer* r, const std::set<Transform*>* ignoreObjects, Ray& cameraRay, std::vector<ObjectWithDistance>& sortedObjects)
{
    MeshFilter* mf = r->QueryComponent<MeshFilter>();
    if (!mf || !mf->GetSharedMesh())
    {
        SpriteRenderer* sr = r->QueryComponent<SpriteRenderer>();
        if (!sr || !sr->GetSprite())
            return false;
    }

    if (ignoreObjects && ignoreObjects->find(t) != ignoreObjects->end())
        return false;

    AABB worldAABB;
    r->GetWorldAABB(worldAABB);
    sortedObjects.push_back(ObjectWithDistance(cameraRay.SqrDistToPoint(worldAABB.GetCenter()) - SqrMagnitude(worldAABB.GetExtent()), &r->GetGameObject()));

    return true;
}

static void ProcessVertexForPicking(const Vector3f& worldV, const Vector2f& screenPoint, Camera& camera, float& minDist, bool& found, Vector3f *point)
{
    Vector3f p = camera.WorldToScreenPoint(worldV);
    // skip points behind the camera
    if (p.z > 0.0f)
    {
        float dist = SqrMagnitude(Vector2f(p.x, p.y) - screenPoint);
        if (dist < minDist)
        {
            minDist = dist;
            *point = worldV;
            found = true;
        }
    }
}

bool FindNearestVertex(const std::vector<Transform*> *objectsToSearchIn, const std::set<Transform*> *ignoreObjects, Vector2f screenPoint, Camera &camera, Vector3f *point)
{
    std::vector<ObjectWithDistance> sortedObjects;

    Ray cameraRay = camera.ScreenPointToRay(screenPoint);
    if (objectsToSearchIn == NULL)
    {
        PickingCuller pickingCuller(camera, false, false, NULL, NULL);
        const RenderNodeQueue& queue = pickingCuller.GetResults();

        size_t nodeCount = queue.GetRenderNodesCount();
        for (size_t i = 0; i != nodeCount; i++)
        {
            BaseRenderer* baseRenderer = queue.GetNode(i).GetBaseRendererMainThread();

            if (!baseRenderer->IsRenderer())
                continue;

            Renderer* r = static_cast<Renderer*>(baseRenderer);
            AddToSortedObjectsIfCanFindVerticesOnRenderer(&r->GetComponent<Transform>(), r, ignoreObjects, cameraRay, sortedObjects);
        }
    }
    else
    {
        // should this only be for objects in the selection AFTER a proper camera culling?
        for (std::vector<Transform*>::const_iterator i = objectsToSearchIn->cbegin(); i != objectsToSearchIn->cend(); i++)
        {
            Renderer* r = (*i)->QueryComponent<Renderer>();
            if (!r)
                continue;
            AddToSortedObjectsIfCanFindVerticesOnRenderer(*i, r, ignoreObjects, cameraRay, sortedObjects);
        }
    }

    std::sort(sortedObjects.begin(), sortedObjects.end(), CompareObjectDistances);

    float minDist = std::numeric_limits<float>::infinity();
    bool found = false;

    for (std::vector<ObjectWithDistance>::iterator i = sortedObjects.begin(); i != sortedObjects.end(); i++)
    {
        Matrix4x4f localToWorldMatrix = i->go->GetComponent<Transform>().GetLocalToWorldMatrix();

        MeshFilter* mf = i->go->QueryComponent<MeshFilter>();
        if (mf)
        {
            Mesh* mesh = mf->GetSharedMesh();

            // go over all mesh points
            for (StrideIterator<Vector3f> v = mesh->GetVertexBegin(), end = mesh->GetVertexEnd(); v != end; ++v)
            {
                Vector3f worldV = localToWorldMatrix.MultiplyPoint3(*v);
                ProcessVertexForPicking(worldV, screenPoint, camera, minDist, found, point);
            }
        }
        else
        {
            // If there is no MeshFilter, then there must be a SpriteRenderer
            SpriteRenderer* sr = i->go->QueryComponent<SpriteRenderer>();
            AABB localAABB;
            sr->GetLocalAABB(localAABB);

            // Go over
            Vector3f v[4];
            GetAABBVerticesForSprite(localAABB, v);
            for (int i = 0; i < 4; ++i)
            {
                Vector3f worldV = localToWorldMatrix.MultiplyPoint3(v[i]);
                ProcessVertexForPicking(worldV, screenPoint, camera, minDist, found, point);
            }
        }
    }

    return found;
}

#pragma once

#include <set>
#include <vector>
#include "Runtime/Math/Rect.h"
#include "Runtime/Shaders/ShaderTags.h"

class Camera;
class GameObject;
class Material;
class Object;
class Shader;
class Transform;
class Vector2f;
class Vector3f;
template<class T>
class PPtr;

struct SelectionEntry
{
    Object* object;
    int materialIndex;
    int score;
    friend bool operator<(const SelectionEntry& lhs, const SelectionEntry& rhs) { return lhs.object < rhs.object; }
};
typedef std::set<SelectionEntry> SelectionList;

/// Finds all Renderer that are within a rect centered around screenPosition with size screenSizePickRect
/// All results are written to selection and the number of objects that hit the rect is returned
/// Culls Renderers that are marked !ShouldDisplayInEditor
int PickObjects(Camera& camera, UInt32 layers, const Vector2f& screenPosition, const Vector2f& screenSizePickRect, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, SelectionList* selection);


// Returns the closest GameObject at screen position pos
Object* PickClosestObject(Camera& cam, UInt32 layers, const Vector2f& screenPosition, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, int* pickedMaterialIndex = NULL);
GameObject* PickClosestGO(Camera& cam, UInt32 layers, const Vector2f& screenPosition, std::set<GameObject*> *ignore, std::set<GameObject*> *filter, int* pickedMaterialIndex = NULL);

Material* GetPickMaterial();
Material *GetAlphaPickMaterial();
Material *GetAlphaPickMaterialNoZWrite();

bool IsPickingInProgress();
// Returns GameObjects in screen rect
void PickRectObjects(Camera& cam, const Rectf& rect, std::set<GameObject*>* result, bool selectPrefabAssetsOnly);

bool FindNearestVertex(const std::vector<Transform*> *objectsToSearchIn, const std::set<Transform*> *ignoreObjects, Vector2f screenPoint, Camera &camera, Vector3f *point);

void GetPickingSettings(Material* baseMaterial, Shader* baseShader, bool& useAlpha, bool& ZWrite, ShaderTagID& subshaderTypeID);

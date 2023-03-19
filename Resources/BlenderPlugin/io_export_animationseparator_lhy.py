bl_info = {
        "name":         "Fbx Animation Splitter for Stride3D Engine",
        "category":     "Import-Export",
        "version":      (0,0,5),
        "blender":      (3,3,0),
        "location":     "File > Import-Export",
        "description":  "Split Animation Export",
        "category":     "Import-Export"
        }
        
import bpy
import os


def main(context):
    startStates = []
    blend_file_path = bpy.data.filepath
    directory = os.path.dirname(blend_file_path)
#    target_file = os.path.join(directory, os.path.splitext(context.blend_data.filepath)[0] + '.fbx')
#    bpy.ops.export_scene.fbx(filepath = target_file, check_existing = True, object_types = {'MESH'})
    #record original state
    target_file = ''
    for i in range(0, len(bpy.data.actions)):
        print(str(i) + " " + str(bpy.data.actions[i]) + " " + str(bpy.data.actions[i].use_fake_user))
        startStates.append(bpy.data.actions[i].use_fake_user)
        bpy.data.actions[i].use_fake_user = False
    
    for i in range(0, len(bpy.data.actions)):
        if bpy.data.actions[i].name == 'ArmatureAction':
            print('skipping')

        elif startStates[i]:
            bpy.data.actions[i].use_fake_user = True
            #export here
            
            context.object.animation_data.action = bpy.data.actions[i]
            context.scene.frame_end = int(bpy.data.actions[i].frame_range[1])

            target_file = os.path.join(directory, 'animation_'+ bpy.data.actions[i].name + '.fbx')
            target_file = target_file.replace('|', '_')
            print(str(bpy.data.actions[i].name))
            bpy.ops.export_scene.fbx(filepath = target_file,
                check_existing = False,
                object_types = {'ARMATURE'},
                bake_anim = True,
                bake_anim_use_nla_strips = False,
                bake_anim_use_all_actions = False,
                bake_anim_force_startend_keying = True,
                add_leaf_bones = False,
                use_armature_deform_only = True)
                
            bpy.data.actions[i].use_fake_user = False

    
    #导出一份mesh和骨架
    project_name = bpy.context.blend_data.filepath.replace('/', '\\').split("\\")[-1].split('.')[0]
    project_name = project_name + '.fbx'
    print(project_name)
    target_file = os.path.join(directory, project_name)
    bpy.ops.export_scene.fbx(filepath = target_file,
                check_existing = False,
                object_types = {'ARMATURE', 'MESH'},
                bake_anim = False,
                bake_anim_use_nla_strips = False,
                bake_anim_use_all_actions = False,
                bake_anim_force_startend_keying = False,
                add_leaf_bones = False,
                use_armature_deform_only = True)

    #revert to what it was
    for i in range(0, len(bpy.data.actions)):
        bpy.data.actions[i].use_fake_user = startStates[i]


class SplitAnimations(bpy.types.Operator):
    """Split and Export Animations"""
    bl_idname = "object.splitanimations"
    bl_label = "Split and Export Animations"
    
    def execute(self, context):
        main(context)
        return {'FINISHED'}

def menu_func(self, context):
    self.layout.operator(SplitAnimations.bl_idname)

def register():
    bpy.utils.register_class(SplitAnimations)
    bpy.types.TOPBAR_MT_file_export.append(menu_func)
    
def unregister():
    bpy.utils.unregister_class(SplitAnimations)
    bpy.types.TOPBAR_MT_file_export.remove(menu_func)

if __name__ == "__main__":
    register()

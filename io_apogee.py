bl_info = {
    "name": "Geo Format Exporter",
    "description": "Writes geometry format to disk",
    "author": "Mark Kughler (GoliathForgeOnline)",
    "version": (1, 0),
    "blender": (2, 79, 0),
    "location": "File > Export > GEO",
    "warning": "",
    "wiki_url": "",
    "tracker_url": "",
    "support": 'COMMUNITY',
    "category": "Import-Export"
}

import bpy
import bmesh
import struct #https://docs.python.org/3/library/struct.html
from bpy import context

def triangulateObject(obj):
    me = obj.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bmesh.ops.triangulate(bm, faces=bm.faces[:])
    bm.to_mesh(me)
    bm.free()
    
    
def writeObject(self, context):
    ob = context.object
    uv_layer = ob.data.uv_layers.active.data
    
    vertBuff = []
    uvBuff   = []
    faceBuff = []
    #rebuild vertex, uv and face indices excluding duplicates
    for poly in ob.data.polygons:
        for index in poly.loop_indices:
            thisVertex = ob.data.vertices[ob.data.loops[index].vertex_index].co 
            thisUV = uv_layer[index].uv
            
            #check if already in the list
            i = 0
            found = 0
            for v in vertBuff:
                if(abs(v.x-thisVertex.x) <= max(1e-09 * max(abs(v.x), abs(thisVertex.x)), 0.0)):
                    if(abs(v.y-thisVertex.y) <= max(1e-09 * max(abs(v.y), abs(thisVertex.y)), 0.0)):
                        if(abs(v.z-thisVertex.z) <= max(1e-09 * max(abs(v.z), abs(thisVertex.z)), 0.0)):
                            if(abs(uvBuff[i].x-thisUV.x) <= max(1e-09 * max(abs(uvBuff[i].x), abs(thisUV.x)), 0.0)):
                                if(abs(uvBuff[i].y-thisUV.y) <= max(1e-09 * max(abs(uvBuff[i].y), abs(thisUV.y)), 0.0)):
                                    faceBuff.append(int(i))
                                    found = 1
                                    break
                i+=1
            #otherwise stash a new vertex
            if(found==0):
                faceBuff.append(len(vertBuff)) #index
                vertBuff.append(thisVertex)    #float, float, float
                uvBuff.append(thisUV)          #float, float
                
    #write to file
    if(self.format == "OPT_A"):
        with open(self.filepath, 'w') as ofile:
            ofile.write("%d " % len(vertBuff)) #num unique vertex/uv pairs
            ofile.write("%d " % len(faceBuff)) #num indices
            for v in vertBuff:
                ofile.write("%f %f %f " % v[:])
            for t in uvBuff:
                ofile.write("%f %f " % t[:])
            for p in faceBuff:
                ofile.write("%d " % p)
            ofile.close()
        return {'FINISHED'}
    else:
        
        with open(self.filepath, 'wb') as ofile:
            ofile.write(struct.pack('H', len(vertBuff))) 
            ofile.write(struct.pack('H', len(faceBuff)))
        
            for v in vertBuff:
                ofile.write(struct.pack('3f', v.x, v.y, v.z)) #v[:])) #"%f %f %f " % v[:])
            for t in uvBuff:
                ofile.write(struct.pack('2f', t.x, t.y)) #t[:])) #"%f %f " % t[:])
            for p in faceBuff:
                ofile.write(struct.pack('H', p)) #"%d " % p)
            ofile.close()
        return {'FINISHED'}    



class ObjectExport(bpy.types.Operator):
    """My object export script"""
    bl_idname = "object.export_geo"
    bl_label = "Geo Format Export"
    bl_options = {'REGISTER', 'UNDO'}
    filename_ext = ".geo"
    
    total           = bpy.props.IntProperty(name="Steps", default=2, min=1, max=100)
    filter_glob     = bpy.props.StringProperty(default="*.geo", options={'HIDDEN'}, maxlen=255)
    use_setting     = bpy.props.BoolProperty(name="Selected only", description="Export selected mesh items only", default=True)
    use_triangulate = bpy.props.BoolProperty(name="Triangulate", description="Triangulate object", default=True)
    format          = bpy.props.EnumProperty(name="Format", description="Choose between two items", items=(('OPT_A', "ASCII ", "Text file format"), ('OPT_B', "Binary", "Binary file format")), default='OPT_A')

    filepath = bpy.props.StringProperty(subtype='FILE_PATH')    
    
    def execute(self, context):
        if(context.active_object.mode == 'EDIT'):
            bpy.ops.object.mode_set(mode='OBJECT')
            
        if(self.use_triangulate):
            triangulateObject(context.active_object)
            
        writeObject(self, context);        
        return {'FINISHED'}

    def invoke(self, context, event):
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}



# Add trigger into a dynamic menu
def menu_func_export(self, context):
    self.layout.operator(ObjectExport.bl_idname, text="Geometry Export (.geo)")
    

def register():
    bpy.utils.register_class(ObjectExport)
    bpy.types.TOPBAR_MT_file_export.append(menu_func_export)


def unregister():
    bpy.utils.unregister_class(ObjectExport)
    bpy.types.INFO_MT_file_export.remove(menu_func_export)


if __name__ == "__main__":
    register()
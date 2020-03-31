from __future__ import annotations
import bpy
import numpy as np


class RRTCoralSettings:
    max_x: float = 5
    max_y: float = 5
    max_z: float = 4

    limits: np.array = np.array([max_x, max_y, max_z])
    init_pos: np.array = np.array([max_x/2, max_y/2, 0])

    dq: float = 0.2

    resolution: float = 0.0075
    render_resolution: float = 0.1
    base_thickness: float = 0.2


class Node:
    def __init__(self, location_):
        self.location = location_
        self.parent = None
        self.children = []


class RRT:
    def __init__(self, settings):

        self.settings = settings
        self.metaballs = bpy.data.metaballs.new('MetaBall')
        obj = bpy.data.objects.new('MetaBallObj', self.metaballs)
        bpy.context.collection.objects.link(obj)
        self.metaballs.resolution = self.settings.resolution
        self.metaballs.render_resolution = self.settings.render_resolution
        self.num_balls = 0

        self.tree = None

        self.node_list = []
        self.first_node = Node(self.settings.init_pos)
        self.node_list.append(self.first_node)

    def build_rrt(self, K):
        for k in range(K):
            qrand = self.rand_conf()
            qnear = self.nearest_vertex(qrand)
            self.new_conf(qnear, qrand, self.settings.dq)

    def rand_conf(self):
        return self.settings.limits * np.random.random_sample(3)

    def nearest_vertex(self, vert):
        nearest = None
        nearest_dist = 9999

        for node in self.node_list:
            if nearest is None:
                nearest = node
            else:
                dist = np.linalg.norm(node.location-vert)
                if nearest_dist > dist:
                    nearest = node
                    nearest_dist = dist
        return nearest

    def new_conf(self, qnear, qrand, dq):
        if np.linalg.norm(qnear.location - qrand) < dq:
            new_node = Node(qrand)
        else:
            qnew = dq * ((qrand - qnear.location) / np.linalg.norm(qrand - qnear.location)) + qnear.location
            new_node = Node(qnew)

        new_node.parent = qnear

        qnear.children.append(new_node)
        self.node_list.append(new_node)

    def create_metaball(self, node, scale):
        self.num_balls += 1
        """
        Adds a new metaball
        """
        element = self.metaballs.elements.new()
        element.co = node
        element.radius = scale

    def draw_lines(self, p1, p2):
        bpy.ops.surface.primitive_nurbs_surface_sphere_add(radius=.2, location=p1)
        # bpy.ops.surface.primitive_nurbs_surface_sphere_add(radius=.05, location=(-1, -1, 0))
        bpy.ops.surface.primitive_nurbs_surface_sphere_add(radius=.2, location=p2)

        bpy.ops.curve.primitive_bezier_curve_add()
        obj = bpy.context.object
        obj.data.dimensions = '3D'
        obj.data.fill_mode = 'FULL'
        obj.data.bevel_depth = 0.1
        obj.data.bevel_resolution = 4
        # set first point to centre of sphere1
        obj.data.splines[0].bezier_points[0].co = p1
        obj.data.splines[0].bezier_points[0].handle_left_type = 'VECTOR'
        # set second point to centre of sphere2
        obj.data.splines[0].bezier_points[1].co = p2
        obj.data.splines[0].bezier_points[1].handle_left_type = 'VECTOR'

    def traverse_all_nodes(self, root):
        for node in root.children:
            # self.create_metaball(node.location.tolist())
            self.create_metaball(node.location.tolist(), self.settings.base_thickness)
            # self.draw_lines(root.location.tolist(), node.location.tolist())
            self.traverse_all_nodes(node)


        '''
        if root:
            # First recur on left child
            printInorder(root.left)

            # then print the data of node
            print(root.val),

            # now recur on right child
            printInorder(root.right)
        '''


if __name__ == "__main__":
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()

    generator_settings = RRTCoralSettings()

    coral_tree_generator = RRT(generator_settings)
    coral_tree_generator.build_rrt(200)
    root = coral_tree_generator.first_node
    coral_tree_generator.traverse_all_nodes(root)

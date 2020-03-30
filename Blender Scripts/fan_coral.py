import dataclasses
import math
import random

import bpy
# import bmesh


@dataclasses.dataclass(frozen=True)
class Vector3:
    x: float
    y: float
    z: float

    def as_tuple(self):
        return (self.x, self.y, self.z)


@dataclasses.dataclass(frozen=True)
class SphericalCoordinates:
    xy: float
    z: float


@dataclasses.dataclass(frozen=True)
class FanCoralGeneratorSettings:
    resolution: float = 0.02
    render_resolution: float = 0.02
    base_thickness: float = 0.1
    bumpiness: float = 0.5
    scatter: float = 0.7
    branch_scale_scatter: float = 0.3
    decay_prob: float = 0.99
    decay_factor: float = 0.9
    new_branch_prob: float = 0.6
    min_branch_width: float = 0.05
    min_distance_to_close_branch: float = 0.5
    min_z_radians: float = math.pi * 0.1
    max_distance_from_base: float = 3.0


class FanCoralGenerator:
    def __init__(self, settings: FanCoralGeneratorSettings):
        self.settings = settings
        self.metaballs = bpy.data.metaballs.new('MetaBall')
        obj = bpy.data.objects.new('MetaBallObj', self.metaballs)
        bpy.context.collection.objects.link(obj)
        self.metaballs.resolution = self.settings.resolution
        self.metaballs.render_resolution = self.settings.render_resolution
        self.num_balls = 0

    def extend_branch(
            self,
            node: Vector3,
            direction: SphericalCoordinates,
            scale: float,
            length_from_base: float) -> None:
        """
        Recursively add nodes to branch
        """
        # either continue or split branch
        num_new_nodes = 1
        if random.random() < self.settings.new_branch_prob:
            num_new_nodes = 2
        for i in range(num_new_nodes):
            # taper branch
            new_scale = scale
            if random.random() < self.settings.decay_prob:
                new_scale = scale * self.settings.decay_factor

            # if creating new branch, taper further
            if i != 0:
                new_scale *= random.uniform(
                    1 - self.settings.branch_scale_scatter, 1)

            # add new node
            restrict_xy_angle = i == 0
            new_direction = self.get_new_direction(
                direction, restrict_xy_angle)
            distance = (scale + new_scale) * self.settings.bumpiness
            new_node = Vector3(
                x=node.x + distance * (
                    math.cos(new_direction.xy) * math.cos(new_direction.z)),
                y=0.0,
                z=node.z + distance * math.sin(new_direction.z))
            self.create_metaball(new_node, new_scale)

            # possible continue branch
            new_length_from_base = length_from_base + distance
            if not self.should_close_branch(new_scale, new_length_from_base):
                self.extend_branch(
                    node=new_node,
                    direction=new_direction,
                    scale=new_scale,
                    length_from_base=new_length_from_base)

    def create_metaball(self, node: Vector3, scale: float) -> None:
        self.num_balls += 1
        """
        Adds a new metaball
        """
        element = self.metaballs.elements.new()
        element.co = node.as_tuple()
        element.radius = scale

    def get_new_direction(
            self,
            direction: SphericalCoordinates,
            restrict_angle: bool) -> SphericalCoordinates:
        """
        Get direction for next node based on current direction
        """
        # get new z direction within allowed bounds
        if restrict_angle:
            new_z_direction = direction.z + random.uniform(
                self.settings.scatter * -1,
                self.settings.scatter)
            if new_z_direction < self.settings.min_z_radians:
                new_z_direction = self.settings.min_z_radians
            elif new_z_direction > 1 - self.settings.min_z_radians:
                new_z_direction = 1 - self.settings.min_z_radians

            xy = direction.xy + self.settings.scatter * random.uniform(
                math.pi * -1,
                math.pi)
        else:
            new_z_direction = random.uniform(-1 * math.pi, math.pi)
            xy = random.uniform(
                math.pi * -1,
                math.pi)

        # determine new direction
        new_direction = SphericalCoordinates(
            xy=xy,
            z=new_z_direction)
        return new_direction

    def should_close_branch(
            self,
            scale: float,
            length_from_base: float) -> bool:
        """
        Close branch when it's fully decayed
        """
        if length_from_base < self.settings.min_distance_to_close_branch:
            return False
        elif length_from_base > self.settings.max_distance_from_base:
            return True
        elif scale < self.settings.min_branch_width:
            return True
        else:
            return False

    def generate(self):
        """
        Generate a coral mesh
        """
        # start in a random direction within bounds
        start_node = Vector3(x=0.0, y=0.0, z=0.0)
        start_direction = SphericalCoordinates(
            xy=random.uniform(0, math.pi * 2),
            z=math.pi / 2)
        self.extend_branch(
            node=start_node,
            direction=start_direction,
            scale=self.settings.base_thickness,
            length_from_base=0.0)
        print(f"Generated coral with {self.num_balls} balls!")


if __name__ == "__main__":
    generator_settings = FanCoralGeneratorSettings()
    generator = FanCoralGenerator(generator_settings)
    generator.generate()

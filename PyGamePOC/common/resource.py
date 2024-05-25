import pygame.image

file_images = {}
pixel_images = {}
pixel_animations = {}


class ImageResource:
    def __init__(self, id: str, filename: str, offset: tuple, size: tuple):
        self.id = id
        self.filename = filename
        self.offset = offset
        self.size = size

    def render(self):
        return get_image_surface(self)


class AnimationResource:
    def __init__(self, id: str, filename: str, offset: tuple, size: tuple, frames: int):
        self.id = id
        self.filename = filename
        self.offset = offset
        self.size = size
        self.frames = frames
        self.current_frame = 0

    def render(self, repeat=True):
        surface = get_animation_frame_surface(self, self.current_frame)
        if self.current_frame + 1 >= self.frames:
            if repeat:
                self.current_frame = 0
        else:
            self.current_frame = self.current_frame + 1
        return surface


def play_animation(animation_resource: AnimationResource, start_frame: int = 0, repeat=True):
    pygame.init()
    clock = pygame.time.Clock()
    flags = pygame.SHOWN | pygame.SCALED | pygame.RESIZABLE  # | pygame.FULLSCREEN
    screen = pygame.display.set_mode(animation_resource.size, flags=flags)

    frame = start_frame
    load_animation(animation_resource)

    running = True
    while running:
        screen.fill('white')

        surface = get_animation_frame_surface(animation_resource, frame)

        if frame + 1 >= animation_resource.frames:
            if repeat:
                frame = 0
        else:
            frame = frame + 1

        # process event
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False

        # render scene
        screen.blit(surface, surface.get_rect())

        # flip() the display to put your work on screen
        pygame.display.flip()
        clock.tick(12)

    pygame.quit()


def get_full_image_surface(filename: str):
    global file_images
    if filename not in file_images.keys():
        image_surface = pygame.image.load(filename).convert_alpha()
        image_rect = image_surface.get_rect()
        file_images[filename] = (pygame.image.tobytes(image_surface, 'ARGB'), image_rect.size)
    buffer = file_images[filename]
    return pygame.image.frombytes(buffer[0], buffer[1], 'ARGB')


# def get_image_surface(filename: str, offset: tuple, size: tuple):
#     source_surface = get_full_image_surface(filename)
#     rect = pygame.Rect(offset, size)
#     pxarray = pygame.PixelArray(source_surface)
#     newarray = pxarray[rect.x:rect.x+rect.width, rect.y:rect.y+rect.height]
#
#     return newarray.make_surface()


def get_image_surface(image_resource: ImageResource):
    global pixel_images
    if image_resource.id not in pixel_images.keys():
        source_surface = get_full_image_surface(image_resource.filename)
        rect = pygame.Rect(image_resource.offset, image_resource.size)
        pxarray = pygame.PixelArray(source_surface)
        newarray = pxarray[rect.x:rect.x+rect.width, rect.y:rect.y+rect.height]
        pixel_images[image_resource.id] = newarray
    newarray = pixel_images[image_resource.id]
    return newarray.make_surface()


def load_animation(animation_resource: AnimationResource):
    global pixel_animations
    if animation_resource.id not in pixel_animations.keys():
        source_surface = get_full_image_surface(animation_resource.filename)
        pxarray = pygame.PixelArray(source_surface)
        pixel_animations[animation_resource.id] = []
        (x, y) = animation_resource.offset
        width = animation_resource.size[0]
        for i in range(animation_resource.frames):
            rect = pygame.Rect((x + i * width, y), animation_resource.size)
            newarray = pxarray[rect.x:rect.x+rect.width, rect.y:rect.y+rect.height]
            pixel_animations[animation_resource.id].append(newarray)


def get_animation_frame_surface(animation_resource: AnimationResource, frame: int):
    global pixel_animations
    newarray = pixel_animations[animation_resource.id][frame]
    return newarray.make_surface()


if __name__ == "__main__":
    resource = AnimationResource('BALL', '../resources/imgs/JumpBall.png', (0, 0), (16, 16), 12)
    play_animation(resource, start_frame=5, repeat=False)


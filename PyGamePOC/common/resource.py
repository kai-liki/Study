import pygame.image

file_images = {}
pixel_images = {}


class ImageResource:
    id: str
    filename: str
    offset: tuple
    size: tuple

    def __init__(self, id: str, filename: str, offset: tuple, size: tuple):
        self.id = id
        self.filename = filename
        self.offset = offset
        self.size = size


def get_full_image_surface(filename: str):
    global file_images
    if filename not in file_images.keys():
        image_surface = pygame.image.load(filename).convert_alpha()
        image_rect = image_surface.get_rect()
        file_images[filename] = (pygame.image.tobytes(image_surface, 'ARGB'), image_rect.size)
    buffer = file_images[filename]
    return pygame.image.frombytes(buffer[0], buffer[1], 'ARGB')
    # return pygame.image.load(filename)


# def get_image_surface(filename: str, offset: tuple, size: tuple):
#     source_surface = get_full_image_surface(filename)
#     rect = pygame.Rect(offset, size)
#     pxarray = pygame.PixelArray(source_surface)
#     newarray = pxarray[rect.x:rect.x+rect.width, rect.y:rect.y+rect.height]
#
#     return newarray.make_surface()


def get_image_surface(resource: ImageResource):
    global pixel_images
    if resource.id not in pixel_images.keys():
        source_surface = get_full_image_surface(resource.filename)
        rect = pygame.Rect(resource.offset, resource.size)
        pxarray = pygame.PixelArray(source_surface)
        newarray = pxarray[rect.x:rect.x+rect.width, rect.y:rect.y+rect.height]
        pixel_images[resource.id] = newarray
    newarray = pixel_images[resource.id]
    return newarray.make_surface()


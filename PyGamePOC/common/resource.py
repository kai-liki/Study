import pygame.image

images = {}


def get_image_surface(filename):
    global images
    if filename not in images.keys():
        image_surface = pygame.image.load(filename).convert_alpha()
        image_rect = image_surface.get_rect()
        images[filename] = (pygame.image.tobytes(image_surface, 'ARGB'), image_rect.size)
    buffer = images[filename]
    return pygame.image.frombytes(buffer[0], buffer[1], 'ARGB')
    # return pygame.image.load(filename)



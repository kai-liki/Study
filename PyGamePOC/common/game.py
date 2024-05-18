import pygame

from PyGamePOC.common.control import Controller


class GameController:
    screen: pygame.surface.Surface
    running = False
    controllers = {}
    current_controller = None
    clock: pygame.time.Clock

    def __init__(self):
        pygame.init()
        pygame.font.init()
        self.clock = pygame.time.Clock()
        flags = pygame.SHOWN | pygame.SCALED #| pygame.FULLSCREEN
        self.screen = pygame.display.set_mode((320, 240), flags=flags)
        self.running = False

    def initialize_controllers(self, controllers: dict):
        from PyGamePOC.reception.reception import ReceptionController
        self.current_controller = ReceptionController()
        self.current_controller.set_game(self)

        for key, value in controllers:
            assert type(value) is Controller
            self.controllers[key] = value
            value.set_game(self)

    def run(self):
        self.running = True
        while self.running:
            current_scene = self.current_controller.get_scene()

            # process event
            for event in pygame.event.get():
                if event.type == pygame.QUIT:
                    self.current_controller.process(event_name='on_quit_game')
                else:
                    current_scene.handle_event(event)

            # render scene
            scene_surface = current_scene.render()
            self.screen.blit(scene_surface, scene_surface.get_rect())

            # flip() the display to put your work on screen
            pygame.display.flip()

            self.clock.tick(60)

        pygame.quit()


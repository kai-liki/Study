import pygame


class GameController:
    screen: pygame.surface.Surface
    running = False
    current_controller = None
    clock: pygame.time.Clock

    def __init__(self):
        pygame.init()
        pygame.font.init()
        self.clock = pygame.time.Clock()
        self.screen = pygame.display.set_mode((500, 500))
        self.running = False

        from PyGamePOC.reception.reception import ReceptionController
        self.current_controller = ReceptionController()

    def run(self):
        self.running = True
        self.current_controller.set_game(self)
        current_scene = self.current_controller.get_scene()
        while self.running:
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



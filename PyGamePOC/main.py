# Example file showing a circle moving on screen
import pygame

from common.game import GameController

if __name__ == "__main__":
    fonts = pygame.font.get_fonts()
    print(fonts)
    game = GameController()

    from reception.reception import ReceptionController
    from root import POCRootController

    game.initialize_controllers({
        "RECEPTION": ReceptionController,
        "ROOT": POCRootController
    }, "RECEPTION")
    game.run()


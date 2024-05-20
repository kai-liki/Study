# Example file showing a circle moving on screen
import pygame

from PyGamePOC.common.game import GameController

if __name__ == "__main__":
    game = GameController()

    from PyGamePOC.reception.reception import ReceptionController
    from PyGamePOC.root import POCRootController

    game.initialize_controllers({
        "RECEPTION": ReceptionController,
        "ROOT": POCRootController
    }, "RECEPTION")
    game.run()


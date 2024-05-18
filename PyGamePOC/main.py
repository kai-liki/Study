# Example file showing a circle moving on screen
import pygame

from PyGamePOC.common.game import GameController
from PyGamePOC.root import POCRootController

if __name__ == "__main__":
    game = GameController()
    game.initialize_controllers({
        "ROOT": POCRootController()
    })
    game.run()


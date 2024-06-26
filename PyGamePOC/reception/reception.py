from common.control import Controller, Scene, Button
from common.game import GameController
from reception.model import Reception


class NewGameController(Controller):
    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_new_game()

    def on_new_game(self):
        root_controller = self.get_game().new_controller('ROOT')
        root_controller.initialize()
        self.get_game().current_controller = root_controller


class LoadGameController(Controller):
    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_load_game()

    def on_load_game(self):
        root_controller = self.get_game().new_controller('ROOT')
        saved_data = root_controller.load()
        root_controller.initialize(saved_data)
        self.get_game().current_controller = root_controller


class QuitGameController(Controller):
    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_quit_game()

    def on_quit_game(self):
        # print("Quit Game")
        self.game.running = False


class ReceptionController(Controller):
    def __init__(self):
        self.scene_reception = Scene('reception')
        self.button_new_game = Button('btn_new')
        self.button_load_game = Button('btn_load')
        self.button_quit_game = Button('btn_quit')
        self.new_game_controller = NewGameController()
        self.load_game_controller = LoadGameController()
        self.quit_game_controller = QuitGameController()

        self.model = Reception()

        self.scene_reception.register_controller(self)
        self.scene_reception.set_size(320, 240)
        self.scene_reception.set_bg_color((10, 10, 10))

        self.button_new_game.register_controller(self.new_game_controller)
        self.button_new_game.set_label(self.model.options[0])
        self.button_new_game.set_position(60, 50)
        self.button_new_game.set_size(200, 50)
        self.button_new_game.set_layer(1)
        self.button_new_game.set_bg_color((255, 0, 0))
        self.button_new_game.set_label_color((255, 0, 0))
        self.scene_reception.add_control(self.button_new_game)

        self.button_load_game.register_controller(self.load_game_controller)
        self.button_load_game.set_label(self.model.options[1])
        self.button_load_game.set_position(60, 105)
        self.button_load_game.set_size(200, 50)
        self.button_load_game.set_layer(2)
        self.button_load_game.set_bg_color((255, 255, 255))
        self.button_load_game.set_label_color((0, 255, 0))
        self.scene_reception.add_control(self.button_load_game)

        self.button_quit_game.register_controller(self.quit_game_controller)
        self.button_quit_game.set_label(self.model.options[2])
        self.button_quit_game.set_position(60, 160)
        self.button_quit_game.set_size(200, 50)
        self.button_quit_game.set_layer(3)
        self.button_quit_game.set_bg_color((255, 255, 255))
        self.button_quit_game.set_label_color((0, 255, 0))
        self.scene_reception.add_control(self.button_quit_game)

    def get_scene(self):
        return self.scene_reception

    def set_game(self, game: GameController):
        super().set_game(game)
        self.new_game_controller.set_game(game)
        self.load_game_controller.set_game(game)
        self.quit_game_controller.set_game(game)

    def process(self, **kwargs):
        print(kwargs)
        event_name = kwargs['event_name']
        if event_name == 'on_quit_game':
            self.on_quit_game()

    def on_quit_game(self):
        self.game.running = False



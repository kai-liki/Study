from PyGamePOC.common.control import Controller, Scene, Button
import PyGamePOC.common.edu as edu


class RootController(Controller):
    scene_intro = Scene('introduction')
    scene_main = Scene('main')
    scene_event = Scene('event')
    scene_ending = Scene('ending')

    button_save = Button('func_btn_save')
    button_load = Button('func_btn_load')
    button_quit = Button('func_btn_quit')
    button_next = Button('func_btn_next')
    function_buttons = [button_next, button_save, button_load, button_quit]

    action_button = []

    def __init__(self, world: edu.World, almanac: edu.Almanac, ui: edu.UI):
        self.world = world
        self.almanac = almanac
        self.ui = ui

    def get_scene(self):
        if self.world.status is edu.STATUS_START:
            pass

    def process(self):
        super().process()


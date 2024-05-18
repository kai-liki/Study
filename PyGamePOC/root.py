from PyGamePOC.common.control import Controller, Scene, Button, Panel
import PyGamePOC.common.edu as edu


class CookController(Controller):
    def __init__(self, world: edu.World):
        super().__init__(self)
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_cook()

    def on_cook(self):
        pass


class NextController(Controller):
    def __init__(self, world: edu.World):
        super().__init__(self)
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_next_cycle()

    def on_next_cycle(self):
        pass


class LeaveController(Controller):
    def __init__(self, world: edu.World):
        super().__init__(self)
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_leave()

    def on_leave(self):
        pass


class POCRootController(Controller):
    scene_start = Scene('SCENE_START')
    scene_intro = Scene('SCENE_INTRO')
    scene_main = Scene('SCENE_MAIN')
    scene_ending = Scene('SCENE_ENDING')

    panel_calendar = Panel('PANEL_CALENDAR')
    panel_action = Panel('PANEL_ACTION')

    button_quit = Button('FUNC_BTN_QUIT')
    button_next = Button('FUNC_BTN_NEXT')

    button_cook = Button('ACTION_BTN_COOK')

    def init_scene(self):
        pass

    def __init__(self):
        world, almanac = edu.build_POC()
        self.world = world
        self.almanac = almanac
        self.init_scene()

    def get_scene(self):
        if self.world.status is edu.STATE_START:
            pass

    def process(self):
        super().process()


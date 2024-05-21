import pygame

from common.control import Controller, Scene, Button, Panel, VisibleControl, Label
import common.edu as edu
from common.game import GameController
from common.resource import ImageResource


class CookController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_cook()

    def on_cook(self):
        self.world.status = edu.STATE_ACTION_BEGIN


class NextController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_next_cycle()

    def on_next_cycle(self):
        self.world.status = edu.STATE_CYCLE_END


class LeaveController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.on_leave()

    def on_leave(self):
        self.world.status = edu.STATE_IDLE
        self.game.current_controller = self.get_game().new_controller('RECEPTION')


class ActionController(Controller):
    def __init__(self, world: edu.World):
        super().__init__()
        self.world = world

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'mouse_click':
            self.world.status = edu.STATE_ACTION_END


class POCRootController(Controller):

    def load_ui_resource(self):
        self.ui_resources = {
            'LOGO': ImageResource('LOGO', 'resources/imgs/logo.gif', (0, 0), (100, 62)),
            'CHAR_WIFE_NORMAL': ImageResource('CHAR_WIFE_NORMAL', 'resources/imgs/NPC01.gif', (76, 0), (70, 95)),
            'CHAR_WIFE_ANGRY': ImageResource('CHAR_WIFE_ANGRY', 'resources/imgs/NPC01.gif', (1, 0), (70, 95)),
            'SCENE_INTRO': ImageResource('SCENE_INTRO', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
            'SCENE_MAIN': ImageResource('SCENE_MAIN', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
            'SCENE_ACTION': ImageResource('SCENE_ACTION', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
            'SCENE_ENDING': ImageResource('SCENE_ENDING', 'resources/imgs/Livingroom.gif', (0, 0), (320, 240)),
            'SCENE_KITCHEN': ImageResource('SCENE_KITCHEN', 'resources/imgs/Kitchen.gif', (0, 0), (320, 240)),
            'PANEL_ACTION': ImageResource('PANEL_ACTION', 'resources/imgs/ActionPanel.gif', (0, 0), (200, 150)),
            'PANEL_MESSAGE': ImageResource('PANEL_MESSAGE', 'resources/imgs/MessagePanel.gif', (0, 0), (150, 150)),
            'PANEL_DIALOG': ImageResource('PANEL_DIALOG', 'resources/imgs/DialogPanel.gif', (0, 0), (320, 120)),
            'ACTION_SAMPLE_NORMAL': ImageResource('ACTION_SAMPLE_NORMAL', 'resources/imgs/ActionButton.gif', (0, 0), (50, 50)),
            'ACTION_SAMPLE_OVER': ImageResource('ACTION_SAMPLE_OVER', 'resources/imgs/ActionButton.gif', (50, 0), (50, 50)),
            'ACTION_SAMPLE_DOWN': ImageResource('ACTION_SAMPLE_DOWN', 'resources/imgs/ActionButton.gif', (100, 0), (50, 50)),
            'OPTION_DEFAULT_NORMAL': ImageResource('OPTION_DEFAULT_NORMAL', 'resources/imgs/OptionButton.gif', (0, 0), (280, 20)),
            'OPTION_DEFAULT_OVER': ImageResource('OPTION_DEFAULT_OVER', 'resources/imgs/OptionButton.gif', (0, 20), (280, 20)),
            'OPTION_DEFAULT_DOWN': ImageResource('OPTION_DEFAULT_DOWN', 'resources/imgs/OptionButton.gif', (0, 40), (280, 20))
        }

    def init_scene(self):
        # Build START scene
        self.scene_start.register_controller(self)
        self.scene_start.set_size(320, 240)
        self.scene_start.set_bg_color((255, 255, 255))

        logo = Panel('PANEL_LOGO')
        logo.set_size(100, 62)
        logo.set_position(100, 90)
        logo.set_bg_color((255, 255, 255))
        logo.set_background_image(self.ui_resources['LOGO'])
        self.scene_start.add_control(logo)

        # Build INTRO scene
        self.scene_intro.register_controller(self)
        self.scene_intro.set_size(320, 240)
        self.scene_intro.set_bg_color((0, 0, 0))
        # self.scene_intro.set_background_image(self.ui_resources['SCENE_INTRO'])

        intro_label = Label('LABEL_INTRO')
        intro_label.set_labels(['This is a POC game.', 'It comes from my life.', '欢迎建议！'])
        intro_label.set_label_color((255, 255, 255))
        intro_label.set_size(100, 100)
        intro_label.set_position(100, 90)
        intro_label.set_font(pygame.font.SysFont('arial', 10))
        self.scene_intro.add_control(intro_label)

        # Build MAIN scene
        self.scene_main.register_controller(self)
        self.scene_main.set_size(320,240)
        self.scene_main.set_background_image(self.ui_resources['SCENE_MAIN'])

        # self.label_calendar.register_controller(Controller())
        self.label_calendar.set_size(40, 20)
        self.label_calendar.set_position(260, 20)
        self.label_calendar.set_labels([''])
        self.label_calendar.set_label_color((255, 255, 255))
        self.label_calendar.set_font(pygame.font.SysFont('arial', 18))
        self.scene_main.add_control(self.label_calendar)

        self.button_cook.set_size(50, 50)
        self.button_cook.set_position(50, 190)
        self.button_cook.set_label('Cook')
        self.button_cook.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.cook_controller = CookController(self.world)
        self.button_cook.register_controller(self.cook_controller)
        self.scene_main.add_control(self.button_cook)

        self.button_next.set_size(50, 50)
        self.button_next.set_position(190, 190)
        self.button_next.set_label('Next')
        self.button_next.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.next_controller = NextController(self.world)
        self.button_next.register_controller(self.next_controller)
        self.scene_main.add_control(self.button_next)

        self.button_leave.set_size(50, 50)
        self.button_leave.set_position(120, 190)
        self.button_leave.set_label('Leave')
        self.button_leave.set_button_images((self.ui_resources['ACTION_SAMPLE_NORMAL'],
                                            self.ui_resources['ACTION_SAMPLE_OVER'],
                                            self.ui_resources['ACTION_SAMPLE_DOWN']))
        self.leave_controller = LeaveController(self.world)
        self.button_leave.register_controller(self.leave_controller)
        self.scene_main.add_control(self.button_leave)

        # Build ENDING scene
        self.scene_ending.register_controller(self)
        self.scene_ending.set_size(320, 240)
        self.scene_ending.set_background_image(self.ui_resources['SCENE_ENDING'])

        # Build ACTION scene
        self.scene_action.register_controller(self)
        self.scene_action.set_size(320, 240)
        self.scene_action.set_background_image(self.ui_resources['SCENE_KITCHEN'])

        self.panel_action.set_size(200, 150)
        self.panel_action.set_position(60, 45)
        self.panel_action.set_bg_color((255, 255, 255))
        self.panel_action.set_background_image(self.ui_resources['PANEL_ACTION'])
        self.action_panel_controller = ActionController(self.world)
        self.panel_action.register_controller(self.action_panel_controller)
        self.scene_action.add_control(self.panel_action)


    def __init__(self):
        self.scene_start = Scene('SCENE_START')
        self.scene_intro = Scene('SCENE_INTRO')
        self.scene_main = Scene('SCENE_MAIN')
        self.scene_action = Scene('SCENE_ACTION')
        self.scene_ending = Scene('SCENE_ENDING')

        self.panel_calendar = Panel('PANEL_CALENDAR')
        self.panel_action = Panel('PANEL_ACTION')

        self.button_leave = Button('FUNC_BTN_LEAVE')
        self.button_next = Button('FUNC_BTN_NEXT')

        self.button_cook = Button('ACTION_BTN_COOK')

        self.leave_controller: LeaveController
        self.next_controller: NextController

        self.cook_controller: CookController

        self.label_calendar = Label('LABEL_CALENDAR')

        self.ui_resources: dict

        self.load_ui_resource()
        world, almanac = edu.build_POC(self.ui_resources)
        self.world = world
        self.almanac = almanac
        self.init_scene()

    def set_game(self, game: GameController):
        super().set_game(game)
        self.leave_controller.set_game(game)
        self.next_controller.set_game(game)
        self.cook_controller.set_game(game)

    def get_scene(self):
        if self.world.status is edu.STATE_START:
            self.start_tick()
            return self.scene_start
        elif self.world.status is edu.STATE_INTRO:
            return self.scene_intro
        elif self.world.status is edu.STATE_CYCLE_BEGIN:
            self.on_cycle_begin()
            return self.scene_main
        elif self.world.status is edu.STATE_WAIT_ACTION:
            return self.scene_main
        elif self.world.status is edu.STATE_CYCLE_END:
            self.on_cycle_end()
            return self.scene_main
        elif self.world.status is edu.STATE_ACTION_BEGIN:
            return self.scene_action
        elif self.world.status is edu.STATE_ACTION_END:
            self.on_action_end()
            return self.scene_action
        elif self.world.status is edu.STATE_IDLE:
            return self.scene_ending

    def process(self, **kwargs):
        event_name = kwargs['event_name']
        if event_name == 'on_key_down':
            key = kwargs['key']
            self.on_key_down(key)
        # elif event_name == 'mouse_click':
        #     self.on_mouse_click()

    def on_cycle_begin(self):
        player = self.world.characters['PLAYER']
        player.strength = player.max_strength
        self.button_cook.set_is_enabled(True)
        current = self.world.calendar.next()
        if current is not None:
            self.label_calendar.set_labels([current])

        self.world.status = edu.STATE_WAIT_ACTION

    def on_cycle_end(self):
        self.world.status = edu.STAGE_CYCLE_BEGIN

    def on_action_end(self):
        player = self.world.characters['PLAYER']
        player.strength = 0 if player.strength < 10 else player.strength - 10
        self.button_cook.set_is_enabled(player.strength >= 10)
        self.world.status = edu.STATE_WAIT_ACTION

    def on_key_down(self, key):
        if self.world.status is edu.STATE_INTRO:
            self.world.status = edu.STATE_CYCLE_BEGIN
            # TOTO: Add event
        if self.world.status is edu.STATE_ACTION_BEGIN:
            self.world.status = edu.STATE_ACTION_END
        elif self.world.status is edu.STATE_IDLE:
            self.game.current_controller = self.game.new_controller('RECEPTION')

    def on_mouse_click(self):
        pass

    def start_tick(self):
        stage = self.world.stages[edu.STAGE_START]
        if stage.tick > stage.max_tick:
            self.world.status = edu.STAGE_INTRO
        else:
            stage.tick = stage.tick + 1

